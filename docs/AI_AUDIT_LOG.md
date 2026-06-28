# AI Audit Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Building Cross-Platform Back-End Application With .NET |
| Mã môn học | PRN232 |
| Lớp | SE18D05 |
| Học kỳ | SU26 |
| Project | PlayCourt API - Sport Court Booking System |
| Nhóm | Group 01 |
| Danh sách MSSV | DE180519, DE180405, DE180313, DE180310, DE190946 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày bắt đầu | 11/05/2026 |
| Ngày hoàn thành | 05/07/2026 |

---

## 2. Công cụ AI đã sử dụng

Đánh dấu các công cụ AI đã sử dụng trong quá trình thực hiện bài tập/project.

- [x] ChatGPT
- [x] Gemini
- [x] Claude
- [x] GitHub Copilot
- [x] Cursor
- [x] Antigravity
- [x] Codex
- [x] Stitch
- [ ] Perplexity
- [ ] Microsoft Copilot
- [ ] Công cụ khác:............................

---

## 3. Mục tiêu sử dụng AI

Mô tả ngắn gọn sinh viên/nhóm đã sử dụng AI để hỗ trợ những công việc nào.

Ví dụ:

- Phân tích yêu cầu bài toán
- Gợi ý ý tưởng giải pháp
- Thiết kế database
- Thiết kế giao diện
- Viết code mẫu
- Debug lỗi
- Tối ưu code
- Viết test case
- Kiểm tra bảo mật
- Viết báo cáo
- Chuẩn bị slide thuyết trình
- Tìm hiểu công nghệ mới

### Mô tả mục tiêu sử dụng AI

```text
Nhóm dự kiến sử dụng AI để hỗ trợ phân tích yêu cầu, gợi ý thiết kế hệ thống, hỗ trợ viết code, debug lỗi, viết test case, review code và cập nhật tài liệu.

Các prompt, kết quả AI, phần đã áp dụng và phần nhóm tự chỉnh sửa sẽ được ghi chi tiết trong quá trình thực hiện project.
```

## 4. Nhật ký sử dụng AI chi tiết

> Mỗi lần sử dụng AI cho một phần quan trọng của bài tập/project, sinh viên cần ghi lại theo mẫu bên dưới.  
> Sinh viên/nhóm có thể nhân bản mẫu “Lần sử dụng AI” nhiều lần tùy theo số lần sử dụng AI thực tế.

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 14/05/2026 |
| Công cụ AI | Codex |
| Mục đích sử dụng | Cập nhật tài liệu ban đầu cho project |
| Phần việc liên quan | Report / Documentation |
| Mức độ sử dụng | Hỗ trợ một phần |

#### 4.1. Prompt đã sử dụng

```text
Điền thông tin cơ bản cho tài liệu audit của project PlayCourt, gồm tên môn học, lớp, nhóm, thời gian làm project và danh sách công cụ AI đã sử dụng.
```

#### 4.2. Kết quả AI gợi ý

Tóm tắt nội dung AI đã trả lời hoặc gợi ý.

```text
AI gợi ý cách trình bày thông tin project, danh sách công cụ AI và các mục cần ghi log trong quá trình làm bài.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

Mô tả rõ phần nào được sử dụng lại từ gợi ý của AI.

```text
Nhóm sử dụng gợi ý để hoàn thiện phần thông tin chung, mục tiêu sử dụng AI và cấu trúc ghi log.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

Mô tả sinh viên/nhóm đã thay đổi, kiểm tra, sửa lỗi hoặc cải tiến gì so với gợi ý ban đầu của AI.

```text
Nhóm tự kiểm tra lại tên môn học, mã lớp, danh sách MSSV và chỉnh nội dung cho phù hợp với project.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit |  |
| File liên quan | `docs/AI_AUDIT_LOG.md`, `docs/PROMPTS.md`, `docs/CHANGELOG.md` |
| Screenshot |  |
| Kết quả chạy/test |  |
| Link video demo |  |
| Ghi chú khác |  |

#### 4.6. Nhận xét cá nhân/nhóm

Sinh viên/nhóm học được gì sau lần sử dụng AI này?

```text
AI giúp nhóm chuẩn bị tài liệu nhanh hơn, nhưng thông tin cuối cùng vẫn cần nhóm kiểm tra lại.
```

---

### Lần sử dụng AI số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 21/05/2026 |
| Công cụ AI | Codex |
| Mục đích sử dụng | Hướng dẫn tạo domain model và cấu hình DbContext |
| Phần việc liên quan | Database / Backend |
| Mức độ sử dụng | Hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
Hãy hướng dẫn tôi tạo các entity model và cấu hình DbContext cho hệ thống đặt sân thể thao PlayCourt bằng ASP.NET Core và EF Core. Cần có DbSet, quan hệ khóa ngoại, index, check constraint, migration và cấu hình connection string an toàn cho team.
```

#### 4.2. Kết quả AI gợi ý

