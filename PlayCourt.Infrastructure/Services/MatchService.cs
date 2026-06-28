using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Matches;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;

namespace PlayCourt.Infrastructure.Services
{
    public sealed class MatchService : IMatchService
    {
        private readonly PlayCourtDbContext _context;

        public MatchService(PlayCourtDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<List<MatchResponseDto>>> SearchAsync(
            int userId,
            MatchSearchRequestDto request)
        {
            var profile = await GetPlayerProfileAsync(userId);
            if (profile is null)
            {
                return PagedResponse<List<MatchResponseDto>>.Fail("Player profile not found.");
            }

            var query = MatchQuery()
                .AsNoTracking()
                .Where(match =>
                    match.Status != MatchStatus.Cancelled &&
                    match.Status != MatchStatus.Completed &&
                    match.StartAt > DateTimeOffset.UtcNow);

            if (!request.IncludeFull)
            {
                query = query.Where(match => match.Status == MatchStatus.Open);
            }

            if (request.SportId.HasValue)
            {
                query = query.Where(match => match.SportId == request.SportId.Value);
            }

            if (request.SkillLevel.HasValue)
            {
                var level = (SkillLevel)request.SkillLevel.Value;
                query = query.Where(match =>
                    (!match.RequiredSkillLevelMin.HasValue || match.RequiredSkillLevelMin <= level) &&
                    (!match.RequiredSkillLevelMax.HasValue || match.RequiredSkillLevelMax >= level));
            }

            if (!string.IsNullOrWhiteSpace(request.Location))
            {
                var location = request.Location.Trim();
                query = query.Where(match =>
                    (match.LocationDescription != null && match.LocationDescription.Contains(location)) ||
                    (match.Court != null &&
                        (match.Court.Venue.Name.Contains(location) ||
                         match.Court.Venue.Address.Contains(location))));
            }

            if (request.StartFrom.HasValue)
            {
                query = query.Where(match => match.StartAt >= request.StartFrom.Value);
            }

            if (request.StartTo.HasValue)
            {
                query = query.Where(match => match.StartAt <= request.StartTo.Value);
            }

            var totalCount = await query.CountAsync();
            var matches = await query
                .OrderBy(match => match.StartAt)
                .ThenBy(match => match.Id)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return PagedResponse<List<MatchResponseDto>>.Ok(
                matches.Select(match => MapMatch(match, profile.Id)).ToList(),
                totalCount,
                request.PageIndex,
                request.PageSize,
                "Matches retrieved successfully.");
        }

        public async Task<ApiResponse<List<MatchResponseDto>>> GetRecommendedAsync(
            int userId,
            int limit = 20)
        {
            if (limit is < 1 or > 100)
            {
                return ApiResponse<List<MatchResponseDto>>.Fail("Limit must be between 1 and 100.");
            }

            var profile = await _context.UserProfiles
                .AsNoTracking()
                .Include(item => item.User)
                .Include(item => item.PlayerSports)
                .FirstOrDefaultAsync(item =>
                    item.UserId == userId &&
                    item.User.Role == UserRole.Player &&
                    item.User.Status == UserStatus.Active);

            if (profile is null)
            {
                return ApiResponse<List<MatchResponseDto>>.Fail("Player profile not found.");
            }

            if (profile.PlayerSports.Count == 0)
            {
                return ApiResponse<List<MatchResponseDto>>.Ok(
                    [],
                    "Add at least one sport to your profile to receive recommendations.");
            }

            var sportIds = profile.PlayerSports.Select(item => item.SportId).ToList();
            var matches = await MatchQuery()
                .AsNoTracking()
                .Where(match =>
                    match.HostId != profile.Id &&
                    match.Status == MatchStatus.Open &&
                    match.StartAt > DateTimeOffset.UtcNow &&
                    sportIds.Contains(match.SportId))
                .ToListAsync();

            var ranked = matches
                .Where(match =>
                {
                    var playerSport = profile.PlayerSports.First(item => item.SportId == match.SportId);
                    return IsSkillCompatible(
                        playerSport.SkillLevel,
                        match.RequiredSkillLevelMin,
                        match.RequiredSkillLevelMax);
                })
                .Where(match => match.Participants.All(item => item.PlayerId != profile.Id))
                .Where(match => match.JoinRequests.All(item =>
                    item.PlayerId != profile.Id ||
                    item.Status != MatchJoinRequestStatus.Pending))
                .Where(match => match.Invitations.All(item =>
                    item.InviteeId != profile.Id ||
                    item.Status != MatchInvitationStatus.Pending))
                .OrderByDescending(match => RecommendationScore(match, profile))
                .ThenBy(match => match.StartAt)
                .Take(limit)
                .Select(match => MapMatch(match, profile.Id))
                .ToList();

            return ApiResponse<List<MatchResponseDto>>.Ok(
                ranked,
                "Recommended matches retrieved successfully.");
        }

        public async Task<ApiResponse<MatchDetailResponseDto>> GetByIdAsync(int userId, int matchId)
        {
            var profile = await GetPlayerProfileAsync(userId);
            if (profile is null)
            {
                return ApiResponse<MatchDetailResponseDto>.Fail("Player profile not found.");
            }

            var match = await MatchQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == matchId);
            if (match is null)
            {
                return ApiResponse<MatchDetailResponseDto>.Fail("Match not found.");
            }

            var participantDtos = match.Participants
                .OrderByDescending(item => item.IsHost)
                .ThenBy(item => item.JoinedAt)
                .Select(item => new MatchParticipantDto
                {
                    ProfileId = item.PlayerId,
                    FullName = item.Player.FullName,
                    AvatarUrl = item.Player.AvatarUrl,
                    SkillLevel = item.Player.PlayerSports
                        .FirstOrDefault(sport => sport.SportId == match.SportId)?
                        .SkillLevel.ToString(),
                    IsHost = item.IsHost,
                    JoinedAt = item.JoinedAt
                })
                .ToList();

            return ApiResponse<MatchDetailResponseDto>.Ok(
                new MatchDetailResponseDto
                {
                    Match = MapMatch(match, profile.Id),
                    Participants = participantDtos
                },
                "Match retrieved successfully.");
        }

