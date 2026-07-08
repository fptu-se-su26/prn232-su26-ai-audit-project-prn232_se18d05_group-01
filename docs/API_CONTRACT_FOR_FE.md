# PlayCourt API Contract for FrontEnd

> **LƯU Ý:** Tài liệu này dành cho lập trình viên FrontEnd phát triển ứng dụng PlayCourt mà không cần đọc mã nguồn backend.
> Toàn bộ các API đều trả về dạng bọc chuẩn:
> - **ApiResponse<T>**: Định dạng phản hồi đơn.
> - **PagedResponse<T>**: Định dạng phản hồi danh sách kèm phân trang.

## 1. Định dạng Phản hồi Chung (Common Wrappers)

### ApiResponse<T>
```typescript
interface ApiResponse<T> {
  success: boolean;   // true nếu thành công, false nếu có lỗi
  message: string;   // Thông điệp kết quả hoặc thông điệp lỗi
  data: T | null;    // Dữ liệu trả về (null nếu thất bại)
  errors: string[];  // Danh sách các lỗi chi tiết (validate, nghiệp vụ)
}
```

### PagedResponse<T>
```typescript
interface PagedResponse<T> {
  success: boolean;
  message: string;
  data: T | null;      // Thường là Array các DTO (ví dụ VenueDto[])
  errors: string[];
  totalCount: number;  // Tổng số bản ghi khớp bộ lọc trên server
  totalPages: number;  // Tổng số trang
  pageIndex: number;   // Trang hiện tại (1-indexed)
  pageSize: number;    // Kích thước trang
}
```

## 2. Danh sách Endpoints theo Module

## Module: Amenities

### `[GET]` /api/Amenities
- **Mô tả:** Action GetAll
- **Xác thực:** Public (Không yêu cầu)
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<IReadOnlyCollection<AmenityDto>>`
- **FE Screen đề xuất:** Màn hình Admin - Quản lý tiện ích hệ thống

### `[POST]` /api/Amenities
- **Mô tả:** Action Create
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Admin`)
- **Request Body:** `CreateAmenityRequestDto`
- **Response Shape:** `ApiResponse<AmenityDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Admin - Quản lý tiện ích hệ thống

### `[GET]` /api/Amenities/{id}
- **Mô tả:** Action GetById
- **Xác thực:** Public (Không yêu cầu)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<AmenityDto>`
- **FE Screen đề xuất:** Màn hình Admin - Quản lý tiện ích hệ thống

### `[PUT]` /api/Amenities/{id}
- **Mô tả:** Action Update
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Admin`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** `CreateAmenityRequestDto`
- **Response Shape:** `ApiResponse<AmenityDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Admin - Quản lý tiện ích hệ thống

### `[DELETE]` /api/Amenities/{id}
- **Mô tả:** Action Delete
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Admin`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<object>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Admin - Quản lý tiện ích hệ thống

## Module: Auth

### `[POST]` /api/Auth/register
- **Mô tả:** Action Register
- **Xác thực:** Public (Không yêu cầu)
- **Request Body:** `RegisterRequestDto`
- **Response Shape:** `ApiResponse<RegisterResponseDto>`
- **FE Screen đề xuất:** Màn hình Đăng ký (Register)

### `[POST]` /api/Auth/login
- **Mô tả:** Action Login
- **Xác thực:** Public (Không yêu cầu)
- **Request Body:** `LoginRequestDto`
- **Response Shape:** `ApiResponse<LoginResponseDto>`
- **FE Screen đề xuất:** Màn hình Đăng nhập (Login)

### `[POST]` /api/Auth/refresh-token
- **Mô tả:** Action RefreshToken
- **Xác thực:** Public (Không yêu cầu)
- **Request Body:** `RefreshTokenRequestDto`
- **Response Shape:** `ApiResponse<RefreshTokenResponseDto>`
- **FE Screen đề xuất:** Hệ thống xác thực ngầm (Auth Service)

### `[POST]` /api/Auth/logout
- **Mô tả:** Action Logout
- **Xác thực:** Public (Không yêu cầu)
- **Request Body:** `LogoutRequestDto`
- **Response Shape:** `ApiResponse<object>`
- **FE Screen đề xuất:** Hệ thống xác thực ngầm (Auth Service)

### `[POST]` /api/Auth/verify-email
- **Mô tả:** Action VerifyEmail
- **Xác thực:** Public (Không yêu cầu)
- **Request Body:** `VerifyEmailRequestDto`
- **Response Shape:** `ApiResponse<object>`
- **FE Screen đề xuất:** Màn hình Xác thực Email (Verify Email)

### `[POST]` /api/Auth/resend-verify-email
- **Mô tả:** Action ResendVerifyEmail
- **Xác thực:** Public (Không yêu cầu)
- **Request Body:** `ResendVerifyEmailRequestDto`
- **Response Shape:** `ApiResponse<object>`
- **FE Screen đề xuất:** Màn hình Xác thực Email (Verify Email)

### `[POST]` /api/Auth/forgot-password
- **Mô tả:** Action ForgotPassword
- **Xác thực:** Public (Không yêu cầu)
- **Request Body:** `ForgotPasswordRequestDto`
- **Response Shape:** `ApiResponse<object>`
- **FE Screen đề xuất:** Màn hình Đặt lại mật khẩu (Reset Password)

### `[POST]` /api/Auth/reset-password
- **Mô tả:** Action ResetPassword
- **Xác thực:** Public (Không yêu cầu)
- **Request Body:** `ResetPasswordRequestDto`
- **Response Shape:** `ApiResponse<object>`
- **FE Screen đề xuất:** Màn hình Đặt lại mật khẩu (Reset Password)

### `[POST]` /api/Auth/change-password
- **Mô tả:** Action ChangePassword
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Request Body:** `ChangePasswordRequestDto`
- **Response Shape:** `ApiResponse<object>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Màn hình Cá nhân - Đổi mật khẩu