```text
AI gợi ý tạo các entity trong Domain, thêm navigation properties, dùng enum cho các trường trạng thái, cấu hình PlayCourtDbContext với DbSet, relationship, index, soft-delete query filter, check constraint và migration.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Nhóm sử dụng gợi ý để hoàn thiện entity model, enum, DbContext configuration và migration cho database PlayCourt.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Nhóm kiểm tra lại mapping EF Core, sửa duplicate index, bổ sung soft-delete filter, kiểm tra connection string không chứa thông tin cá nhân và chạy build/test để xác nhận.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Sẽ cập nhật sau khi commit |
| File liên quan | `PlayCourt.Domain/Entities/`, `PlayCourt.Domain/Enums/`, `PlayCourt.Infrastructure/Data/PlayCourtDbContext.cs`, `PlayCourt.Infrastructure/Data/Migrations/` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet build PlayCourt.sln --no-restore`, `dotnet test PlayCourt.sln --no-build` |
| Link video demo |  |
| Ghi chú khác | AI chỉ hỗ trợ hướng dẫn và review, nhóm kiểm tra lại trước khi commit |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI giúp nhóm định hướng nhanh cấu trúc model và DbContext, nhưng các ràng buộc database, migration và kết quả build/test vẫn cần tự kiểm tra kỹ trước khi sử dụng.
```

---

### Lần sử dụng AI số 3

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 21/05/2026 |
| Công cụ AI | Codex |
| Mục đích sử dụng | Thiết lập application layer, dependency injection và response/error handling nền tảng |
| Phần việc liên quan | Design / Backend / Testing / Debug |
| Mức độ sử dụng | Hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
Hãy giúp tôi setup application layer cho PlayCourt API theo hướng clean architecture nhẹ: Program.cs gọn, mỗi layer có DependencyInjection riêng, API có middleware xử lý exception thống nhất, Application có common ApiResponse, Infrastructure đăng ký DbContext và service implementation. Hãy review để code build được, không thay đổi docs/README trong commit code.
```

#### 4.2. Kết quả AI gợi ý

```text
AI gợi ý tách đăng ký service khỏi Program.cs sang các extension method theo API, Application và Infrastructure; thêm ExceptionHandlingMiddleware để trả lỗi dạng ApiResponse; tạo interface/service placeholder để team có điểm mở rộng cho các feature sau; đồng thời loại bỏ các Class1.cs mặc định.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Nhóm áp dụng cấu trúc DependencyInjection cho từng layer, thêm middleware xử lý exception, thêm ApiResponse dùng chung, thêm IService/Service placeholder và cập nhật project reference/package cần thiết.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Nhóm tự kiểm tra lại scope thay đổi so với nhánh dev, giữ commit code tách riêng với docs, chạy dotnet format, dotnet build và dotnet test. File test không phù hợp với contract hiện tại đã được loại khỏi scope trước khi verify lại.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Sẽ cập nhật sau khi commit |
| File liên quan | `PlayCourt.API/Program.cs`, `PlayCourt.API/DependencyInjection.cs`, `PlayCourt.API/Middlewares/ExceptionHandlingMiddleware.cs`, `PlayCourt.Application/Common/Responses/ApiResponse.cs`, `PlayCourt.Application/DependencyInjection.cs`, `PlayCourt.Application/Interfaces/IService.cs`, `PlayCourt.Infrastructure/DependencyInjection.cs`, `PlayCourt.Infrastructure/Services/Service.cs` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet format PlayCourt.sln --verify-no-changes --no-restore` passed; `dotnet build PlayCourt.sln` passed; `dotnet test PlayCourt.sln` passed 3/3 tests |
| Link video demo |  |
| Ghi chú khác | Thay đổi tập trung vào setup layer và backend foundation |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI giúp nhóm định hình nhanh cách gom DI và pipeline theo layer, nhưng nhóm vẫn phải review lại dependency direction, package reference và kết quả build/test trước khi commit.
```

---

### Lần sử dụng AI số 4

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 03/06/2026 |
| Công cụ AI | Codex |
| Mục đích sử dụng | Triển khai Register API |
| Phần việc liên quan | Backend / Testing |
| Mức độ sử dụng | Hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
Implement API đăng ký tài khoản cho PlayCourt Backend theo Clean Architecture. Dùng ApiResponse<T>, tạo DTO, IAuthService, AuthService, AuthController, hash password bằng BCrypt, tạo User, UserProfile và CourtOwnerProfile nếu role Owner.
```

#### 4.2. Kết quả AI gợi ý

```text
AI gợi ý thêm RegisterRequestDto, RegisterResponseDto, IAuthService, AuthService, AuthController, đăng ký DI và kiểm tra build/test.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Nhóm áp dụng để tạo endpoint POST /api/auth/register, kiểm tra role Player/Owner, hash password và trả response theo ApiResponse<T>.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Nhóm yêu cầu không thêm SQLite vì project dùng SQL Server, sau đó kiểm tra lại package và chạy build/test.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Sẽ cập nhật sau khi commit |
| File liên quan | `PlayCourt.API/Controllers/AuthController.cs`, `PlayCourt.Infrastructure/Services/AuthService.cs`, `PlayCourt.Application/DTOs/Auth/` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet build PlayCourt.sln`, `dotnet test PlayCourt.sln --no-build` |
| Link video demo |  |
| Ghi chú khác | Endpoint register đã tách logic khỏi controller |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI hỗ trợ nhanh phần code backend, nhưng nhóm cần kiểm tra lại công nghệ đang dùng để tránh thêm package không cần thiết.
```

---

### Lần sử dụng AI số 5

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 03/06/2026 |
| Công cụ AI | Codex |
| Mục đích sử dụng | Triển khai Login API với JWT |
| Phần việc liên quan | Backend / Testing |
| Mức độ sử dụng | Hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
Implement Login API cho PlayCourt Backend. Dùng JWT, BCrypt verify password, login bằng email hoặc số điện thoại, dùng ApiResponse<T>, không thêm migration và không thêm LastLoginAt.
```