        public async Task<ApiResponse<MatchResponseDto>> CreateAsync(
            int userId,
            CreateMatchRequestDto request)
        {
            var profile = await GetPlayerProfileAsync(userId);
            if (profile is null)
            {
                return ApiResponse<MatchResponseDto>.Fail("Player profile not found.");
            }

            var validationError = await ValidateMatchRequestAsync(profile, request);
            if (validationError is not null)
            {
                return ApiResponse<MatchResponseDto>.Fail(validationError);
            }

            var match = new Match
            {
                HostId = profile.Id,
                SportId = request.SportId,
                CourtId = request.CourtId,
                LocationDescription = Normalize(request.LocationDescription),
                StartAt = request.StartAt,
                EndAt = request.EndAt,
                RequiredSkillLevelMin = ToSkillLevel(request.RequiredSkillLevelMin),
                RequiredSkillLevelMax = ToSkillLevel(request.RequiredSkillLevelMax),
                MaxParticipants = request.MaxParticipants,
                CostDescription = Normalize(request.CostDescription),
                Description = Normalize(request.Description),
                Status = MatchStatus.Open,
                CreatedAt = DateTimeOffset.UtcNow
            };
            match.Participants.Add(new MatchParticipant
            {
                PlayerId = profile.Id,
                IsHost = true,
                JoinedAt = DateTimeOffset.UtcNow
            });

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            var created = await MatchQuery()
                .AsNoTracking()
                .SingleAsync(item => item.Id == match.Id);
            return ApiResponse<MatchResponseDto>.Ok(
                MapMatch(created, profile.Id),
                "Match created successfully.");
        }

        public async Task<ApiResponse<MatchResponseDto>> UpdateAsync(
            int userId,
            int matchId,
            UpdateMatchRequestDto request)
        {
            var profile = await GetPlayerProfileAsync(userId);
            if (profile is null)
            {
                return ApiResponse<MatchResponseDto>.Fail("Player profile not found.");
            }

            var match = await _context.Matches
                .Include(item => item.Participants)
                .FirstOrDefaultAsync(item => item.Id == matchId);
            if (match is null)
            {
                return ApiResponse<MatchResponseDto>.Fail("Match not found.");
            }

            if (match.HostId != profile.Id)
            {
                return ApiResponse<MatchResponseDto>.Fail("Only the host can update this match.");
            }

            if (match.Status != MatchStatus.Open)
            {
                return ApiResponse<MatchResponseDto>.Fail("Only an open match can be updated.");
            }

            if (request.SportId != match.SportId)
            {
                return ApiResponse<MatchResponseDto>.Fail(
                    "The sport of an existing match cannot be changed.");
            }

            if (request.MaxParticipants < match.Participants.Count)
            {
                return ApiResponse<MatchResponseDto>.Fail(
                    "Maximum participants cannot be lower than the current participant count.");
            }

            var validationError = await ValidateMatchRequestAsync(profile, request);
            if (validationError is not null)
            {
                return ApiResponse<MatchResponseDto>.Fail(validationError);
            }

            match.SportId = request.SportId;
            match.CourtId = request.CourtId;
            match.LocationDescription = Normalize(request.LocationDescription);
            match.StartAt = request.StartAt;
            match.EndAt = request.EndAt;
            match.RequiredSkillLevelMin = ToSkillLevel(request.RequiredSkillLevelMin);
            match.RequiredSkillLevelMax = ToSkillLevel(request.RequiredSkillLevelMax);
            match.MaxParticipants = request.MaxParticipants;
            match.CostDescription = Normalize(request.CostDescription);
            match.Description = Normalize(request.Description);
            match.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();

            var updated = await MatchQuery()
                .AsNoTracking()
                .SingleAsync(item => item.Id == match.Id);
            return ApiResponse<MatchResponseDto>.Ok(
                MapMatch(updated, profile.Id),
                "Match updated successfully.");
        }