## Module: Bookings

### `[POST]` /api/Bookings
- **Mô tả:** Action Create
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Request Body:** `CreateBookingRequestDto`
- **Response Shape:** `ApiResponse<BookingResponseDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Đặt sân - Thanh toán (Checkout)

### `[GET]` /api/Bookings/{id}
- **Mô tả:** Action GetById
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<BookingResponseDto>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Màn hình Lịch sử đặt sân (Booking History) / Quản lý đặt sân của cơ sở

### `[GET]` /api/Bookings/me
- **Mô tả:** Action GetMyBookings
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Query Parameters:**
  - `Status` (string): Bộ lọc query
  - `From` (string): Bộ lọc query
  - `To` (string): Bộ lọc query
  - `Page` (integer): Bộ lọc query
  - `PageSize` (integer): Bộ lọc query
- **Request Body:** Không có (Trống)
- **Response Shape:** `PagedResponse<IReadOnlyCollection<BookingResponseDto>>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Lịch sử đặt sân (Booking History)

### `[GET]` /api/venues/{venueId}/bookings
- **Mô tả:** Action GetVenueBookings
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Path Parameters:**
  - `venueId` (integer): Tham số đường dẫn
- **Query Parameters:**
  - `Status` (string): Bộ lọc query
  - `From` (string): Bộ lọc query
  - `To` (string): Bộ lọc query
  - `Page` (integer): Bộ lọc query
  - `PageSize` (integer): Bộ lọc query
- **Request Body:** Không có (Trống)
- **Response Shape:** `PagedResponse<IReadOnlyCollection<BookingResponseDto>>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Màn hình Lịch sử đặt sân (Booking History) / Quản lý đặt sân của cơ sở

### `[GET]` /api/courts/{courtId}/bookings
- **Mô tả:** Action GetCourtBookings
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Path Parameters:**
  - `courtId` (integer): Tham số đường dẫn
- **Query Parameters:**
  - `Status` (string): Bộ lọc query
  - `From` (string): Bộ lọc query
  - `To` (string): Bộ lọc query
  - `Page` (integer): Bộ lọc query
  - `PageSize` (integer): Bộ lọc query
- **Request Body:** Không có (Trống)
- **Response Shape:** `PagedResponse<IReadOnlyCollection<BookingResponseDto>>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Màn hình Lịch sử đặt sân (Booking History) / Quản lý đặt sân của cơ sở

### `[GET]` /api/courts/{courtId}/availability
- **Mô tả:** Action CheckAvailability
- **Xác thực:** Public (Không yêu cầu)
- **Path Parameters:**
  - `courtId` (integer): Tham số đường dẫn
- **Query Parameters:**
  - `StartAt` (string) (Required): Bộ lọc query
  - `EndAt` (string) (Required): Bộ lọc query
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<BookingAvailabilityResponseDto>`
- **FE Screen đề xuất:** Màn hình Chi tiết Sân / Cơ sở

### `[PATCH]` /api/Bookings/{id}/cancel
- **Mô tả:** Action Cancel
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** `UpdateBookingStatusRequestDto`
- **Response Shape:** `ApiResponse<BookingResponseDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Quản lý đặt sân (Booking Details / History)

### `[PATCH]` /api/Bookings/{id}/confirm
- **Mô tả:** Action Confirm
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** `UpdateBookingStatusRequestDto`
- **Response Shape:** `ApiResponse<BookingResponseDto>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Màn hình Quản lý đặt sân (Booking Details / History)

### `[PATCH]` /api/Bookings/{id}/reject
- **Mô tả:** Action Reject
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** `UpdateBookingStatusRequestDto`
- **Response Shape:** `ApiResponse<BookingResponseDto>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Màn hình Quản lý đặt sân (Booking Details / History)

### `[PATCH]` /api/Bookings/{id}/complete
- **Mô tả:** Action Complete
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** `UpdateBookingStatusRequestDto`
- **Response Shape:** `ApiResponse<BookingResponseDto>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Màn hình Quản lý đặt sân (Booking Details / History)

## Module: CourtOwners

### `[GET]` /api/court-owners
- **Mô tả:** Action GetAll
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Admin`)
- **Query Parameters:**
  - `status` (string): Bộ lọc query
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<List<CourtOwnerListItemDto>>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Admin - Quản lý Chủ sân

### `[GET]` /api/court-owners/{id}
- **Mô tả:** Action GetById
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Admin`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<CourtOwnerDetailDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Admin - Quản lý Chủ sân

### `[PATCH]` /api/court-owners/{id}/verification-status
- **Mô tả:** Action UpdateVerificationStatus
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Admin`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** `UpdateCourtOwnerVerificationStatusRequestDto`
- **Response Shape:** `ApiResponse<CourtOwnerDetailDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Admin - Quản lý Chủ sân

## Module: CourtSchedules

### `[GET]` /api/courts/{courtId}/schedules
- **Mô tả:** Action GetByCourt
- **Xác thực:** Public (Không yêu cầu)
- **Path Parameters:**
  - `courtId` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<List<CourtScheduleDto>>`
- **FE Screen đề xuất:** Màn hình Chủ sân - Quản lý Lịch đóng/mở sân

### `[POST]` /api/courts/{courtId}/schedules
- **Mô tả:** Action Create
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Path Parameters:**
  - `courtId` (integer): Tham số đường dẫn
- **Request Body:** `CreateCourtScheduleRequestDto`
- **Response Shape:** `ApiResponse<CourtScheduleDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Chủ sân - Quản lý Lịch đóng/mở sân

### `[DELETE]` /api/court-schedules/{id}
- **Mô tả:** Action Delete
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<object>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Chủ sân - Quản lý Lịch đóng/mở sân

## Module: Courts

### `[GET]` /api/venues/{venueId}/courts
- **Mô tả:** Action GetByVenue
- **Xác thực:** Public (Không yêu cầu)
- **Path Parameters:**
  - `venueId` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<List<CourtDto>>`