#### 4.2. Kết quả AI gợi ý

```text
AI gợi ý thêm LoginRequestDto, LoginResponseDto, IJwtTokenService, JwtTokenService, LoginAsync trong AuthService, endpoint /api/auth/login và cấu hình JWT authentication.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Nhóm áp dụng để triển khai đăng nhập, kiểm tra password bằng BCrypt, tạo JWT token và trả thông tin user an toàn.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Nhóm kiểm tra role claim để tương thích authorization policy, không thay đổi database provider và không tạo migration.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Sẽ cập nhật sau khi commit |
| File liên quan | `PlayCourt.API/Controllers/AuthController.cs`, `PlayCourt.Infrastructure/Services/AuthService.cs`, `PlayCourt.Infrastructure/Services/JwtTokenService.cs`, `PlayCourt.Application/DTOs/Auth/` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet build PlayCourt.sln`, `dotnet test PlayCourt.sln --no-build` |
| Link video demo |  |
| Ghi chú khác | Login API dùng JWT Bearer và BCrypt |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI hỗ trợ nhanh phần JWT và flow đăng nhập, nhưng nhóm vẫn cần tự kiểm tra package, claim và middleware order.
```

---

### Lần sử dụng AI số 6

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 03/06/2026 |
| Công cụ AI | Codex |
| Mục đích sử dụng | Triển khai Email OTP infrastructure |
| Phần việc liên quan | Backend / Database / Testing |
| Mức độ sử dụng | Hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
Implement Email/OTP infrastructure cho PlayCourt Backend bằng EF Core Code First. Thêm VerificationToken entity, enum purpose, DbContext config, migration, OTP service và development email service. Không thêm endpoint verify/forgot password.
```

#### 4.2. Kết quả AI gợi ý

```text
AI gợi ý thêm VerificationTokenPurpose, VerificationToken entity, User navigation, DbSet, Fluent API config, migration, IVerificationTokenService, IEmailService, VerificationTokenService và DevelopmentEmailService.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Nhóm áp dụng để tạo bảng VerificationTokens, sinh OTP 6 số, hash OTP bằng BCrypt, kiểm tra hạn dùng, one-time use và failed attempts.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Nhóm kiểm tra migration, chạy database update, giữ SQL Server, không thêm SMTP package và không triển khai endpoint ngoài scope.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Sẽ cập nhật sau khi commit |
| File liên quan | `PlayCourt.Domain/Entities/VerificationToken.cs`, `PlayCourt.Infrastructure/Data/PlayCourtDbContext.cs`, `PlayCourt.Infrastructure/Services/VerificationTokenService.cs`, `PlayCourt.Infrastructure/Services/DevelopmentEmailService.cs` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet ef migrations add AddVerificationTokenTable`, `dotnet ef database update`, `dotnet build PlayCourt.sln`, `dotnet test PlayCourt.sln --no-build` |
| Link video demo |  |
| Ghi chú khác | Chỉ triển khai infrastructure, chưa có verify email/reset password endpoint |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI hỗ trợ nhanh phần cấu trúc OTP, nhưng nhóm vẫn cần kiểm tra migration và database update để đảm bảo bảng được tạo đúng.
```

---

### Lần sử dụng AI số 7

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 03/06/2026 |
| Công cụ AI | Codex |
| Mục đích sử dụng | Triển khai Verify Email bằng OTP và SMTP |
| Phần việc liên quan | Backend / Auth / Testing |
| Mức độ sử dụng | Hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
Implement Verify Email bằng OTP 6 số. Register tạo OTP và gửi email, thêm endpoint verify-email, resend-verify-email, dùng ApiResponse<T>, dùng MailKit SMTP và không sửa login flow.
```

#### 4.2. Kết quả AI gợi ý

```text
AI gợi ý thêm DTO VerifyEmail/ResendVerifyEmail, EmailSettings, SmtpEmailService, cập nhật AuthService, AuthController, appsettings và controller tests.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Nhóm áp dụng để gửi OTP sau register, verify OTP để set IsEmailVerified, resend OTP có cooldown và gửi email bằng Gmail SMTP.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Nhóm giữ password SMTP trong appsettings.Development.json local, appsettings.json chỉ dùng placeholder, và kiểm tra build/test sau khi thêm MailKit.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Sẽ cập nhật sau khi commit |
| File liên quan | `PlayCourt.API/Controllers/AuthController.cs`, `PlayCourt.Infrastructure/Services/AuthService.cs`, `PlayCourt.Infrastructure/Services/SmtpEmailService.cs`, `PlayCourt.Application/DTOs/Auth/VerifyEmailRequestDto.cs` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet build PlayCourt.sln`, `dotnet test PlayCourt.sln --no-build` |
| Link video demo |  |
| Ghi chú khác | Thêm endpoints `/api/auth/verify-email` và `/api/auth/resend-verify-email` |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI hỗ trợ triển khai nhanh flow verify email, nhưng nhóm cần tự kiểm tra bảo mật config để không đưa SMTP password vào file shared.
```

---

### Lần sử dụng AI số 8

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 03/06/2026 |
| Công cụ AI | Codex |
| Mục đích sử dụng | Triển khai Forgot/Reset/Change Password |
| Phần việc liên quan | Backend / Auth / Testing |
| Mức độ sử dụng | Hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
Triển khai Forgot Password, Reset Password và Change Password cho PlayCourt. Dùng lại VerificationTokenPurpose.PasswordReset, OTP service, SMTP email service, BCrypt và JWT authorize cho change password. Không thêm migration.
```