        public async Task<ApiResponse<MatchResponseDto>> CancelAsync(int userId, int matchId)
        {
            var profile = await GetPlayerProfileAsync(userId);
            if (profile is null)
            {
                return ApiResponse<MatchResponseDto>.Fail("Player profile not found.");
            }

            var match = await _context.Matches.FirstOrDefaultAsync(item => item.Id == matchId);
            if (match is null)
            {
                return ApiResponse<MatchResponseDto>.Fail("Match not found.");
            }

            if (match.HostId != profile.Id)
            {
                return ApiResponse<MatchResponseDto>.Fail("Only the host can cancel this match.");
            }

            if (match.Status is MatchStatus.Cancelled or MatchStatus.Completed)
            {
                return ApiResponse<MatchResponseDto>.Fail("This match can no longer be cancelled.");
            }

            match.Status = MatchStatus.Cancelled;
            match.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();

            var cancelled = await MatchQuery()
                .IgnoreQueryFilters()
                .AsNoTracking()
                .SingleAsync(item => item.Id == match.Id);
            return ApiResponse<MatchResponseDto>.Ok(
                MapMatch(cancelled, profile.Id),
                "Match cancelled successfully.");
        }

        public async Task<ApiResponse<MatchJoinRequestDto>> RequestToJoinAsync(
            int userId,
            int matchId)
        {
            var profile = await GetPlayerProfileAsync(userId);
            if (profile is null)
            {
                return ApiResponse<MatchJoinRequestDto>.Fail("Player profile not found.");
            }

            var match = await MatchQuery().FirstOrDefaultAsync(item => item.Id == matchId);
            if (match is null)
            {
                return ApiResponse<MatchJoinRequestDto>.Fail("Match not found.");
            }

            var eligibilityError = await ValidatePlayerEligibilityAsync(profile, match);
            if (eligibilityError is not null)
            {
                return ApiResponse<MatchJoinRequestDto>.Fail(eligibilityError);
            }

            var joinRequest = match.JoinRequests.FirstOrDefault(item => item.PlayerId == profile.Id);
            if (joinRequest?.Status == MatchJoinRequestStatus.Pending)
            {
                return ApiResponse<MatchJoinRequestDto>.Fail("A join request is already pending.");
            }

            if (match.Invitations.Any(item =>
                    item.InviteeId == profile.Id &&
                    item.Status == MatchInvitationStatus.Pending))
            {
                return ApiResponse<MatchJoinRequestDto>.Fail(
                    "You already have a pending invitation for this match.");
            }

            if (joinRequest is null)
            {
                joinRequest = new MatchJoinRequest
                {
                    MatchId = match.Id,
                    PlayerId = profile.Id
                };
                _context.MatchJoinRequests.Add(joinRequest);
            }

            joinRequest.Status = MatchJoinRequestStatus.Pending;
            joinRequest.RequestedAt = DateTimeOffset.UtcNow;
            joinRequest.RespondedAt = null;
            await _context.SaveChangesAsync();

            joinRequest.Player = profile;
            return ApiResponse<MatchJoinRequestDto>.Ok(
                MapJoinRequest(joinRequest, match.SportId),
                "Join request sent successfully.");
        }

        public async Task<ApiResponse<MatchJoinRequestDto>> CancelJoinRequestAsync(
            int userId,
            int matchId)
        {
            var profile = await GetPlayerProfileAsync(userId);
            if (profile is null)
            {
                return ApiResponse<MatchJoinRequestDto>.Fail("Player profile not found.");
            }

            var joinRequest = await _context.MatchJoinRequests
                .Include(item => item.Player)
                    .ThenInclude(item => item.PlayerSports)
                .Include(item => item.Match)
                .FirstOrDefaultAsync(item =>
                    item.MatchId == matchId &&
                    item.PlayerId == profile.Id &&
                    item.Status == MatchJoinRequestStatus.Pending);
            if (joinRequest is null)
            {
                return ApiResponse<MatchJoinRequestDto>.Fail("Pending join request not found.");
            }

            var response = MapJoinRequest(joinRequest, joinRequest.Match.SportId);
            _context.MatchJoinRequests.Remove(joinRequest);
            await _context.SaveChangesAsync();
            return ApiResponse<MatchJoinRequestDto>.Ok(
                response,
                "Join request cancelled successfully.");
        }