- **FE Screen đề xuất:** Màn hình Chi tiết Sân / Cơ sở

### `[POST]` /api/venues/{venueId}/courts
- **Mô tả:** Action Create
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Path Parameters:**
  - `venueId` (integer): Tham số đường dẫn
- **Request Body:** `CreateCourtRequestDto`
- **Response Shape:** `ApiResponse<CourtDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Chủ sân - Đăng ký Cơ sở kinh doanh mới

### `[GET]` /api/courts/{id}
- **Mô tả:** Action GetById
- **Xác thực:** Public (Không yêu cầu)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<CourtDto>`
- **FE Screen đề xuất:** Màn hình Chủ sân - Quản lý Sân nhỏ

### `[PUT]` /api/courts/{id}
- **Mô tả:** Action Update
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** `UpdateCourtRequestDto`
- **Response Shape:** `ApiResponse<CourtDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Chủ sân - Quản lý Sân nhỏ

### `[DELETE]` /api/courts/{id}
- **Mô tả:** Action Delete
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<bool>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Chủ sân - Quản lý Sân nhỏ

## Module: Matches

### `[GET]` /api/Matches
- **Mô tả:** Action Search
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Query Parameters:**
  - `SportId` (integer): Bộ lọc query
  - `SkillLevel` (integer): Bộ lọc query
  - `Location` (string): Bộ lọc query
  - `StartFrom` (string): Bộ lọc query
  - `StartTo` (string): Bộ lọc query
  - `IncludeFull` (boolean): Bộ lọc query
  - `PageIndex` (integer): Bộ lọc query
  - `PageSize` (integer): Bộ lọc query
- **Request Body:** Không có (Trống)
- **Response Shape:** `PagedResponse<List<MatchResponseDto>>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Giao lưu Kèo đấu (Matches / Social)

### `[POST]` /api/Matches
- **Mô tả:** Action Create
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Request Body:** `CreateMatchRequestDto`
- **Response Shape:** `ApiResponse<MatchResponseDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Giao lưu Kèo đấu (Matches / Social)

### `[GET]` /api/Matches/recommended
- **Mô tả:** Action GetRecommended
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Query Parameters:**
  - `limit` (integer): Bộ lọc query
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<List<MatchResponseDto>>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Giao lưu Kèo đấu (Matches / Social)

### `[GET]` /api/Matches/{id}
- **Mô tả:** Action GetById
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<MatchDetailResponseDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Giao lưu Kèo đấu (Matches / Social)

### `[PUT]` /api/Matches/{id}
- **Mô tả:** Action Update
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** `UpdateMatchRequestDto`
- **Response Shape:** `ApiResponse<MatchResponseDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Giao lưu Kèo đấu (Matches / Social)

### `[PATCH]` /api/Matches/{id}/cancel
- **Mô tả:** Action Cancel
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<MatchResponseDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Giao lưu Kèo đấu (Matches / Social)

### `[POST]` /api/Matches/{id}/join-requests
- **Mô tả:** Action RequestToJoin
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<MatchJoinRequestDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Giao lưu Kèo đấu (Matches / Social)

### `[GET]` /api/Matches/{id}/join-requests
- **Mô tả:** Action GetJoinRequests
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<List<MatchJoinRequestDto>>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Giao lưu Kèo đấu (Matches / Social)

### `[DELETE]` /api/Matches/{id}/join-requests/me
- **Mô tả:** Action CancelMyJoinRequest
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<MatchJoinRequestDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Giao lưu Kèo đấu (Matches / Social)

### `[PATCH]` /api/Matches/{id}/join-requests/{requestId}
- **Mô tả:** Action RespondToJoinRequest
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
  - `requestId` (integer): Tham số đường dẫn
- **Request Body:** `RespondJoinRequestDto`
- **Response Shape:** `ApiResponse<MatchJoinRequestDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Giao lưu Kèo đấu (Matches / Social)

### `[DELETE]` /api/Matches/{id}/participants/me
- **Mô tả:** Action Leave
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<MatchResponseDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Giao lưu Kèo đấu (Matches / Social)

### `[GET]` /api/Matches/{id}/candidates
- **Mô tả:** Action GetCandidates
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Query Parameters:**
  - `limit` (integer): Bộ lọc query
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<List<PlayerMatchCandidateDto>>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Giao lưu Kèo đấu (Matches / Social)

### `[POST]` /api/Matches/{id}/invitations
- **Mô tả:** Action Invite
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** `CreateMatchInvitationDto`
- **Response Shape:** `ApiResponse<MatchInvitationDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Giao lưu Kèo đấu (Matches / Social)

### `[GET]` /api/Matches/invitations/me
- **Mô tả:** Action GetMyInvitations
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<List<MatchInvitationDto>>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Giao lưu Kèo đấu (Matches / Social)

### `[PATCH]` /api/Matches/invitations/{invitationId}
- **Mô tả:** Action RespondToInvitation
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Path Parameters:**
  - `invitationId` (integer): Tham số đường dẫn
- **Request Body:** `RespondMatchInvitationDto`
- **Response Shape:** `ApiResponse<MatchInvitationDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Giao lưu Kèo đấu (Matches / Social)

## Module: Notifications

### `[GET]` /api/Notifications
- **Mô tả:** Action GetMyNotifications
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Query Parameters:**
  - `PageIndex` (integer): Bộ lọc query
  - `PageSize` (integer): Bộ lọc query
  - `IsRead` (boolean): Bộ lọc query
  - `Type` (string): Bộ lọc query
- **Request Body:** Không có (Trống)
- **Response Shape:** `PagedResponse<IReadOnlyCollection<NotificationDto>>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Trung tâm thông báo (Notification Center)

### `[GET]` /api/Notifications/unread-count
- **Mô tả:** Action GetUnreadCount
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<UnreadNotificationCountDto>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Trung tâm thông báo (Notification Center)