#### 4.2. Kết quả AI gợi ý

```text
AI gợi ý thêm DTO, mở rộng AuthService/AuthController, thêm email reset password template và controller tests cho các endpoint mới.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Nhóm áp dụng để tạo `/api/auth/forgot-password`, `/api/auth/reset-password` và `/api/auth/change-password`, dùng OTP PasswordReset và hash password mới bằng BCrypt.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Nhóm giữ SQL Server hiện tại, không thêm migration, không ghi SMTP password vào docs và kiểm tra bằng build/test.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Sẽ cập nhật sau khi commit |
| File liên quan | `PlayCourt.API/Controllers/AuthController.cs`, `PlayCourt.Infrastructure/Services/AuthService.cs`, `PlayCourt.Infrastructure/Services/SmtpEmailService.cs`, `PlayCourt.Application/DTOs/Auth/` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet build PlayCourt.sln`; `dotnet test PlayCourt.sln --no-build` passed 19/19 |
| Link video demo |  |
| Ghi chú khác | Không thêm migration vì dùng bảng VerificationTokens đã có |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI hỗ trợ nhanh phần auth flow, nhưng nhóm cần tự kiểm tra lại bảo mật response và test case cho token claim.
```

---

### Lần sử dụng AI số 9

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 03/06/2026 |
| Công cụ AI | Codex |
| Mục đích sử dụng | Triển khai User Profile API |
| Phần việc liên quan | Backend / Testing / Documentation |
| Mức độ sử dụng | Hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
Implement User Profile APIs for PlayCourt API: GET /api/users/me and PUT /api/users/me. Keep profile logic separate from AuthService, use ApiResponse<T>, do not expose PasswordHash, do not allow updating account or court-owner business fields, add controller tests and update docs.
```

#### 4.2. Kết quả AI gợi ý

```text
AI gợi ý tạo DTO trong Application, IUserService, UserService dùng EF Core Include User/CourtOwnerProfile, UsersController đọc ClaimTypes.NameIdentifier và controller tests bằng stub service.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Nhóm áp dụng để tạo GET/PUT /api/users/me, mapping response thủ công, validation FullName/Gender trong service và test controller cho success/fail/unauthorized.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Nhóm giữ AuthService không đổi, không tạo migration, không thêm upload avatar, kiểm tra field nhạy cảm không trả về và chạy build/test toàn solution.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Sẽ cập nhật sau khi commit |
| File liên quan | `PlayCourt.API/Controllers/UsersController.cs`, `PlayCourt.Infrastructure/Services/UserService.cs`, `PlayCourt.Application/DTOs/Users/`, `PlayCourt.ApiTests/UsersControllerTests.cs` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet build PlayCourt.sln`; `dotnet test PlayCourt.sln --no-build` |
| Link video demo |  |
| Ghi chú khác | Không thêm migration; AvatarUrl chỉ là string URL |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI giúp triển khai nhanh theo pattern hiện có, nhưng nhóm vẫn cần tự kiểm tra boundary bảo mật giữa profile, auth và court-owner business data.
```

---

### Lần sử dụng AI số 10

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 04/06/2026 |
| Công cụ AI | Codex |
| Mục đích sử dụng | Triển khai Sport Management API |
| Phần việc liên quan | Backend / Testing |
| Mức độ sử dụng | Hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
Implement Sport Management cho PlayCourt Backend. Tạo API quản lý môn thể thao, dùng ApiResponse<T>, tách logic vào SportService, admin mới được create/update/toggle active, có validate code/name/player count và thêm test.
```

#### 4.2. Kết quả AI gợi ý

```text
AI gợi ý thêm SportsController, DTO cho sport, ISportService, SportService, đăng ký DI và test cho controller/service.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Nhóm áp dụng để tạo API lấy danh sách sport, lấy chi tiết sport, tạo sport, cập nhật sport và bật/tắt trạng thái active.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Nhóm kiểm tra lại quyền admin cho API thay đổi dữ liệu, chuẩn hóa code viết hoa, validate trùng code/name và chạy test toàn solution.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Sẽ cập nhật sau khi commit |
| File liên quan | `PlayCourt.API/Controllers/SportsController.cs`, `PlayCourt.Application/DTOs/Sports/`, `PlayCourt.Application/Interfaces/ISportService.cs`, `PlayCourt.Infrastructure/Services/SportService.cs`, `PlayCourt.ApiTests/SportsControllerTests.cs`, `PlayCourt.ApiTests/SportServiceTests.cs` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet test PlayCourt.sln --no-build` passed 39/39 |
| Link video demo |  |
| Ghi chú khác | Không thêm migration vì bảng Sport đã có trong model/database trước đó |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI hỗ trợ nhanh phần CRUD backend và test, nhưng nhóm vẫn cần tự kiểm tra rule nghiệp vụ, phân quyền admin và kết quả test.
```

---

---

### Lần sử dụng AI số 11

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 06/06/2026 |
| Công cụ AI | Codex |
| Mục đích sử dụng | Triển khai Venue Management API cho CourtOwner |
| Phần việc liên quan | Backend / Testing / Documentation |
| Mức độ sử dụng | Hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
Implement feature/de180310-venue-management cho PlayCourt API. Scope gồm POST /api/venues, GET /api/venues/my, GET /api/venues/{id}, PUT /api/venues/{id}. Chỉ CourtOwner được tạo/sửa/xem Venue của mình. Venue mới tạo có Status = Pending. Không làm Admin approve, không upload file thật, không làm Venue Images/Amenities trong branch này.
```