        public async Task<ApiResponse<List<MatchJoinRequestDto>>> GetJoinRequestsAsync(
            int userId,
            int matchId)
        {
            var profile = await GetPlayerProfileAsync(userId);
            if (profile is null)
            {
                return ApiResponse<List<MatchJoinRequestDto>>.Fail("Player profile not found.");
            }

            var match = await _context.Matches
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == matchId);
            if (match is null)
            {
                return ApiResponse<List<MatchJoinRequestDto>>.Fail("Match not found.");
            }

            if (match.HostId != profile.Id)
            {
                return ApiResponse<List<MatchJoinRequestDto>>.Fail(
                    "Only the host can view join requests.");
            }

            var requests = await _context.MatchJoinRequests
                .AsNoTracking()
                .Include(item => item.Player)
                    .ThenInclude(item => item.PlayerSports)
                .Where(item => item.MatchId == matchId)
                .OrderBy(item => item.Status)
                .ThenBy(item => item.RequestedAt)
                .ToListAsync();
            return ApiResponse<List<MatchJoinRequestDto>>.Ok(
                requests.Select(item => MapJoinRequest(item, match.SportId)).ToList(),
                "Join requests retrieved successfully.");
        }

        public async Task<ApiResponse<MatchJoinRequestDto>> RespondToJoinRequestAsync(
            int userId,
            int matchId,
            int requestId,
            RespondJoinRequestDto request)
        {
            var profile = await GetPlayerProfileAsync(userId);
            if (profile is null)
            {
                return ApiResponse<MatchJoinRequestDto>.Fail("Player profile not found.");
            }

            if (!request.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase) &&
                !request.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
            {
                return ApiResponse<MatchJoinRequestDto>.Fail(
                    "Status must be Approved or Rejected.");
            }

            var match = await MatchQuery().FirstOrDefaultAsync(item => item.Id == matchId);
            if (match is null)
            {
                return ApiResponse<MatchJoinRequestDto>.Fail("Match not found.");
            }

            if (match.HostId != profile.Id)
            {
                return ApiResponse<MatchJoinRequestDto>.Fail(
                    "Only the host can respond to join requests.");
            }

            var joinRequest = match.JoinRequests.FirstOrDefault(item => item.Id == requestId);
            if (joinRequest is null)
            {
                return ApiResponse<MatchJoinRequestDto>.Fail("Join request not found.");
            }

            if (joinRequest.Status != MatchJoinRequestStatus.Pending)
            {
                return ApiResponse<MatchJoinRequestDto>.Fail(
                    "This join request has already been processed.");
            }

            var approved = request.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase);
            if (approved)
            {
                if (match.Status != MatchStatus.Open ||
                    match.StartAt <= DateTimeOffset.UtcNow ||
                    match.Participants.Count >= match.MaxParticipants)
                {
                    return ApiResponse<MatchJoinRequestDto>.Fail("The match is no longer available.");
                }

                match.Participants.Add(new MatchParticipant
                {
                    PlayerId = joinRequest.PlayerId,
                    JoinedAt = DateTimeOffset.UtcNow
                });
                joinRequest.Status = MatchJoinRequestStatus.Approved;
                if (match.Participants.Count >= match.MaxParticipants)
                {
                    match.Status = MatchStatus.Full;
                }
            }
            else
            {
                joinRequest.Status = MatchJoinRequestStatus.Rejected;
            }