### `[PATCH]` /api/Notifications/{id}/read
- **Mô tả:** Action MarkAsRead
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<NotificationDto>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Trung tâm thông báo (Notification Center)

### `[PATCH]` /api/Notifications/read-all
- **Mô tả:** Action MarkAllAsRead
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<MarkAllNotificationsReadResponseDto>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Trung tâm thông báo (Notification Center)

### `[DELETE]` /api/Notifications/{id}
- **Mô tả:** Action Delete
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<object>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Trung tâm thông báo (Notification Center)

## Module: Payments

### `[POST]` /api/Payments/bookings/{bookingId}/payos
- **Mô tả:** Action CreatePayOsPaymentLink
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Path Parameters:**
  - `bookingId` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<CreatePayOsPaymentResponseDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Đặt sân - Thanh toán (Checkout)

### `[POST]` /api/Payments/bookings/{bookingId}/sync-payos
- **Mô tả:** Action SyncPayOsPayment
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Path Parameters:**
  - `bookingId` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<PaymentResponseDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Đặt sân - Thanh toán (Checkout)

### `[GET]` /api/Payments/bookings/{bookingId}
- **Mô tả:** Action GetBookingPayments
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Path Parameters:**
  - `bookingId` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<IReadOnlyCollection<PaymentResponseDto>>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Màn hình Lịch sử đặt sân (Booking History) / Quản lý đặt sân của cơ sở

### `[POST]` /api/Payments/payos/webhook
- **Mô tả:** Action HandlePayOsWebhook
- **Xác thực:** Public (Không yêu cầu)
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<PaymentResponseDto>`
- **FE Screen đề xuất:** Màn hình Đặt sân - Thanh toán (Checkout) / Lịch sử giao dịch

## Module: PricingRules

### `[GET]` /api/courts/{courtId}/pricing-rules
- **Mô tả:** Action GetByCourt
- **Xác thực:** Public (Không yêu cầu)
- **Path Parameters:**
  - `courtId` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<List<PricingRuleDto>>`
- **FE Screen đề xuất:** Màn hình Chủ sân - Thiết lập Bảng giá

### `[POST]` /api/courts/{courtId}/pricing-rules
- **Mô tả:** Action Create
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Path Parameters:**
  - `courtId` (integer): Tham số đường dẫn
- **Request Body:** `CreatePricingRuleRequestDto`
- **Response Shape:** `ApiResponse<PricingRuleDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Chủ sân - Thiết lập Bảng giá

### `[PUT]` /api/pricing-rules/{id}
- **Mô tả:** Action Update
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** `UpdatePricingRuleRequestDto`
- **Response Shape:** `ApiResponse<PricingRuleDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Chủ sân - Thiết lập Bảng giá

### `[DELETE]` /api/pricing-rules/{id}
- **Mô tả:** Action Delete
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<object>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Chủ sân - Thiết lập Bảng giá

## Module: Reviews

### `[POST]` /api/Reviews
- **Mô tả:** Action Create
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Request Body:** `CreateReviewRequestDto`
- **Response Shape:** `ApiResponse<ReviewResponseDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Đánh giá & Phản hồi (Reviews)

### `[GET]` /api/venues/{venueId}/reviews
- **Mô tả:** Action GetVenueReviews
- **Xác thực:** Public (Không yêu cầu)
- **Path Parameters:**
  - `venueId` (integer): Tham số đường dẫn
- **Query Parameters:**
  - `page` (integer): Bộ lọc query
  - `pageSize` (integer): Bộ lọc query
- **Request Body:** Không có (Trống)
- **Response Shape:** `PagedResponse<IReadOnlyCollection<ReviewResponseDto>>`
- **FE Screen đề xuất:** Màn hình Chi tiết Sân / Cơ sở

### `[GET]` /api/courts/{courtId}/reviews
- **Mô tả:** Action GetCourtReviews
- **Xác thực:** Public (Không yêu cầu)
- **Path Parameters:**
  - `courtId` (integer): Tham số đường dẫn
- **Query Parameters:**
  - `page` (integer): Bộ lọc query
  - `pageSize` (integer): Bộ lọc query
- **Request Body:** Không có (Trống)
- **Response Shape:** `PagedResponse<IReadOnlyCollection<ReviewResponseDto>>`
- **FE Screen đề xuất:** Màn hình Đánh giá & Phản hồi (Reviews)

### `[PUT]` /api/Reviews/{id}
- **Mô tả:** Action Update
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** `UpdateReviewRequestDto`
- **Response Shape:** `ApiResponse<ReviewResponseDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Đánh giá & Phản hồi (Reviews)

### `[DELETE]` /api/Reviews/{id}
- **Mô tả:** Action Delete
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<object>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Màn hình Đánh giá & Phản hồi (Reviews)

### `[POST]` /api/Reviews/{id}/report
- **Mô tả:** Action Report
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<object>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Màn hình Đánh giá & Phản hồi (Reviews)

### `[PUT]` /api/admin/reviews/{id}/moderate
- **Mô tả:** Action Moderate
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Admin`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Query Parameters:**
  - `status` (string): Bộ lọc query
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<ReviewResponseDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Đánh giá & Phản hồi (Reviews)

### `[POST]` /api/Reviews/{id}/images
- **Mô tả:** Action AddImage
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** `AddReviewImageRequestDto`
- **Response Shape:** `ApiResponse<ReviewImageDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Đánh giá & Phản hồi (Reviews)

### `[DELETE]` /api/Reviews/{id}/images/{imageId}
- **Mô tả:** Action DeleteImage
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
  - `imageId` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<object>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Đánh giá & Phản hồi (Reviews)

### `[GET]` /api/venues/{venueId}/rating-stats
- **Mô tả:** Action GetVenueStats
- **Xác thực:** Public (Không yêu cầu)
- **Path Parameters:**
  - `venueId` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<ReviewStatsDto>`
- **FE Screen đề xuất:** Màn hình Chi tiết Sân / Cơ sở

### `[GET]` /api/courts/{courtId}/rating-stats
- **Mô tả:** Action GetCourtStats
- **Xác thực:** Public (Không yêu cầu)
- **Path Parameters:**
  - `courtId` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<ReviewStatsDto>`