#### 4.2. Kết quả AI gợi ý

```text
AI gợi ý tạo VenuesController, DTOs/Venues, IVenueService, VenueService dùng EF Core và đăng ký DI. API dùng policy CourtOwner, lấy userId từ JWT claim, tìm CourtOwnerProfile và chỉ thao tác với Venue thuộc owner hiện tại.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Nhóm áp dụng để tạo các endpoint quản lý Venue cho CourtOwner, mapping VenueResponseDto thủ công, validate Name/Address/Latitude/Longitude/OpenTime/CloseTime và kiểm tra ownership bằng CourtOwnerProfileId.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Nhóm giới hạn đúng scope branch, không làm Admin approve, không làm upload file, không làm Venue Images/Amenities, không thêm migration, không sửa AuthService và chỉ thêm dòng DI cần thiết cho IVenueService.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | `556a7fc` |
| File liên quan | `PlayCourt.API/Controllers/VenuesController.cs`, `PlayCourt.Application/DTOs/Venues/`, `PlayCourt.Application/Interfaces/IVenueService.cs`, `PlayCourt.Infrastructure/Services/VenueService.cs`, `PlayCourt.Infrastructure/DependencyInjection.cs` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet build PlayCourt.sln` passed; `dotnet test PlayCourt.sln --no-build` passed 39/39 |
| Link video demo |  |
| Ghi chú khác | Không thêm migration; không cập nhật `docs/REFLECTION.md` |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI giúp triển khai nhanh theo pattern sẵn có, nhưng nhóm vẫn cần tự kiểm tra route, phân quyền CourtOwner, ownership của Venue và kết quả build/test trước khi commit.
```
---

### Lần sử dụng AI số 12

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 07/06/2026 |
| Công cụ AI | Antigravity |
| Mục đích sử dụng | Triển khai Court Management API và DTOs/PricingRules |
| Phần việc liên quan | Backend / Testing |
| Mức độ sử dụng | Hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
Implement Court Management API cho PlayCourt Backend. Tạo CourtsController, ICourtService, CourtService, DTOs/Courts và DTOs/PricingRules. Chỉ CourtOwner sở hữu Venue mới được thêm/sửa Court. Court cần SportId. Không sửa AuthService, không format toàn bộ solution, DependencyInjection.cs chỉ thêm đúng dòng service của mình.
```

#### 4.2. Kết quả AI gợi ý