            joinRequest.RespondedAt = DateTimeOffset.UtcNow;
            match.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();
            return ApiResponse<MatchJoinRequestDto>.Ok(
                MapJoinRequest(joinRequest, match.SportId),
                approved ? "Join request approved." : "Join request rejected.");
        }

        public async Task<ApiResponse<MatchResponseDto>> LeaveAsync(int userId, int matchId)
        {
            var profile = await GetPlayerProfileAsync(userId);
            if (profile is null)
            {
                return ApiResponse<MatchResponseDto>.Fail("Player profile not found.");
            }

            var match = await MatchQuery().FirstOrDefaultAsync(item => item.Id == matchId);
            if (match is null)
            {
                return ApiResponse<MatchResponseDto>.Fail("Match not found.");
            }

            if (match.HostId == profile.Id)
            {
                return ApiResponse<MatchResponseDto>.Fail(
                    "The host cannot leave the match; cancel it instead.");
            }

            var participant = match.Participants.FirstOrDefault(item => item.PlayerId == profile.Id);
            if (participant is null)
            {
                return ApiResponse<MatchResponseDto>.Fail("You are not a participant in this match.");
            }

            _context.MatchParticipants.Remove(participant);
            if (match.Status == MatchStatus.Full && match.StartAt > DateTimeOffset.UtcNow)
            {
                match.Status = MatchStatus.Open;
            }

            match.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();

            var updated = await MatchQuery()
                .AsNoTracking()
                .SingleAsync(item => item.Id == match.Id);
            return ApiResponse<MatchResponseDto>.Ok(
                MapMatch(updated, profile.Id),
                "You left the match successfully.");
        }

        public async Task<ApiResponse<List<PlayerMatchCandidateDto>>> GetCandidatesAsync(
            int userId,
            int matchId,
            int limit = 20)
        {
            if (limit is < 1 or > 100)
            {
                return ApiResponse<List<PlayerMatchCandidateDto>>.Fail(
                    "Limit must be between 1 and 100.");
            }

            var host = await GetPlayerProfileAsync(userId);
            if (host is null)
            {
                return ApiResponse<List<PlayerMatchCandidateDto>>.Fail("Player profile not found.");
            }

            var match = await MatchQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == matchId);
            if (match is null)
            {
                return ApiResponse<List<PlayerMatchCandidateDto>>.Fail("Match not found.");
            }

            if (match.HostId != host.Id)
            {
                return ApiResponse<List<PlayerMatchCandidateDto>>.Fail(
                    "Only the host can view matching players.");
            }

            if (match.Status != MatchStatus.Open || match.StartAt <= DateTimeOffset.UtcNow)
            {
                return ApiResponse<List<PlayerMatchCandidateDto>>.Fail(
                    "The match is no longer open.");
            }

            var excludedIds = match.Participants.Select(item => item.PlayerId)
                .Concat(match.JoinRequests
                    .Where(item => item.Status == MatchJoinRequestStatus.Pending)
                    .Select(item => item.PlayerId))
                .Concat(match.Invitations
                    .Where(item => item.Status == MatchInvitationStatus.Pending)
                    .Select(item => item.InviteeId))
                .Distinct()
                .ToList();

            var candidates = await _context.PlayerSports
                .AsNoTracking()
                .Include(item => item.UserProfile)
                    .ThenInclude(item => item.User)
                .Where(item =>
                    item.SportId == match.SportId &&
                    !excludedIds.Contains(item.UserProfileId) &&
                    item.UserProfile.User.Role == UserRole.Player &&
                    item.UserProfile.User.Status == UserStatus.Active)
                .ToListAsync();

            var result = candidates
                .Where(item => IsSkillCompatible(
                    item.SkillLevel,
                    match.RequiredSkillLevelMin,
                    match.RequiredSkillLevelMax))
                .Select(item => new PlayerMatchCandidateDto
                {
                    ProfileId = item.UserProfileId,
                    FullName = item.UserProfile.FullName,
                    AvatarUrl = item.UserProfile.AvatarUrl,
                    City = item.UserProfile.City,
                    SkillLevel = item.SkillLevel.ToString(),
                    MatchScore = CandidateScore(item, host)
                })
                .OrderByDescending(item => item.MatchScore)
                .ThenBy(item => item.FullName)
                .Take(limit)
                .ToList();

            return ApiResponse<List<PlayerMatchCandidateDto>>.Ok(
                result,
                "Matching players retrieved successfully.");
        }

        public async Task<ApiResponse<MatchInvitationDto>> InviteAsync(
            int userId,
            int matchId,
            CreateMatchInvitationDto request)
        {
            var host = await GetPlayerProfileAsync(userId);
            if (host is null)
            {
                return ApiResponse<MatchInvitationDto>.Fail("Player profile not found.");
            }

            var match = await MatchQuery().FirstOrDefaultAsync(item => item.Id == matchId);
            if (match is null)
            {
                return ApiResponse<MatchInvitationDto>.Fail("Match not found.");
            }

            if (match.HostId != host.Id)
            {
                return ApiResponse<MatchInvitationDto>.Fail(
                    "Only the host can invite players.");
            }

            var invitee = await _context.UserProfiles
                .Include(item => item.User)
                .Include(item => item.PlayerSports)
                .FirstOrDefaultAsync(item => item.Id == request.InviteeProfileId);
            if (invitee is null ||
                invitee.User.Role != UserRole.Player ||
                invitee.User.Status != UserStatus.Active)
            {
                return ApiResponse<MatchInvitationDto>.Fail("Invitee player not found.");
            }

            var eligibilityError = ValidatePlayerEligibility(invitee, match);
            if (eligibilityError is not null)
            {
                return ApiResponse<MatchInvitationDto>.Fail(eligibilityError);
            }

            if (match.JoinRequests.Any(item =>
                    item.PlayerId == invitee.Id &&
                    item.Status == MatchJoinRequestStatus.Pending))
            {
                return ApiResponse<MatchInvitationDto>.Fail(
                    "This player already has a pending join request.");
            }

            var invitation = match.Invitations.FirstOrDefault(item => item.InviteeId == invitee.Id);
            if (invitation is not null && invitation.Status == MatchInvitationStatus.Pending)
            {
                return ApiResponse<MatchInvitationDto>.Fail(
                    "An invitation is already pending for this player.");
            }

            if (invitation is null)
            {
                invitation = new MatchInvitation
                {
                    MatchId = match.Id,
                    InviterId = host.Id,
                    InviteeId = invitee.Id
                };
                _context.MatchInvitations.Add(invitation);
            }

            invitation.InviterId = host.Id;
            invitation.Status = MatchInvitationStatus.Pending;
            invitation.Message = Normalize(request.Message);
            invitation.InvitedAt = DateTimeOffset.UtcNow;
            invitation.RespondedAt = null;
            await _context.SaveChangesAsync();

            invitation.Match = match;
            invitation.Inviter = host;
            invitation.Invitee = invitee;
            return ApiResponse<MatchInvitationDto>.Ok(
                MapInvitation(invitation),
                "Match invitation sent successfully.");
        }

        public async Task<ApiResponse<List<MatchInvitationDto>>> GetMyInvitationsAsync(int userId)
        {
            var profile = await GetPlayerProfileAsync(userId);
            if (profile is null)
            {
                return ApiResponse<List<MatchInvitationDto>>.Fail("Player profile not found.");
            }

            var invitations = await _context.MatchInvitations
                .AsNoTracking()
                .Include(item => item.Match)
                    .ThenInclude(item => item.Sport)
                .Include(item => item.Inviter)
                .Include(item => item.Invitee)
                .Where(item => item.InviteeId == profile.Id)
                .OrderBy(item => item.Status)
                .ThenByDescending(item => item.InvitedAt)
                .ToListAsync();
            return ApiResponse<List<MatchInvitationDto>>.Ok(
                invitations.Select(MapInvitation).ToList(),
                "Match invitations retrieved successfully.");
        }

        public async Task<ApiResponse<MatchInvitationDto>> RespondToInvitationAsync(
            int userId,
            int invitationId,
            RespondMatchInvitationDto request)
        {
            var profile = await GetPlayerProfileAsync(userId);
            if (profile is null)
            {
                return ApiResponse<MatchInvitationDto>.Fail("Player profile not found.");
            }

            if (!request.Status.Equals("Accepted", StringComparison.OrdinalIgnoreCase) &&
                !request.Status.Equals("Declined", StringComparison.OrdinalIgnoreCase))
            {
                return ApiResponse<MatchInvitationDto>.Fail(
                    "Status must be Accepted or Declined.");
            }

            var invitation = await _context.MatchInvitations
                .Include(item => item.Match)
                    .ThenInclude(item => item.Sport)
                .Include(item => item.Match)
                    .ThenInclude(item => item.Participants)
                .Include(item => item.Inviter)
                .Include(item => item.Invitee)
                .FirstOrDefaultAsync(item => item.Id == invitationId);
            if (invitation is null || invitation.InviteeId != profile.Id)
            {
                return ApiResponse<MatchInvitationDto>.Fail("Match invitation not found.");
            }

            if (invitation.Status != MatchInvitationStatus.Pending)
            {
                return ApiResponse<MatchInvitationDto>.Fail(
                    "This invitation has already been processed.");
            }

            var accepted = request.Status.Equals("Accepted", StringComparison.OrdinalIgnoreCase);
            if (accepted)
            {
                var eligibilityError = ValidatePlayerEligibility(profile, invitation.Match);
                if (eligibilityError is not null)
                {
                    return ApiResponse<MatchInvitationDto>.Fail(eligibilityError);
                }

                invitation.Match.Participants.Add(new MatchParticipant
                {
                    PlayerId = profile.Id,
                    JoinedAt = DateTimeOffset.UtcNow
                });
                invitation.Status = MatchInvitationStatus.Accepted;
                if (invitation.Match.Participants.Count >= invitation.Match.MaxParticipants)
                {
                    invitation.Match.Status = MatchStatus.Full;
                }
            }
            else
            {
                invitation.Status = MatchInvitationStatus.Declined;
            }

            invitation.RespondedAt = DateTimeOffset.UtcNow;
            invitation.Match.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();
            return ApiResponse<MatchInvitationDto>.Ok(
                MapInvitation(invitation),
                accepted ? "Match invitation accepted." : "Match invitation declined.");
        }

        private IQueryable<Match> MatchQuery()
        {
            return _context.Matches
                .Include(item => item.Host)
                .Include(item => item.Sport)
                .Include(item => item.Court)
                    .ThenInclude(item => item!.Venue)
                .Include(item => item.Participants)
                    .ThenInclude(item => item.Player)
                        .ThenInclude(item => item.PlayerSports)
                .Include(item => item.JoinRequests)
                    .ThenInclude(item => item.Player)
                        .ThenInclude(item => item.PlayerSports)
                .Include(item => item.Invitations);
        }

        private async Task<UserProfile?> GetPlayerProfileAsync(int userId)
        {
            return await _context.UserProfiles
                .Include(item => item.User)
                .Include(item => item.PlayerSports)
                .FirstOrDefaultAsync(item =>
                    item.UserId == userId &&
                    item.User.Role == UserRole.Player &&
                    item.User.Status == UserStatus.Active);
        }

        private async Task<string?> ValidateMatchRequestAsync(
            UserProfile profile,
            CreateMatchRequestDto request)
        {
            if (request.MaxParticipants is < 2 or > 100)
            {
                return "Maximum participants must be between 2 and 100.";
            }

            if ((request.RequiredSkillLevelMin.HasValue &&
                 request.RequiredSkillLevelMin is < 0 or > 2) ||
                (request.RequiredSkillLevelMax.HasValue &&
                 request.RequiredSkillLevelMax is < 0 or > 2))
            {
                return "Skill level must be between 0 and 2.";
            }

            if (request.StartAt <= DateTimeOffset.UtcNow)
            {
                return "Match start time must be in the future.";
            }

            if (request.EndAt <= request.StartAt)
            {
                return "Match end time must be later than its start time.";
            }

            if (request.RequiredSkillLevelMin > request.RequiredSkillLevelMax)
            {
                return "Minimum skill level cannot exceed maximum skill level.";
            }

            var playerSport = profile.PlayerSports.FirstOrDefault(item => item.SportId == request.SportId);
            if (playerSport is null)
            {
                return "Add this sport to your player profile before creating a match.";
            }

            var sportExists = await _context.Sports
                .AnyAsync(item => item.Id == request.SportId && item.IsActive);
            if (!sportExists)
            {
                return "Active sport not found.";
            }

            if (request.CourtId.HasValue)
            {
                var validCourt = await _context.Courts
                    .Include(item => item.Venue)
                    .AnyAsync(item =>
                        item.Id == request.CourtId.Value &&
                        item.SportId == request.SportId &&
                        item.Status == CourtStatus.Available &&
                        item.Venue.Status == VenueStatus.Approved);
                if (!validCourt)
                {
                    return "The selected court is unavailable or does not support this sport.";
                }
            }
            else if (string.IsNullOrWhiteSpace(request.LocationDescription))
            {
                return "Location description is required when no court is selected.";
            }

            return null;
        }

        private async Task<string?> ValidatePlayerEligibilityAsync(UserProfile profile, Match match)
        {
            await Task.CompletedTask;
            return ValidatePlayerEligibility(profile, match);
        }

        private static string? ValidatePlayerEligibility(UserProfile profile, Match match)
        {
            if (match.Status != MatchStatus.Open ||
                match.StartAt <= DateTimeOffset.UtcNow ||
                match.Participants.Count >= match.MaxParticipants)
            {
                return "The match is no longer available.";
            }

            if (match.HostId == profile.Id ||
                match.Participants.Any(item => item.PlayerId == profile.Id))
            {
                return "You are already a participant in this match.";
            }

            var playerSport = profile.PlayerSports.FirstOrDefault(item => item.SportId == match.SportId);
            if (playerSport is null)
            {
                return "Add this sport to your player profile before joining.";
            }

            if (!IsSkillCompatible(
                    playerSport.SkillLevel,
                    match.RequiredSkillLevelMin,
                    match.RequiredSkillLevelMax))
            {
                return "Your skill level does not meet this match's requirements.";
            }

            return null;
        }

        private static bool IsSkillCompatible(
            SkillLevel level,
            SkillLevel? minimum,
            SkillLevel? maximum)
        {
            return (!minimum.HasValue || level >= minimum.Value) &&
                   (!maximum.HasValue || level <= maximum.Value);
        }

        private static int RecommendationScore(Match match, UserProfile profile)
        {
            var playerSport = profile.PlayerSports.First(item => item.SportId == match.SportId);
            var score = 70 - (10 * SkillDistance(
                playerSport.SkillLevel,
                match.RequiredSkillLevelMin,
                match.RequiredSkillLevelMax));
            if (!string.IsNullOrWhiteSpace(profile.City) &&
                GetMatchLocation(match).Contains(profile.City, StringComparison.OrdinalIgnoreCase))
            {
                score += 30;
            }

            return score;
        }

        private static int CandidateScore(PlayerSport candidate, UserProfile host)
        {
            var hostSport = host.PlayerSports.FirstOrDefault(item => item.SportId == candidate.SportId);
            var score = hostSport is null
                ? 70
                : 70 - (15 * Math.Abs((int)hostSport.SkillLevel - (int)candidate.SkillLevel));
            if (!string.IsNullOrWhiteSpace(host.City) &&
                string.Equals(host.City, candidate.UserProfile.City, StringComparison.OrdinalIgnoreCase))
            {
                score += 30;
            }

            return Math.Max(score, 0);
        }

        private static int SkillDistance(
            SkillLevel level,
            SkillLevel? minimum,
            SkillLevel? maximum)
        {
            if (minimum.HasValue && level < minimum.Value)
            {
                return (int)minimum.Value - (int)level;
            }

            if (maximum.HasValue && level > maximum.Value)
            {
                return (int)level - (int)maximum.Value;
            }

            return 0;
        }

        private static MatchResponseDto MapMatch(Match match, int currentProfileId)
        {
            var participantCount = match.Participants.Count;
            return new MatchResponseDto
            {
                Id = match.Id,
                HostProfileId = match.HostId,
                HostName = match.Host.FullName,
                HostAvatarUrl = match.Host.AvatarUrl,
                SportId = match.SportId,
                SportCode = match.Sport.Code,
                SportName = match.Sport.Name,
                CourtId = match.CourtId,
                CourtName = match.Court?.Name,
                VenueName = match.Court?.Venue.Name,
                LocationDescription = match.LocationDescription,
                StartAt = match.StartAt,
                EndAt = match.EndAt,
                RequiredSkillLevelMin = match.RequiredSkillLevelMin?.ToString(),
                RequiredSkillLevelMax = match.RequiredSkillLevelMax?.ToString(),
                MaxParticipants = match.MaxParticipants,
                ParticipantCount = participantCount,
                AvailableSlots = Math.Max(match.MaxParticipants - participantCount, 0),
                CostDescription = match.CostDescription,
                Description = match.Description,
                Status = match.Status.ToString(),
                IsHost = match.HostId == currentProfileId,
                IsParticipant = match.Participants.Any(item => item.PlayerId == currentProfileId),
                MyJoinRequestStatus = match.JoinRequests
                    .Where(item => item.PlayerId == currentProfileId)
                    .OrderByDescending(item => item.RequestedAt)
                    .FirstOrDefault()?
                    .Status.ToString(),
                CreatedAt = match.CreatedAt
            };
        }

        private static MatchJoinRequestDto MapJoinRequest(
            MatchJoinRequest request,
            int sportId)
        {
            return new MatchJoinRequestDto
            {
                Id = request.Id,
                MatchId = request.MatchId,
                PlayerProfileId = request.PlayerId,
                PlayerName = request.Player.FullName,
                PlayerAvatarUrl = request.Player.AvatarUrl,
                SkillLevel = request.Player.PlayerSports
                    .FirstOrDefault(item => item.SportId == sportId)?
                    .SkillLevel.ToString(),
                Status = request.Status.ToString(),
                RequestedAt = request.RequestedAt,
                RespondedAt = request.RespondedAt
            };
        }

        private static MatchInvitationDto MapInvitation(MatchInvitation invitation)
        {
            return new MatchInvitationDto
            {
                Id = invitation.Id,
                MatchId = invitation.MatchId,
                SportName = invitation.Match.Sport.Name,
                MatchStartAt = invitation.Match.StartAt,
                InviterProfileId = invitation.InviterId,
                InviterName = invitation.Inviter.FullName,
                InviteeProfileId = invitation.InviteeId,
                InviteeName = invitation.Invitee.FullName,
                Message = invitation.Message,
                Status = invitation.Status.ToString(),
                InvitedAt = invitation.InvitedAt,
                RespondedAt = invitation.RespondedAt
            };
        }

        private static SkillLevel? ToSkillLevel(int? value)
        {
            return value.HasValue ? (SkillLevel)value.Value : null;
        }

        private static string? Normalize(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static string GetMatchLocation(Match match)
        {
            return match.Court is null
                ? match.LocationDescription ?? string.Empty
                : $"{match.Court.Venue.Name} {match.Court.Venue.Address}";
        }
    }
}