- **FE Screen đề xuất:** Màn hình Chi tiết Sân / Cơ sở

### `[GET]` /api/Reviews/my
- **Mô tả:** Action GetMyReviews
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Player`)
- **Query Parameters:**
  - `page` (integer): Bộ lọc query
  - `pageSize` (integer): Bộ lọc query
- **Request Body:** Không có (Trống)
- **Response Shape:** `PagedResponse<IReadOnlyCollection<ReviewResponseDto>>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Đánh giá & Phản hồi (Reviews)

## Module: Sports

### `[GET]` /api/Sports
- **Mô tả:** Action GetAll
- **Xác thực:** Public (Không yêu cầu)
- **Query Parameters:**
  - `isActive` (boolean): Bộ lọc query
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<List<SportDto>>`
- **FE Screen đề xuất:** Màn hình Admin - Quản lý môn thể thao / Màn hình cá nhân (chọn môn yêu thích)

### `[POST]` /api/Sports
- **Mô tả:** Action Create
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Admin`)
- **Request Body:** `CreateSportRequestDto`
- **Response Shape:** `ApiResponse<SportDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Admin - Quản lý môn thể thao / Màn hình cá nhân (chọn môn yêu thích)

### `[GET]` /api/Sports/{id}
- **Mô tả:** Action GetById
- **Xác thực:** Public (Không yêu cầu)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<SportDto>`
- **FE Screen đề xuất:** Màn hình Admin - Quản lý môn thể thao / Màn hình cá nhân (chọn môn yêu thích)

### `[PUT]` /api/Sports/{id}
- **Mô tả:** Action Update
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Admin`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** `UpdateSportRequestDto`
- **Response Shape:** `ApiResponse<SportDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Admin - Quản lý môn thể thao / Màn hình cá nhân (chọn môn yêu thích)

### `[PATCH]` /api/Sports/{id}/toggle-active
- **Mô tả:** Action ToggleActive
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Admin`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<SportDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Admin - Quản lý môn thể thao / Màn hình cá nhân (chọn môn yêu thích)

## Module: Users

### `[GET]` /api/Users/me
- **Mô tả:** Action GetMe
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<UserProfileResponseDto>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Màn hình Thông tin cá nhân (Profile)

### `[PUT]` /api/Users/me
- **Mô tả:** Action UpdateMe
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Request Body:** `UpdateUserProfileRequestDto`
- **Response Shape:** `ApiResponse<UserProfileResponseDto>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Màn hình Thông tin cá nhân (Profile)

### `[GET]` /api/Users/me/sports
- **Mô tả:** Action GetMySports
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<List<PlayerSportResponseDto>>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Màn hình Admin - Quản lý môn thể thao / Màn hình cá nhân (chọn môn yêu thích)

### `[POST]` /api/Users/me/sports
- **Mô tả:** Action AddMySport
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Request Body:** `AddPlayerSportRequestDto`
- **Response Shape:** `ApiResponse<PlayerSportResponseDto>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Màn hình Admin - Quản lý môn thể thao / Màn hình cá nhân (chọn môn yêu thích)

### `[PUT]` /api/Users/me/sports/{sportId}
- **Mô tả:** Action UpdateMySport
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Path Parameters:**
  - `sportId` (integer): Tham số đường dẫn
- **Request Body:** `UpdatePlayerSportRequestDto`
- **Response Shape:** `ApiResponse<PlayerSportResponseDto>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Màn hình Admin - Quản lý môn thể thao / Màn hình cá nhân (chọn môn yêu thích)

### `[DELETE]` /api/Users/me/sports/{sportId}
- **Mô tả:** Action RemoveMySport
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Path Parameters:**
  - `sportId` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<PlayerSportResponseDto>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Màn hình Admin - Quản lý môn thể thao / Màn hình cá nhân (chọn môn yêu thích)

## Module: VenueStaffs

### `[POST]` /api/venues/{venueId}/staff
- **Mô tả:** Action AddStaff
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Path Parameters:**
  - `venueId` (integer): Tham số đường dẫn
- **Request Body:** `AddVenueStaffRequestDto`
- **Response Shape:** `ApiResponse<VenueStaffResponseDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Chủ sân - Đăng ký Cơ sở kinh doanh mới

### `[GET]` /api/venues/{venueId}/staff
- **Mô tả:** Action GetStaff
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Path Parameters:**
  - `venueId` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<IReadOnlyCollection<VenueStaffResponseDto>>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Màn hình Chi tiết Sân / Cơ sở

### `[DELETE]` /api/venues/{venueId}/staff/{staffId}
- **Mô tả:** Action RemoveStaff
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Path Parameters:**
  - `venueId` (integer): Tham số đường dẫn
  - `staffId` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<object>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Chủ sân - Quản lý & Thiết lập Cơ sở

## Module: Venues

### `[GET]` /api/Venues
- **Mô tả:** Action GetAll
- **Xác thực:** Public (Không yêu cầu)
- **Query Parameters:**
  - `Keyword` (string): Bộ lọc query
  - `SportId` (integer): Bộ lọc query
  - `IsOpenNow` (boolean): Bộ lọc query
  - `PageIndex` (integer): Bộ lọc query
  - `PageSize` (integer): Bộ lọc query
- **Request Body:** Không có (Trống)
- **Response Shape:** `PagedResponse<IReadOnlyCollection<VenueResponseDto>>`
- **FE Screen đề xuất:** Màn hình Tìm kiếm Sân / Trang chủ (Home)

### `[POST]` /api/Venues
- **Mô tả:** Action Create
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Request Body:** `CreateVenueRequestDto`
- **Response Shape:** `ApiResponse<VenueResponseDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Chủ sân - Đăng ký Cơ sở kinh doanh mới