```text
AI gợi ý tạo CourtDto, CreateCourtRequestDto, UpdateCourtRequestDto trong DTOs/Courts; PricingRuleDto, CreatePricingRuleRequestDto, UpdatePricingRuleRequestDto trong DTOs/PricingRules; ICourtService với 4 phương thức; CourtService với ownership verification qua chain Venue → CourtOwnerProfile → UserProfile → UserId; CourtsController với explicit route attributes hỗ trợ /api/venues/{venueId}/courts và /api/courts/{id}; đăng ký DI thêm 1 dòng.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Áp dụng để tạo đủ Controller, Interface, Service và DTOs theo đúng scope. Verify ownership theo chain entity thực của project. Lấy currentUserId từ ClaimTypes.NameIdentifier nhất quán với JwtTokenService hiện có.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Kiểm tra lại scope — AI ban đầu tạo thêm PricingRuleService, IPricingRuleService, PricingRulesController ngoài yêu cầu, đã yêu cầu AI xóa đúng scope chỉ giữ DTOs/PricingRules. Kiểm tra DependencyInjection.cs chỉ thêm đúng 1 dòng ICourtService. Chạy dotnet build toàn solution để xác nhận.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | `8c80134` |
| File liên quan | `PlayCourt.API/Controllers/CourtsController.cs`, `PlayCourt.Application/Interfaces/ICourtService.cs`, `PlayCourt.Infrastructure/Services/CourtService.cs`, `PlayCourt.Application/DTOs/Courts/`, `PlayCourt.Application/DTOs/PricingRules/`, `PlayCourt.Infrastructure/DependencyInjection.cs` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet build PlayCourt.sln` passed — 0 Warning(s), 0 Error(s) |
| Link video demo |  |
| Ghi chú khác | Không sửa AuthService, không sửa DbContext, không format toàn solution |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI giúp sinh code nhanh theo đúng pattern đã có trong project, nhưng cần kiểm soát scope chặt — AI có xu hướng làm thêm ngoài yêu cầu (thêm PricingRule service/controller khi chỉ cần DTOs). Việc review và yêu cầu AI revert là cần thiết trước khi commit.
```

---

### Lần sử dụng AI số 13

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 07/06/2026 |
| Công cụ AI | Antigravity |
| Mục đích sử dụng | Triển khai Pricing Rule API (CRUD bảng giá giờ) |
| Phần việc liên quan | Backend / Testing |
| Mức độ sử dụng | Hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
Branch tiếp theo: feature/de180313-pricing-rules. Scope: POST /api/courts/{courtId}/pricing-rules, GET /api/courts/{courtId}/pricing-rules, PUT /api/pricing-rules/{id}, DELETE /api/pricing-rules/{id}. DTOs đã có từ branch trước. Thêm logic validate để kiểm tra không cho phép tạo PricingRule bị overlap giờ (StartAt và EndAt) trong cùng 1 Court. Owner chỉ được thao tác với PricingRule của Court do Venue của mình quản lý.
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất tạo PricingRulesController với 4 endpoints, IPricingRuleService và PricingRuleService. Service sẽ kiểm tra quyền sở hữu bằng cách join từ PricingRule -> Court -> Venue -> CourtOwnerProfile. Logic chống overlap: StartAt < request.EndAt && EndAt > request.StartAt. DI đăng ký thêm IPricingRuleService.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Áp dụng toàn bộ cấu trúc API, validation logic chống overlap và ownership check.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Chạy build và test tự động để đảm bảo logic chạy đúng. Không có thay đổi lớn so với code AI sinh.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | `3582d61` |
| File liên quan | `PlayCourt.API/Controllers/PricingRulesController.cs`, `PlayCourt.Application/Interfaces/IPricingRuleService.cs`, `PlayCourt.Infrastructure/Services/PricingRuleService.cs`, `PlayCourt.Infrastructure/DependencyInjection.cs` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet build PlayCourt.sln` passed, `dotnet test PlayCourt.sln` passed 39/39 |
| Link video demo |  |
| Ghi chú khác | Validation chống overlap hoạt động ổn định. |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI xử lý tốt logic kiểm tra overlap giờ (StartAt < EndAt overlap) và chain kiểm tra quyền sở hữu.
```

---


### Lần sử dụng AI số 14

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 07/06/2026 |
| Công cụ AI | Antigravity |
| Mục đích sử dụng | Triển khai Court Schedule API (Quản lý lịch khóa sân) |
| Phần việc liên quan | Backend / Testing |
| Mức độ sử dụng | Hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
Branch tiếp theo: feature/de180313-court-schedule. POST /api/courts/{courtId}/schedules, GET /api/courts/{courtId}/schedules, DELETE /api/court-schedules/{id}. Áp dụng và làm tiếp chức năng này.
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất tạo CourtSchedulesController, ICourtScheduleService, CourtScheduleService và các DTOs tương ứng. Service tiếp tục tái sử dụng logic check quyền sở hữu (ownership chain) và thêm cơ chế validate chống trùng lặp giờ khóa sân.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Sử dụng toàn bộ DTOs, Interface, Controller và Service do AI tạo ra. Kiểm tra validation logic chống overlap hoàn toàn tương thích với phần PricingRule.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Kiểm tra code bằng dotnet build và tự chạy dotnet test toàn bộ solution (pass 39/39).
```

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | `3fe045c` |
| File liên quan | `PlayCourt.API/Controllers/CourtSchedulesController.cs`, `PlayCourt.Application/Interfaces/ICourtScheduleService.cs`, `PlayCourt.Infrastructure/Services/CourtScheduleService.cs`, `PlayCourt.Application/DTOs/CourtSchedules/` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet build PlayCourt.sln` passed, `dotnet test PlayCourt.sln` passed 39/39 |
| Link video demo |  |
| Ghi chú khác | Hoàn thiện module cho CourtOwner |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI hỗ trợ viết các endpoints một cách nhất quán dựa trên các kiến trúc đã có. Validation chính xác.
```

---

### Lần sử dụng AI số 15

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 24/06/2026 |
| Công cụ AI | Codex |
| Mục đích sử dụng | Phân tích và triển khai CRUD PlayerSport cho User Profile |
| Phần việc liên quan | Backend / Testing / Documentation |
| Mức độ sử dụng | Hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
Tôi cần làm chức năng thêm sport cho user tức PlayerSport. Hãy phân tích nghiệp vụ và nên làm vào đâu. Tôi muốn thêm vào IUserService vì bản chất vẫn là một phần của profile user. Hãy làm CRUD đầy đủ: thêm, xem, đổi trình độ, xóa.
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất giữ PlayerSport trong IUserService/UserService, tạo 4 endpoint tự phục vụ dưới `/api/users/me/sports`, lấy userId từ JWT, dùng DTO riêng và kiểm tra profile, sport tồn tại/active, SkillLevel hợp lệ, dữ liệu trùng và quyền sở hữu.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Áp dụng cấu trúc endpoint, DTO, interface và service logic. Thêm GET danh sách, POST thêm sport, PUT đổi SkillLevel và DELETE xóa PlayerSport. Dùng ApiResponse<T> và không trả trực tiếp entity EF Core.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Nhóm lựa chọn mở rộng IUserService thay vì tạo IPlayerSportService để phù hợp phạm vi hiện tại. Route sử dụng sportId, mọi truy vấn giới hạn theo UserProfile của JWT và unique constraint database tiếp tục bảo vệ trường hợp request đồng thời. Không tạo migration vì schema đã hỗ trợ PlayerSport.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Sẽ cập nhật sau khi commit |
| File liên quan | `PlayCourt.API/Controllers/UsersController.cs`, `PlayCourt.Application/Interfaces/IUserService.cs`, `PlayCourt.Application/DTOs/Users/`, `PlayCourt.Infrastructure/Services/UserService.cs`, `PlayCourt.ApiTests/UsersControllerTests.cs`, `PlayCourt.ApiTests/UserServicePlayerSportTests.cs` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet test PlayCourt.sln --no-restore` passed 66/66; `dotnet build PlayCourt.sln --no-restore` passed với 0 warning, 0 error |
| Link video demo |  |
| Ghi chú khác | Đã hướng dẫn test bằng Swagger/Postman với tài khoản mẫu và JWT Bearer. |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI hỗ trợ tốt việc phân tách trách nhiệm giữa Sport catalogue và PlayerSport profile, đồng thời đề xuất validation và test case đầy đủ. Nhóm vẫn kiểm tra lại contract, quyền sở hữu và kết quả build/test.
```

---

### Lần sử dụng AI số 16

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 24/06/2026 |
| Công cụ AI | Codex |
| Mục đích sử dụng | Admin xác minh CourtOwner và chặn tạo Venue khi chưa được duyệt |
| Phần việc liên quan | Backend / Database / Documentation |

```text
AI hỗ trợ tách CourtOwnerService/Controller theo layer hiện có, thêm filter trạng thái, approve/reject có lý do tùy chọn và migration RejectionReason. Nhóm giữ rule tạo Venue tại VenueService; build solution đã pass 0 warning, 0 error.
```

---

### Lần sử dụng AI số 17

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 24/06/2026 |
| Công cụ AI | Antigravity |
| Mục đích sử dụng | Hoàn thiện toàn bộ tính năng Venue Module |
| Phần việc liên quan | Backend / Testing / Documentation |
| Mức độ sử dụng | Hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
Bạn hãy code feature/de180310-complete-venue-module với các nội dung tôi đã đưa bạn (Public Discovery, Admin Approval, Images, Amenities, Opening Hours). Trước khi code hãy lên plan trước. Lấy lên code trên nhánh dev.
```

#### 4.2. Kết quả AI gợi ý

```text
AI đề xuất cập nhật toàn diện cấu trúc của module Venue: viết mới AmenitiesController, AdminVenuesController; thiết kế lại VenuesController; tích hợp 3 layer Service tương ứng cùng bộ DTO đầy đủ phục vụ thao tác của Public Player, Admin và Court Owner.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Áp dụng toàn bộ việc sinh API endpoint, validate nghiệp vụ, cấu trúc relation EF Core `Include` nhiều tầng, xử lý upload ảnh (mô phỏng), và pagination cho Public Discovery.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Nhóm yêu cầu AI tự động review kết quả Unit Test cũ và test nhanh qua API HTTP Get (Invoke-RestMethod) để xác nhận code sinh ra không phá hỏng luồng chạy chung của toàn solution.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Sẽ cập nhật sau khi commit |
| File liên quan | `PlayCourt.API/Controllers/VenuesController.cs`, `AdminVenuesController.cs`, `AmenitiesController.cs`, `VenueService.cs`, `AdminVenueService.cs`, `AmenityService.cs`, `DTOs/Venues/` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet test PlayCourt.sln --no-build` passed 66/66; |
| Link video demo |  |
| Ghi chú khác | Hoàn toàn đáp ứng được logic nghiệp vụ phức tạp của mảng Venue |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI xử lý xuất sắc các phần code tích hợp lớn, có khả năng tư duy bao quát và liên kết chặt chẽ các file với nhau trong mô hình Clean Architecture. AI cũng chủ động chạy test giúp team tiết kiệm đáng kể thời gian QA.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

Đánh dấu mức độ AI hỗ trợ ở từng hạng mục.

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Phân tích yêu cầu |  | x |  |  | AI hỗ trợ tóm tắt yêu cầu |
| Viết user story/use case |  | x |  |  | AI hỗ trợ gợi ý luồng người dùng |
| Thiết kế database |  |  | x |  | AI hướng dẫn tạo model và DbContext |
| Thiết kế kiến trúc hệ thống |  |  | x |  | AI hỗ trợ định hướng Domain, Application, Infrastructure và API layer |
| Thiết kế giao diện |  | x |  |  | AI hỗ trợ ý tưởng giao diện cơ bản |
| Code frontend | x |  |  |  | Chưa triển khai chính trong giai đoạn này |
| Code backend |  |  | x |  | AI hỗ trợ các API backend, gồm CourtOwner Approval và PlayerSport CRUD |
| Debug lỗi |  | x |  |  | AI hỗ trợ kiểm tra duplicate index, filter, constraint và lỗi compile/test |
| Viết test case |  | x |  |  | AI hỗ trợ định hướng test smoke, controller test, password management test, user profile test, sport management test và PlayerSport CRUD test |
| Kiểm thử sản phẩm |  | x |  |  | Nhóm tự chạy format, build và test để kiểm chứng |
| Tối ưu code |  | x |  |  | AI hỗ trợ review code đơn giản |
| Viết báo cáo |  | x |  |  | AI hỗ trợ chỉnh tài liệu ngắn gọn |
| Làm slide thuyết trình | x |  |  |  | Chưa thực hiện |

---

## 6. Các lỗi hoặc hạn chế từ AI

Ghi lại các trường hợp AI trả lời sai, thiếu, chưa phù hợp hoặc sinh code không chạy.

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | AI có lúc đề xuất package test chưa phù hợp với SQL Server | Review package được thêm vào project | Loại bỏ package không cần thiết |
| 2 | Một số gợi ý code cần chỉnh theo entity thật của project | Build báo lỗi hoặc review source code | Sửa theo `User`, `UserProfile`, `CourtOwnerProfile` hiện có |
| 3 | Một số nội dung tài liệu còn dài | Review lại trước khi nộp | Rút gọn và viết lại đơn giản |

---

## 7. Kiểm chứng kết quả AI

Mô tả cách sinh viên/nhóm kiểm tra lại kết quả do AI gợi ý.

Có thể bao gồm:

- Chạy thử chương trình
- Viết test case
- So sánh với yêu cầu đề bài
- Kiểm tra output
- Đối chiếu tài liệu môn học
- Hỏi lại giảng viên
- Review cùng thành viên nhóm
- Kiểm tra lỗi bảo mật
- Kiểm tra bằng dữ liệu mẫu
- So sánh trước và sau khi dùng AI

### Nội dung kiểm chứng

```text
Nhóm kiểm chứng bằng cách format code, build solution, kiểm tra EF Core pending model changes và chạy test tự động. Các thay đổi liên quan database được kiểm tra lại qua migration và DbContext snapshot.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