### `[GET]` /api/Venues/{id}
- **Mô tả:** Action GetById
- **Xác thực:** Public (Không yêu cầu)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<VenueResponseDto>`
- **FE Screen đề xuất:** Màn hình Chi tiết Sân / Cơ sở

### `[PUT]` /api/Venues/{id}
- **Mô tả:** Action Update
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** `UpdateVenueRequestDto`
- **Response Shape:** `ApiResponse<VenueResponseDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Chủ sân - Quản lý & Thiết lập Cơ sở

### `[DELETE]` /api/Venues/{id}
- **Mô tả:** Action Delete
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<object>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Chủ sân - Quản lý & Thiết lập Cơ sở

### `[GET]` /api/Venues/admin
- **Mô tả:** Action GetAllForAdmin
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Admin`)
- **Query Parameters:**
  - `status` (string): Bộ lọc query
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<IReadOnlyCollection<VenueResponseDto>>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Admin - Quản lý & Phê duyệt Cơ sở

### `[GET]` /api/Venues/admin/{id}
- **Mô tả:** Action GetByIdForAdmin
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Admin`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<VenueResponseDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Admin - Quản lý & Phê duyệt Cơ sở

### `[PATCH]` /api/Venues/{id}/status
- **Mô tả:** Action UpdateStatus
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Admin`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** `UpdateVenueStatusRequestDto`
- **Response Shape:** `ApiResponse<VenueResponseDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Admin - Quản lý & Phê duyệt Cơ sở

### `[GET]` /api/Venues/my
- **Mô tả:** Action GetMy
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<IReadOnlyCollection<VenueResponseDto>>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Dashboard Chủ sân - Quản lý Cơ sở

### `[POST]` /api/Venues/{id}/images
- **Mô tả:** Action AddImage
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** `AddVenueImageRequestDto`
- **Response Shape:** `ApiResponse<VenueImageDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Chủ sân - Đăng ký Cơ sở kinh doanh mới

### `[DELETE]` /api/Venues/{id}/images/{imageId}
- **Mô tả:** Action DeleteImage
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
  - `imageId` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<object>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Chủ sân - Quản lý & Thiết lập Cơ sở

### `[PATCH]` /api/Venues/{id}/images/{imageId}/set-cover
- **Mô tả:** Action SetCoverImage
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
  - `imageId` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<VenueImageDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Chủ sân - Quản lý tiện ích / Ảnh / Giờ hoạt động / Nhân viên

### `[POST]` /api/Venues/{id}/amenities/{amenityId}
- **Mô tả:** Action AddAmenity
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
  - `amenityId` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<PlayCourt.Application.DTOs.Amenities.AmenityDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Chủ sân - Đăng ký Cơ sở kinh doanh mới

### `[DELETE]` /api/Venues/{id}/amenities/{amenityId}
- **Mô tả:** Action RemoveAmenity
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
  - `amenityId` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<object>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Chủ sân - Quản lý & Thiết lập Cơ sở

### `[GET]` /api/Venues/{id}/opening-hours
- **Mô tả:** Action GetOpeningHours
- **Xác thực:** Public (Không yêu cầu)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<IReadOnlyCollection<VenueOpeningHourDto>>`
- **FE Screen đề xuất:** Màn hình Chi tiết Sân / Cơ sở

### `[PUT]` /api/Venues/{id}/opening-hours
- **Mô tả:** Action UpdateOpeningHours
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** `UpdateOpeningHoursRequestDto`
- **Response Shape:** `ApiResponse<IReadOnlyCollection<VenueOpeningHourDto>>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Chủ sân - Quản lý & Thiết lập Cơ sở

### `[GET]` /api/Venues/my/{id}
- **Mô tả:** Action GetMyById
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<VenueResponseDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Dashboard Chủ sân - Quản lý Cơ sở

### `[POST]` /api/Venues/{id}/favorites
- **Mô tả:** Action AddFavorite
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<FavoriteVenueResponseDto>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Màn hình Chủ sân - Đăng ký Cơ sở kinh doanh mới

### `[DELETE]` /api/Venues/{id}/favorites
- **Mô tả:** Action RemoveFavorite
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Path Parameters:**
  - `id` (integer): Tham số đường dẫn
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<object>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Màn hình Chủ sân - Quản lý & Thiết lập Cơ sở

### `[GET]` /api/Venues/favorites/my
- **Mô tả:** Action GetMyFavorites
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `Authenticated`)
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<IReadOnlyCollection<FavoriteVenueResponseDto>>`
- **Error Cases:** 401 (Unauthorized)
- **FE Screen đề xuất:** Màn hình Sân yêu thích (Favorites)

### `[GET]` /api/Venues/stats
- **Mô tả:** Action GetOwnerStats
- **Xác thực:** Yêu cầu đăng nhập (Role/Policy: `CourtOwner`)
- **Request Body:** Không có (Trống)
- **Response Shape:** `ApiResponse<VenueStatsResponseDto>`
- **Error Cases:** 401 (Unauthorized), 403 (Forbidden)
- **FE Screen đề xuất:** Màn hình Dashboard Chủ sân - Quản lý Cơ sở

## 3. Chi tiết cấu trúc Request/Response DTO (Data Transfer Objects)

### AddPlayerSportRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `sportId` | `integer` | ❌ No | ❌ No | format: int32 |
| `skillLevel` | `integer` | ❌ No | ❌ No | format: int32 |

### AddReviewImageRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `imageUrl` | `string` | ✅ Yes | ❌ No | minLength: 1, format: uri |
| `displayOrder` | `integer` | ❌ No | ❌ No | min: 0, max: 100, format: int32 |

### AddVenueImageRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `imageUrl` | `string` | ✅ Yes | ❌ No | minLength: 1, format: uri |
| `isCover` | `boolean` | ❌ No | ❌ No | - |

### AddVenueStaffRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `email` | `string` | ✅ Yes | ❌ No | minLength: 1, format: email |
| `role` | `VenueStaffRole` | ✅ Yes | ❌ No | - |

### ChangePasswordRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `currentPassword` | `string` | ❌ No | ✅ Yes | - |
| `newPassword` | `string` | ❌ No | ✅ Yes | - |

### CreateAmenityRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `name` | `string` | ✅ Yes | ❌ No | minLength: 1, maxLength: 100 |

### CreateBookingRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `courtId` | `integer` | ✅ Yes | ❌ No | format: int32 |
| `startAt` | `string` | ✅ Yes | ❌ No | format: date-time |
| `endAt` | `string` | ✅ Yes | ❌ No | format: date-time |
| `note` | `string` | ❌ No | ✅ Yes | maxLength: 500 |

### CreateCourtRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `sportId` | `integer` | ✅ Yes | ❌ No | format: int32 |
| `name` | `string` | ✅ Yes | ❌ No | minLength: 1, maxLength: 100 |
| `indoor` | `boolean` | ✅ Yes | ❌ No | - |

### CreateCourtScheduleRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `startAt` | `string` | ✅ Yes | ❌ No | format: date-time |
| `endAt` | `string` | ✅ Yes | ❌ No | format: date-time |
| `reason` | `string` | ❌ No | ✅ Yes | maxLength: 255 |

### CreateMatchInvitationDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `inviteeProfileId` | `integer` | ❌ No | ❌ No | min: 1, max: 2147483647, format: int32 |
| `message` | `string` | ❌ No | ✅ Yes | maxLength: 500 |

### CreateMatchRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `sportId` | `integer` | ❌ No | ❌ No | min: 1, max: 2147483647, format: int32 |
| `courtId` | `integer` | ❌ No | ✅ Yes | min: 1, max: 2147483647, format: int32 |
| `locationDescription` | `string` | ❌ No | ✅ Yes | maxLength: 500 |
| `startAt` | `string` | ❌ No | ❌ No | format: date-time |
| `endAt` | `string` | ❌ No | ❌ No | format: date-time |
| `requiredSkillLevelMin` | `integer` | ❌ No | ✅ Yes | min: 0, max: 2, format: int32 |
| `requiredSkillLevelMax` | `integer` | ❌ No | ✅ Yes | min: 0, max: 2, format: int32 |
| `maxParticipants` | `integer` | ❌ No | ❌ No | min: 2, max: 100, format: int32 |
| `costDescription` | `string` | ❌ No | ✅ Yes | maxLength: 500 |
| `description` | `string` | ❌ No | ✅ Yes | maxLength: 2000 |

### CreatePricingRuleRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `dayOfWeek` | `integer` | ✅ Yes | ❌ No | min: 1, max: 7, format: int32 |
| `startTime` | `string` | ✅ Yes | ❌ No | minLength: 1 |
| `endTime` | `string` | ✅ Yes | ❌ No | minLength: 1 |
| `pricePerHour` | `number` | ✅ Yes | ❌ No | min: 0.01, format: double |
| `effectiveFrom` | `string` | ✅ Yes | ❌ No | format: date-time |
| `effectiveTo` | `string` | ❌ No | ✅ Yes | format: date-time |

### CreateReviewRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `bookingId` | `integer` | ✅ Yes | ❌ No | format: int32 |
| `rating` | `number` | ✅ Yes | ❌ No | min: 1, max: 5, format: double |
| `reviewText` | `string` | ❌ No | ✅ Yes | - |
| `imageUrls` | `Array<string>` | ❌ No | ✅ Yes | - |

### CreateSportRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `code` | `string` | ✅ Yes | ❌ No | minLength: 1, maxLength: 50 |
| `name` | `string` | ✅ Yes | ❌ No | minLength: 1, maxLength: 100 |
| `description` | `string` | ❌ No | ✅ Yes | - |
| `playerCount` | `integer` | ❌ No | ✅ Yes | format: int32 |

### CreateVenueRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `name` | `string` | ✅ Yes | ❌ No | minLength: 1, maxLength: 255 |
| `description` | `string` | ❌ No | ✅ Yes | - |
| `address` | `string` | ✅ Yes | ❌ No | minLength: 1, maxLength: 500 |
| `latitude` | `number` | ❌ No | ✅ Yes | min: -90, max: 90, format: double |
| `longitude` | `number` | ❌ No | ✅ Yes | min: -180, max: 180, format: double |
| `phone` | `string` | ❌ No | ✅ Yes | maxLength: 20 |
| `openTime` | `string` | ❌ No | ✅ Yes | format: date-span |
| `closeTime` | `string` | ❌ No | ✅ Yes | format: date-span |

### ForgotPasswordRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `email` | `string` | ❌ No | ✅ Yes | - |

### LoginRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `identifier` | `string` | ✅ Yes | ❌ No | minLength: 1 |
| `password` | `string` | ✅ Yes | ❌ No | minLength: 1 |

### LogoutRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `refreshToken` | `string` | ✅ Yes | ❌ No | minLength: 1 |

### OpeningHourRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `dayOfWeek` | `integer` | ❌ No | ❌ No | format: int32 |
| `openTime` | `string` | ❌ No | ✅ Yes | format: date-span |
| `closeTime` | `string` | ❌ No | ✅ Yes | format: date-span |
| `isClosed` | `boolean` | ❌ No | ❌ No | - |

### RefreshTokenRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `refreshToken` | `string` | ✅ Yes | ❌ No | minLength: 1 |

### RegisterRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `fullName` | `string` | ✅ Yes | ❌ No | minLength: 1 |
| `email` | `string` | ✅ Yes | ❌ No | minLength: 1, format: email |
| `phoneNumber` | `string` | ✅ Yes | ❌ No | minLength: 1 |
| `password` | `string` | ✅ Yes | ❌ No | minLength: 6 |
| `role` | `string` | ✅ Yes | ❌ No | minLength: 1 |
| `businessName` | `string` | ❌ No | ✅ Yes | - |

### ResendVerifyEmailRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `email` | `string` | ✅ Yes | ❌ No | minLength: 1, format: email |

### ResetPasswordRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `email` | `string` | ❌ No | ✅ Yes | - |
| `otp` | `string` | ❌ No | ✅ Yes | - |
| `newPassword` | `string` | ❌ No | ✅ Yes | - |

### RespondJoinRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `status` | `string` | ✅ Yes | ❌ No | minLength: 1 |

### RespondMatchInvitationDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `status` | `string` | ✅ Yes | ❌ No | minLength: 1 |

### UpdateBookingStatusRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `reason` | `string` | ❌ No | ✅ Yes | maxLength: 500 |

### UpdateCourtOwnerVerificationStatusRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `verificationStatus` | `integer` | ❌ No | ❌ No | format: int32 |
| `rejectionReason` | `string` | ❌ No | ✅ Yes | maxLength: 500 |

### UpdateCourtRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `sportId` | `integer` | ✅ Yes | ❌ No | format: int32 |
| `name` | `string` | ✅ Yes | ❌ No | minLength: 1, maxLength: 100 |
| `indoor` | `boolean` | ✅ Yes | ❌ No | - |
| `status` | `CourtStatus` | ✅ Yes | ❌ No | - |

### UpdateMatchRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `sportId` | `integer` | ❌ No | ❌ No | min: 1, max: 2147483647, format: int32 |
| `courtId` | `integer` | ❌ No | ✅ Yes | min: 1, max: 2147483647, format: int32 |
| `locationDescription` | `string` | ❌ No | ✅ Yes | maxLength: 500 |
| `startAt` | `string` | ❌ No | ❌ No | format: date-time |
| `endAt` | `string` | ❌ No | ❌ No | format: date-time |
| `requiredSkillLevelMin` | `integer` | ❌ No | ✅ Yes | min: 0, max: 2, format: int32 |
| `requiredSkillLevelMax` | `integer` | ❌ No | ✅ Yes | min: 0, max: 2, format: int32 |
| `maxParticipants` | `integer` | ❌ No | ❌ No | min: 2, max: 100, format: int32 |
| `costDescription` | `string` | ❌ No | ✅ Yes | maxLength: 500 |
| `description` | `string` | ❌ No | ✅ Yes | maxLength: 2000 |

### UpdateOpeningHoursRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `openingHours` | `Array<OpeningHourRequestDto>` | ✅ Yes | ❌ No | - |

### UpdatePlayerSportRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `skillLevel` | `integer` | ❌ No | ❌ No | format: int32 |

### UpdatePricingRuleRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `dayOfWeek` | `integer` | ✅ Yes | ❌ No | min: 1, max: 7, format: int32 |
| `startTime` | `string` | ✅ Yes | ❌ No | minLength: 1 |
| `endTime` | `string` | ✅ Yes | ❌ No | minLength: 1 |
| `pricePerHour` | `number` | ✅ Yes | ❌ No | min: 0.01, format: double |
| `effectiveFrom` | `string` | ✅ Yes | ❌ No | format: date-time |
| `effectiveTo` | `string` | ❌ No | ✅ Yes | format: date-time |

### UpdateReviewRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `rating` | `number` | ✅ Yes | ❌ No | min: 1, max: 5, format: double |
| `reviewText` | `string` | ❌ No | ✅ Yes | - |

### UpdateSportRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `code` | `string` | ✅ Yes | ❌ No | minLength: 1, maxLength: 50 |
| `name` | `string` | ✅ Yes | ❌ No | minLength: 1, maxLength: 100 |
| `description` | `string` | ❌ No | ✅ Yes | - |
| `playerCount` | `integer` | ❌ No | ✅ Yes | format: int32 |
| `isActive` | `boolean` | ❌ No | ❌ No | - |

### UpdateUserProfileRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `fullName` | `string` | ❌ No | ✅ Yes | - |
| `avatarUrl` | `string` | ❌ No | ✅ Yes | - |
| `dateOfBirth` | `string` | ❌ No | ✅ Yes | format: date-time |
| `gender` | `integer` | ❌ No | ✅ Yes | format: int32 |
| `address` | `string` | ❌ No | ✅ Yes | - |
| `city` | `string` | ❌ No | ✅ Yes | - |
| `country` | `string` | ❌ No | ✅ Yes | - |

### UpdateVenueRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `name` | `string` | ✅ Yes | ❌ No | minLength: 1, maxLength: 255 |
| `description` | `string` | ❌ No | ✅ Yes | - |
| `address` | `string` | ✅ Yes | ❌ No | minLength: 1, maxLength: 500 |
| `latitude` | `number` | ❌ No | ✅ Yes | min: -90, max: 90, format: double |
| `longitude` | `number` | ❌ No | ✅ Yes | min: -180, max: 180, format: double |
| `phone` | `string` | ❌ No | ✅ Yes | maxLength: 20 |
| `openTime` | `string` | ❌ No | ✅ Yes | format: date-span |
| `closeTime` | `string` | ❌ No | ✅ Yes | format: date-span |

### UpdateVenueStatusRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `status` | `VenueStatus` | ✅ Yes | ❌ No | - |

### VerifyEmailRequestDto
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `email` | `string` | ✅ Yes | ❌ No | minLength: 1, format: email |
| `otp` | `string` | ✅ Yes | ❌ No | minLength: 6, maxLength: 6 |

### WeatherForecast
| Field | Type | Required | Nullable | Validations |
| :--- | :--- | :--- | :--- | :--- |
| `date` | `string` | ❌ No | ❌ No | format: date |
| `temperatureC` | `integer` | ❌ No | ❌ No | format: int32 |
| `temperatureF` | `integer` | ❌ No | ❌ No | format: int32 |
| `summary` | `string` | ❌ No | ✅ Yes | - |