Mô tả phần sinh viên tự làm, phần AI hỗ trợ và phần đã tự cải tiến.

```text
Sinh viên tự kiểm tra yêu cầu, chạy build/test và chỉnh lại các phần AI gợi ý chưa đúng với project.
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
| Nguyen Phan Huy | DE180519 | Frontend, backend project setup, architecture, API coordination, testing and documentation | Có | Commit/PR sẽ cập nhật trong quá trình làm project |
| Phan Thanh Vuong | DE180405 | Backend feature development | Có | Commit/PR sẽ cập nhật trong quá trình làm project |
| Nguyen Van Hai | DE180313 | Backend feature development | Có | Commit/PR sẽ cập nhật trong quá trình làm project |
| Phan Quoc Khanh | DE180310 | Backend feature development | Có | Commit/PR sẽ cập nhật trong quá trình làm project |
| Trinh Viet Hoang | DE190946 | Backend feature development | Có | Commit/PR sẽ cập nhật trong quá trình làm project |

---

## 9. Reflection cuối bài

### 9.1. AI đã hỗ trợ em/nhóm ở điểm nào?

```text
AI hỗ trợ phân tích yêu cầu, gợi ý cấu trúc code, debug lỗi và viết tài liệu.
```

### 9.2. Phần nào em/nhóm không sử dụng theo gợi ý của AI? Vì sao?

```text
Nhóm không dùng nguyên văn toàn bộ gợi ý của AI. Các phần sai công nghệ hoặc chưa đúng kiến trúc đều được chỉnh lại.
```

### 9.3. Em/nhóm đã kiểm tra tính đúng đắn của kết quả AI như thế nào?

```text
Nhóm kiểm tra bằng review code, chạy build/test và so sánh với yêu cầu của project.
```

### 9.4. Nếu không có AI, phần nào sẽ khó khăn nhất?

```text
Phần khó nhất là thiết kế database, cấu hình quan hệ EF Core và xử lý backend theo Clean Architecture.
```

### 9.5. Sau bài tập/project này, em/nhóm học được gì về môn học?

```text
Nhóm hiểu rõ hơn về ASP.NET Core Web API, EF Core, DI, middleware và cách chia layer trong backend.
```

### 9.6. Sau bài tập/project này, em/nhóm học được gì về cách sử dụng AI có trách nhiệm?

```text
Nhóm học được rằng AI chỉ là công cụ hỗ trợ, kết quả phải được kiểm tra lại trước khi đưa vào bài.
```

---

## 10. Cam kết học thuật

Sinh viên/nhóm cam kết rằng:

- Nội dung AI hỗ trợ đã được ghi nhận trung thực.
- Không nộp nguyên văn kết quả AI mà không kiểm tra.
- Có khả năng giải thích các phần đã nộp.
- Chịu trách nhiệm về tính đúng đắn của sản phẩm cuối cùng.
- Hiểu rằng việc sử dụng AI không khai báo có thể ảnh hưởng đến kết quả đánh giá.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyen Phan Huy | 03/06/2026 |

---

## DE190946 - Player Matching (28/06/2026)

| Item | Detail |
|---|---|
| Student | Trinh Viet Hoang (DE190946) |
| Branch | `feature/DE190946-player-matching` |
| AI support | Read the existing architecture, propose the matching workflow, implement DTO/service/controller code, and generate tests |
| Human verification | Reviewed authorization and EF relationships; ran `dotnet build` and the full automated test suite |
| Important correction | Reused a rejected join-request row instead of inserting a duplicate that would violate the SQL Server unique index |
| Verification result | Build succeeded with 0 warnings; 75/75 tests passed |
