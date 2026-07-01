# Changelog

## 1. Quy định ghi Changelog

File này dùng để ghi lại các thay đổi quan trọng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

Nguyên tắc ghi changelog:

- Chỉ ghi những gì đã hoàn thành thật sự.
- Không ghi kế hoạch nếu chưa thực hiện.
- Mỗi thay đổi nên có ngày, nội dung, người thực hiện và minh chứng.
- Nếu có AI hỗ trợ, cần ghi rõ AI đã hỗ trợ phần nào.
- Nếu có commit GitHub, cần ghi link commit.
- Nếu có lỗi đã sửa, cần ghi rõ lỗi, nguyên nhân và cách xử lý.

---

## 2. Thông tin project

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
| Repository URL | https://github.com/fptu-se-su26/prn232-su26-ai-audit-project-prn232_se18d05_group-01 |
| Ngày bắt đầu | 11/05/2026 |
| Ngày hoàn thành | 05/07/2026 |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 | 11/05/2026 - 17/05/2026 | Khởi tạo project | Done |
| Phase 02 | 18/05/2026 - 24/05/2026 | Phân tích yêu cầu | Done |
| Phase 03 | 25/05/2026 - 31/05/2026 | Thiết kế hệ thống | Done |
| Phase 04 | 01/06/2026 - 21/06/2026 | Implementation | In Progress |
| Phase 05 | 22/06/2026 - 28/06/2026 | Testing & Debug | In Progress |
| Phase 06 | 29/06/2026 - 05/07/2026 | Hoàn thiện báo cáo và demo | Not Started |

---

# [Phase 01] Khởi tạo project

## Ngày thực hiện

```text
14/05/2026
```

## Đã hoàn thành

- [x] Tạo repository
- [x] Tạo cấu trúc thư mục project
- [x] Tạo file README.md
- [x] Tạo thư mục `docs/`
- [x] Tạo file `AI_AUDIT_LOG.md`
- [x] Tạo file `PROMPTS.md`
- [x] Tạo file `REFLECTION.md`
- [x] Tạo file `CHANGELOG.md`
- [x] Khởi tạo source code ban đầu
- [x] Cài đặt thư viện/công cụ cần thiết
- [x] Cấu hình môi trường chạy project

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Khởi tạo README và thông tin project ban đầu | Nguyen Phan Huy | `README.md` | Commit/PR sẽ cập nhật |
| 2 | Khởi tạo bộ tài liệu AI audit | Nguyen Phan Huy | `docs/` | Commit/PR sẽ cập nhật |
| 3 | Khởi tạo cấu trúc solution và source code ban đầu | Nguyen Phan Huy | `PlayCourt.*`, `PlayCourt.sln` | Commit/PR sẽ cập nhật |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
AI hỗ trợ chuẩn hóa nội dung README, điền thông tin nền ban đầu cho tài liệu audit và gợi ý cấu trúc ghi chú dự án.
```

## Commit/Screenshot minh chứng

```text
Commit sẽ cập nhật sau khi hoàn tất.
```

## Ghi chú

```text
Thông tin ban đầu được cập nhật trước khi project đi vào các phase tiếp theo.
```

---

# [Phase 02] Phân tích yêu cầu

## Ngày thực hiện

```text
18/05/2026
```

## Đã hoàn thành

- [x] Xác định problem statement
- [x] Xác định user roles
- [x] Viết user stories
- [x] Viết use cases
- [x] Xác định functional requirements
- [x] Xác định non-functional requirements
- [x] Xác định business rules
- [x] Xác định acceptance criteria
- [x] Review yêu cầu với nhóm
- [x] Chỉnh sửa yêu cầu sau feedback

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Tạo domain entity model cho các bảng chính của PlayCourt | Nguyen Phan Huy | `PlayCourt.Domain/Entities/` | Commit sẽ cập nhật |
| 2 | Tạo enum cho các trường trạng thái/loại dữ liệu thay cho magic number | Nguyen Phan Huy | `PlayCourt.Domain/Enums/DomainEnums.cs` | Commit sẽ cập nhật |
| 3 | Cấu hình DbContext, relationship, index, check constraint, soft-delete filter và migration | Nguyen Phan Huy | `PlayCourt.Infrastructure/Data/` | Commit sẽ cập nhật |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
AI hướng dẫn tạo model và cấu hình DbContext cho EF Core, bao gồm entity, enum, relationship, index, constraint, soft-delete filter và migration.
```

## Commit/Screenshot minh chứng

```text
Commit sẽ cập nhật sau khi hoàn tất.
```

## Ghi chú

```text
Nhóm đã kiểm tra lại bằng format, build, EF migration check và test trước khi commit.
```

---

# [Phase 03] Thiết kế hệ thống

## Ngày thực hiện

```text
21/05/2026
```

## Đã hoàn thành

- [x] Thiết kế kiến trúc tổng quan
- [x] Thiết kế database/ERD
- [x] Thiết kế API
- [x] Thiết kế giao diện/wireframe
- [x] Thiết kế flow xử lý
- [x] Thiết kế class diagram
- [x] Thiết kế sequence diagram
- [x] Thiết kế security/authorization flow
- [x] Review thiết kế
- [x] Chỉnh sửa thiết kế sau feedback

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Chọn kiến trúc Clean Architecture cho backend | Nguyen Phan Huy | `PlayCourt.API`, `PlayCourt.Application`, `PlayCourt.Domain`, `PlayCourt.Infrastructure` | Commit sẽ cập nhật |
| 2 | Thiết kế database chính cho user, sân, booking, payment và match | Nguyen Phan Huy | `PlayCourt.Domain/Entities/`, `PlayCourt.Infrastructure/Data/` | Commit sẽ cập nhật |
| 3 | Thiết kế API flow cơ bản cho auth và booking | Nguyen Phan Huy | `docs/`, `PlayCourt.API/Controllers/` | Commit sẽ cập nhật |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
AI hỗ trợ gợi ý kiến trúc layer, quan hệ database, luồng API và cách kiểm tra thiết kế.
```

## Commit/Screenshot minh chứng

```text
Commit sẽ cập nhật sau khi hoàn tất.
```

## Ghi chú

```text
Thiết kế được giữ đơn giản để phù hợp phạm vi môn học.
```

---

# [Phase 04] Implementation

## Ngày thực hiện

```text
03/06/2026
```

## Đã hoàn thành

- [x] Tạo project structure
- [x] Cài đặt database connection
- [x] Xây dựng backend
- [ ] Xây dựng frontend
- [x] Xây dựng authentication/authorization
- [x] Xử lý CRUD
- [x] Xử lý validation
- [ ] Tích hợp API
- [ ] Xử lý upload/download file
- [x] Xử lý lỗi
- [ ] Tối ưu giao diện
- [ ] Cập nhật README hướng dẫn chạy

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Tách cấu hình service và request pipeline khỏi `Program.cs` | Nguyen Phan Huy | `PlayCourt.API/Program.cs`, `PlayCourt.API/DependencyInjection.cs` | Commit sẽ cập nhật |
| 2 | Thêm middleware xử lý exception thống nhất cho API | Nguyen Phan Huy | `PlayCourt.API/Middlewares/ExceptionHandlingMiddleware.cs` | Commit sẽ cập nhật |
| 3 | Thêm common response model cho Application layer | Nguyen Phan Huy | `PlayCourt.Application/Common/Responses/ApiResponse.cs` | Commit sẽ cập nhật |
| 4 | Thêm Application interface/DTO placeholder và service implementation placeholder ở Infrastructure | Nguyen Phan Huy | `PlayCourt.Application/Interfaces/`, `PlayCourt.Application/DTOs/`, `PlayCourt.Infrastructure/Services/` | Commit sẽ cập nhật |
| 5 | Đăng ký dependency injection theo từng layer và cập nhật package reference cần thiết | Nguyen Phan Huy | `PlayCourt.Application/DependencyInjection.cs`, `PlayCourt.Infrastructure/DependencyInjection.cs`, `*.csproj` | Commit sẽ cập nhật |
| 6 | Thêm Register API cho Player và Owner | Nguyen Phan Huy | `PlayCourt.API/Controllers/AuthController.cs`, `PlayCourt.Infrastructure/Services/AuthService.cs`, `PlayCourt.Application/DTOs/Auth/` | Commit sẽ cập nhật |
| 7 | Thêm BCrypt để hash password khi đăng ký | Nguyen Phan Huy | `PlayCourt.Infrastructure/PlayCourt.Infrastructure.csproj` | Commit sẽ cập nhật |
| 8 | Thêm Login API bằng email/số điện thoại | Nguyen Phan Huy | `PlayCourt.API/Controllers/AuthController.cs`, `PlayCourt.Infrastructure/Services/AuthService.cs`, `PlayCourt.Application/DTOs/Auth/` | Commit sẽ cập nhật |
| 9 | Thêm JWT token service và cấu hình JWT Bearer | Nguyen Phan Huy | `PlayCourt.Infrastructure/Services/JwtTokenService.cs`, `PlayCourt.API/DependencyInjection.cs`, `PlayCourt.API/appsettings.json` | Commit sẽ cập nhật |
| 10 | Thêm VerificationToken entity và purpose enum | Nguyen Phan Huy | `PlayCourt.Domain/Entities/VerificationToken.cs`, `PlayCourt.Domain/Enums/DomainEnums.cs`, `PlayCourt.Domain/Entities/User.cs` | Commit sẽ cập nhật |
| 11 | Thêm DbContext config và migration cho VerificationTokens | Nguyen Phan Huy | `PlayCourt.Infrastructure/Data/PlayCourtDbContext.cs`, `PlayCourt.Infrastructure/Data/Migrations/20260603081015_AddVerificationTokenTable.cs` | Commit sẽ cập nhật |
| 12 | Thêm OTP service và development email service | Nguyen Phan Huy | `PlayCourt.Infrastructure/Services/VerificationTokenService.cs`, `PlayCourt.Infrastructure/Services/DevelopmentEmailService.cs`, `PlayCourt.Application/Interfaces/` | Commit sẽ cập nhật |
| 13 | Thêm Verify Email và Resend Verify Email API | Nguyen Phan Huy | `PlayCourt.API/Controllers/AuthController.cs`, `PlayCourt.Infrastructure/Services/AuthService.cs`, `PlayCourt.Application/DTOs/Auth/` | Commit sẽ cập nhật |
| 14 | Thêm SMTP email service bằng MailKit | Nguyen Phan Huy | `PlayCourt.Infrastructure/Services/SmtpEmailService.cs`, `PlayCourt.Application/Settings/EmailSettings.cs`, `PlayCourt.Infrastructure/PlayCourt.Infrastructure.csproj` | Commit sẽ cập nhật |
| 15 | Thêm Forgot Password, Reset Password và Change Password API | Nguyen Phan Huy | `PlayCourt.API/Controllers/AuthController.cs`, `PlayCourt.Infrastructure/Services/AuthService.cs`, `PlayCourt.Application/DTOs/Auth/` | Commit sẽ cập nhật |
| 16 | Thêm User Profile API cho người dùng đăng nhập | Nguyen Phan Huy | `PlayCourt.API/Controllers/UsersController.cs`, `PlayCourt.Infrastructure/Services/UserService.cs`, `PlayCourt.Application/DTOs/Users/` | Commit sẽ cập nhật |
| 17 | Thêm Sport Management API cho danh mục môn thể thao | Nguyen Phan Huy | `PlayCourt.API/Controllers/SportsController.cs`, `PlayCourt.Infrastructure/Services/SportService.cs`, `PlayCourt.Application/DTOs/Sports/` | Commit sẽ cập nhật |
| 18 | Thêm Venue Management API cho CourtOwner | Phan Quoc Khanh | `PlayCourt.API/Controllers/VenuesController.cs`, `PlayCourt.Application/DTOs/Venues/`, `PlayCourt.Application/Interfaces/IVenueService.cs`, `PlayCourt.Infrastructure/Services/VenueService.cs` | `556a7fc` |
| 19 | Đăng ký VenueService vào dependency injection | Phan Quoc Khanh | `PlayCourt.Infrastructure/DependencyInjection.cs` | `556a7fc` |
| 20 | Thêm Court Management API và DTOs/PricingRules | Vo Van Hai | `PlayCourt.API/Controllers/CourtsController.cs`, `PlayCourt.Application/Interfaces/ICourtService.cs`, `PlayCourt.Infrastructure/Services/CourtService.cs`, `PlayCourt.Application/DTOs/Courts/`, `PlayCourt.Application/DTOs/PricingRules/`, `PlayCourt.Infrastructure/DependencyInjection.cs` | `8c80134` |
| 21 | Thêm Pricing Rule API | Vo Van Hai | `PlayCourt.API/Controllers/PricingRulesController.cs`, `PlayCourt.Application/Interfaces/IPricingRuleService.cs`, `PlayCourt.Infrastructure/Services/PricingRuleService.cs`, `PlayCourt.Infrastructure/DependencyInjection.cs` | `3582d61` |
| 22 | Thêm Court Schedule API | Vo Van Hai | `PlayCourt.API/Controllers/CourtSchedulesController.cs`, `PlayCourt.Application/Interfaces/ICourtScheduleService.cs`, `PlayCourt.Infrastructure/Services/CourtScheduleService.cs`, `PlayCourt.Application/DTOs/CourtSchedules/`, `PlayCourt.Infrastructure/DependencyInjection.cs` | `3fe045c` |
| 23 | Thêm CRUD PlayerSport cho hồ sơ người dùng hiện tại | Phan Thanh Vuong | `PlayCourt.API/Controllers/UsersController.cs`, `PlayCourt.Application/Interfaces/IUserService.cs`, `PlayCourt.Application/DTOs/Users/`, `PlayCourt.Infrastructure/Services/UserService.cs` | Sẽ cập nhật sau khi commit |
| 24 | Thêm Admin quản lý xác minh CourtOwner và chặn owner chưa duyệt tạo Venue | Nguyen Phan Huy | `CourtOwnersController.cs`, `CourtOwnerService.cs`, `DTOs/CourtOwners/`, migration `AddCourtOwnerRejectionReason`, `VenueService.cs` | Sẽ cập nhật sau khi commit |
| 25 | Hoàn thiện toàn bộ tính năng Venue Module (Public Search, Admin Approve, Images, Amenities, Opening Hours) | DE180310 | `VenuesController.cs`, `AdminVenuesController.cs`, `AmenitiesController.cs`, `VenueService.cs`, `AdminVenueService.cs`, `AmenityService.cs` | Sẽ cập nhật sau khi commit |
| 26 | Thiết kế và hoàn thiện toàn bộ tính năng Review & VenueStaff Module (15 APIs mới) | Phan Quoc Khanh | `PlayCourt.API/Controllers/ReviewsController.cs`, `PlayCourt.API/Controllers/VenueStaffsController.cs`, `PlayCourt.Infrastructure/Services/ReviewService.cs`, `PlayCourt.Infrastructure/Services/VenueStaffService.cs`, `PlayCourt.Application/DTOs/ReviewDtos.cs`, `PlayCourt.Application/DTOs/VenueStaffDtos.cs` | Sẽ cập nhật sau khi commit |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
AI hỗ trợ đề xuất cách setup layer, gom DependencyInjection theo API/Application/Infrastructure, thêm middleware exception, common ApiResponse và các API backend như Auth, User Profile, Sport Management, Venue Management và PlayerSport CRUD. Nhóm tự review scope thay đổi và chạy kiểm chứng trước khi commit.
```

## Commit/Screenshot minh chứng

```text
Commit sẽ cập nhật sau khi hoàn tất.
```

## Ghi chú

```text
Thay đổi này gồm backend foundation, Register/Login API, Email OTP, Verify Email, Password Management, User Profile API, Sport Management API, Venue Management API, Court Management API, Pricing Rule API, Court Schedule API và PlayerSport CRUD. Kết quả kiểm chứng gần nhất: build passed và 66/66 test passed.
```

---

# [Phase 05] Testing & Debug

## Ngày thực hiện

```text
03/06/2026
```

## Đã hoàn thành

- [x] Viết test case
- [x] Chạy test chức năng chính
- [x] Kiểm tra output
- [x] Kiểm tra validation
- [ ] Kiểm tra lỗi giao diện
- [ ] Kiểm tra lỗi database
- [ ] Kiểm tra phân quyền
- [ ] Kiểm tra bảo mật cơ bản
- [x] Fix bug
- [x] Chạy lại sau khi fix bug
- [x] Ghi nhận kết quả test

## Danh sách lỗi đã xử lý

| STT | Lỗi phát hiện | Nguyên nhân | Cách xử lý | Trạng thái |
|---:|---|---|---|---|
| 1 | Register Owner thiếu BusinessName có thể rơi xuống SQL lỗi | Chưa validate trước khi insert | Thêm business validation trong AuthService | Fixed |
| 2 | AI đề xuất thêm SQLite test package | Không đúng stack SQL Server của project | Remove package, chỉ giữ test controller đơn giản | Fixed |
| 3 | Controller có thể trả validation mặc định không đúng wrapper | ApiController tự xử lý ModelState | Suppress auto validation và trả ApiResponse<T> | Fixed |
| 4 | Role FE Owner khác enum backend CourtOwner | Tên role khác nhau giữa FE và BE | Map Owner sang UserRole.CourtOwner | Fixed |
| 5 | Duplicate email/phone có thể lỗi DB | Chưa check trước khi save | Check bằng EF trước transaction | Fixed |
| 6 | JWT role claim chưa có dạng literal `role` | Token ban đầu chỉ có `ClaimTypes.Role` | Thêm claim `role` để dễ kiểm tra và vẫn giữ policy compatibility | Fixed |
| 7 | Build bị khóa DLL bởi process API đang chạy | `PlayCourt.API` đang giữ file trong `bin` | Dừng đúng process API rồi chạy lại build | Fixed |
| 8 | EmailSettings bind lỗi overload cấu hình | Project thiếu extension bind trực tiếp từ IConfigurationSection | Bind thủ công từng field trong DI | Fixed |
| 9 | Change Password test thiếu HttpContext bị lỗi null principal | Controller đọc claim khi chưa có principal | Kiểm tra `User` null và trả Unauthorized | Fixed |
| 10 | User Profile red test bị khóa bởi API process đang chạy | `PlayCourt.API` đang giữ DLL trong `bin` | Dừng process API rồi chạy lại test để thấy lỗi missing symbol đúng kỳ vọng | Fixed |
| 11 | Sport Management cần chặn trùng code/name | Nếu không kiểm tra trước có thể lỗi database hoặc dữ liệu trùng | Validate trong SportService trước khi save | Fixed |
| 12 | Stub IUserService trong test không còn compile sau khi mở rộng interface | IUserService được thêm 4 method PlayerSport nhưng stub cũ chưa triển khai | Bổ sung implementation mặc định cho stub rồi tiếp tục chu kỳ test-first | Fixed |

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Thêm test controller cho Register success/fail | Nguyen Phan Huy | `PlayCourt.ApiTests/AuthControllerTests.cs` | `dotnet test PlayCourt.sln --no-build` |
| 2 | Chạy build toàn solution | Nguyen Phan Huy | `PlayCourt.sln` | Build passed |
| 3 | Chạy test toàn solution | Nguyen Phan Huy | `PlayCourt.ApiTests` | 12/12 tests passed |
| 4 | Thêm test Login controller và JWT role claim | Nguyen Phan Huy | `PlayCourt.ApiTests/AuthControllerTests.cs`, `PlayCourt.ApiTests/JwtTokenServiceTests.cs` | Tests passed |
| 5 | Tạo và apply migration VerificationTokens | Nguyen Phan Huy | `PlayCourt.Infrastructure/Data/Migrations/20260603081015_AddVerificationTokenTable.cs` | `dotnet ef database update` succeeded |
| 6 | Thêm test Verify Email và Resend Verify Email controller | Nguyen Phan Huy | `PlayCourt.ApiTests/AuthControllerTests.cs` | Tests passed |
| 7 | Thêm test Forgot/Reset/Change Password controller | Nguyen Phan Huy | `PlayCourt.ApiTests/AuthControllerTests.cs` | 19/19 tests passed |
| 8 | Thêm test User Profile controller | Nguyen Phan Huy | `PlayCourt.ApiTests/UsersControllerTests.cs` | 26/26 tests passed |
| 9 | Thêm test Sport Management controller và service | Nguyen Phan Huy | `PlayCourt.ApiTests/SportsControllerTests.cs`, `PlayCourt.ApiTests/SportServiceTests.cs` | 39/39 tests passed |
| 10 | Thêm test CRUD PlayerSport ở controller và service | Phan Thanh Vuong | `PlayCourt.ApiTests/UsersControllerTests.cs`, `PlayCourt.ApiTests/UserServicePlayerSportTests.cs` | 66/66 tests passed; build 0 warning, 0 error |
| 11 | Khởi chạy live test và kiểm thử Swagger JSON endpoints cho Review và VenueStaff | Phan Quoc Khanh | `PlayCourt.API/Controllers/ReviewsController.cs`, `PlayCourt.API/Controllers/VenueStaffsController.cs` | Server live output 200 OK |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
AI hỗ trợ viết test đơn giản cho controller/service và phân tích lỗi, nhóm tự kiểm tra lại bằng build/test.
```

## Commit/Screenshot minh chứng

```text
Kết quả gần nhất: `dotnet build PlayCourt.sln --no-restore` passed với 0 warning, 0 error; `dotnet test PlayCourt.sln --no-restore` passed 66/66 tests.
```

## Ghi chú

```text
Testing hiện tập trung vào smoke test và controller test, chưa có integration test SQL Server riêng.
```

---

# [Phase 06] Hoàn thiện báo cáo và demo

## Ngày thực hiện

```text
03/06/2026
```

## Đã hoàn thành

- [ ] Hoàn thiện source code
- [ ] Hoàn thiện README.md
- [ ] Hoàn thiện report
- [ ] Hoàn thiện slide
- [ ] Hoàn thiện video demo
- [ ] Kiểm tra lại `AI_AUDIT_LOG.md`
- [ ] Kiểm tra lại `PROMPTS.md`
- [ ] Hoàn thiện `REFLECTION.md`
- [ ] Kiểm tra lại `CHANGELOG.md`
- [ ] Đóng gói bài nộp

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 |  |  |  |  |
| 2 |  |  |  |  |
| 3 |  |  |  |  |

## AI có hỗ trợ không?

- [ ] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
Viết sau khi hoàn thiện báo cáo cuối project.
```

## Commit/Screenshot minh chứng

```text
Chưa cập nhật trong giai đoạn này.
```

## Ghi chú

```text
Chưa ghi nhận nội dung cho phase này.
```

---

# 4. Tổng kết thay đổi cuối project

## 4.1. Các chức năng đã hoàn thành

| STT | Chức năng | Trạng thái | Minh chứng | Ghi chú |
|---:|---|---|---|---|
| 1 | Khởi tạo project và docs | Completed | `README.md`, `docs/` | Hoàn thành nền tảng ban đầu |
| 2 | Domain model và DbContext | Completed | `PlayCourt.Domain`, `PlayCourt.Infrastructure/Data` | Đã có entity, enum, migration |
| 3 | Clean Architecture foundation | Completed | `Program.cs`, DI, middleware | Program.cs gọn hơn |
| 4 | Register API | Completed | `AuthController`, `AuthService` | Player/Owner register |
| 5 | Login API | Completed | `AuthController`, `AuthService`, `JwtTokenService` | Email/phone login và JWT |
| 6 | Email OTP infrastructure | Completed | `VerificationToken`, `VerificationTokenService`, migration | Dùng cho verify email/reset password sau này |
| 7 | Verify Email API | Completed | `AuthController`, `AuthService`, `SmtpEmailService` | Verify/resend OTP qua email |
| 8 | Password Management API | Completed | `AuthController`, `AuthService`, `SmtpEmailService` | Forgot/reset/change password |
| 9 | User Profile API | Completed | `UsersController`, `UserService`, `UsersControllerTests` | GET/PUT `/api/users/me` |
| 10 | Sport Management API | Completed | `SportsController`, `SportService`, `SportServiceTests` | Quản lý danh mục môn thể thao |
| 11 | Venue Management API | Completed | `VenuesController`, `VenueService`, `DTOs/Venues` | POST/GET/PUT Venue cho CourtOwner |
| 12 | Court Management API và DTOs/PricingRules | Completed | `CourtsController`, `CourtService`, `ICourtService`, `DTOs/Courts/`, `DTOs/PricingRules/` | DE180313 — POST/GET/PUT Court |
| 13 | Pricing Rule API | Completed | `PricingRulesController`, `PricingRuleService`, `IPricingRuleService` | DE180313 — CRUD cho bảng giá giờ |
| 14 | Court Schedule API | Completed | `CourtSchedulesController`, `CourtScheduleService`, `ICourtScheduleService`, `DTOs/CourtSchedules/` | DE180313 — CRUD quản lý lịch khóa sân |
| 15 | PlayerSport CRUD | Completed | `UsersController`, `IUserService`, `UserService`, `UserServicePlayerSportTests` | DE180405 — GET/POST/PUT/DELETE `/api/users/me/sports` |
| 16 | CourtOwner Approval | Completed | `CourtOwnersController`, `CourtOwnerService`, migration | Admin list/detail/approve/reject; chỉ owner Approved tạo Venue |
| 17 | Hoàn thiện Venue Module | Completed | `VenuesController`, `AdminVenuesController`, `AmenitiesController`, `VenueService`... | DE180310 — Public Discovery, Admin Approval, Images, Amenities, Opening Hours |
| 18 | Review & VenueStaff Module | Completed | `ReviewsController`, `VenueStaffsController`, `ReviewService`, `VenueStaffService` | DE180310 — 15 APIs mới hỗ trợ đánh giá sân và quản lý nhân viên sân |
| 19 | Notification API | Completed | `NotificationsController`, `NotificationService`, `NotificationWriter` | DE180519 — User notification inbox và tích hợp thông báo vào Booking/Payment/Match/CourtOwner |

---

## 4.2. Các chức năng chưa hoàn thành

| STT | Chức năng | Lý do chưa hoàn thành | Hướng cải thiện |
|---:|---|---|---|
| 1 | Frontend hoàn chỉnh | Chưa nằm trong scope hiện tại | Làm sau khi backend ổn định |
| 2 | Integration test SQL Server | Cần database test riêng | Tạo test database hoặc container SQL Server |
| 3 | Integration test cho Password Reset | Hiện mới có controller test | Bổ sung test database SQL Server riêng |

---

## 4.3. Tổng hợp AI hỗ trợ trong project

| Hạng mục | AI có hỗ trợ không? | Mức độ hỗ trợ | Ghi chú |
|---|---|---|---|
| Requirement | Có | Trung bình | Tóm tắt yêu cầu và role |
| Design | Có | Nhiều | Gợi ý layer và flow |
| Database | Có | Nhiều | Entity, DbContext, migration |
| Coding | Có | Nhiều | Backend foundation, Register API, Login API, Email OTP, Verify Email, Password Management, User Profile API, Sport Management API, Venue Management API, Court Management API, Pricing Rule API, Court Schedule API và PlayerSport CRUD |
| Debug | Có | Trung bình | Kiểm tra lỗi package, build, validation |
| Testing | Có | Ít | Smoke test, controller test, JWT claim test, verify/resend test, password management test, user profile test, sport management test, PlayerSport CRUD test và migration update |
| Report | Có | Trung bình | Hoàn thiện docs ngắn gọn |
| Presentation | Không | Ít | Chưa thực hiện |

---

## 4.4. Bài học rút ra

```text
AI giúp làm nhanh hơn nhưng vẫn cần kiểm tra lại bằng build, test và review code.
```

---

## 4.5. Hướng cải thiện tiếp theo

```text
Cần bổ sung refresh token, integration test SQL Server và kiểm thử thủ công flow email thực tế.
```

---

# 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyen Phan Huy | 03/06/2026 |

---

## 28/06/2026 - Player Matching API (DE190946)

### Added

- Match discovery with sport, skill, location, time, status, and pagination filters.
- Personalized match recommendations based on player sports, skill compatibility, and city.
- Match creation, detail, update, cancellation, and participant leave flows.
- Join-request submission, cancellation, host review, approval, and rejection.
- Candidate-player ranking and invitation send/list/accept/decline flows.
- Player-only authorization and server-side ownership, capacity, time, court, sport, and skill validation.
- Nine matching service tests, including the SQL unique-index re-request scenario.

### Verification

- `dotnet build PlayCourt.sln --no-restore`: succeeded with 0 warnings and 0 errors.
- `dotnet test PlayCourt.sln --no-build --no-restore`: 75/75 tests passed.

---

## Cap nhat ngay 28/06/2026 - DE180405 Booking Management

### Da hoan thanh

- [x] Them Booking Management API cho branch `feature/de180405-booking-management`.
- [x] Them DTO, service interface, service implementation va controller cho Booking.
- [x] Them logic tao booking, xem chi tiet, xem booking cua player, xem booking theo venue/court cho owner, check availability, cancel/confirm/reject/complete.
- [x] Ghi `BookingStatusHistory` khi tao booking va khi doi trang thai.
- [x] Tinh gia booking tu `PricingRule`, platform fee 5%, owner earnings bang phan con lai.
- [x] Kiem tra trung lich voi booking dang active, court schedule va match dang open/full.

### File lien quan

```text
PlayCourt.API/Controllers/BookingsController.cs
PlayCourt.Application/DTOs/Bookings/BookingDtos.cs
PlayCourt.Application/Interfaces/IBookingService.cs
PlayCourt.Infrastructure/Services/BookingService.cs
PlayCourt.Infrastructure/DependencyInjection.cs
```

### Kiem chung

```text
dotnet build PlayCourt.sln --no-restore
Ket qua: Build succeeded, 0 warning, 0 error.
Ghi chu: Khong viet/chay test moi theo yeu cau cua thanh vien phu trach.
```

---

## Cap nhat ngay 30/06/2026 - DE180405 PayOS Payment

### Da hoan thanh

- [x] Them Payment API cho branch `feature/de180405-payment`.
- [x] Them DTO, service interface, PayOS gateway adapter, service implementation va controller cho Payment.
- [x] Tich hop package `payOS` de tao payment link, doc trang thai payment link va verify webhook.
- [x] Ho tro local development bang flow `returnUrl` + endpoint `sync-payos`, vi PayOS khong goi duoc webhook vao localhost.
- [x] Van giu endpoint webhook `POST /api/payments/payos/webhook` cho moi truong deploy/production.
- [x] Luu giao dich vao bang `Payments` co san voi provider `payOS`, orderCode trong `TransactionCode`, raw payload trong `ProviderPayload`.
- [x] Them unit tests cho tao payment link, chan user khac thanh toan booking, sync thanh cong, idempotent sync, xem lich su payment va webhook success.

### File lien quan

```text
PlayCourt.API/Controllers/PaymentsController.cs
PlayCourt.Application/DTOs/Payments/PaymentDtos.cs
PlayCourt.Application/Interfaces/IPaymentService.cs
PlayCourt.Application/Interfaces/IPayOsGateway.cs
PlayCourt.Application/Settings/PayOsSettings.cs
PlayCourt.Infrastructure/Services/PaymentService.cs
PlayCourt.Infrastructure/Services/PayOsGateway.cs
PlayCourt.Infrastructure/DependencyInjection.cs
PlayCourt.Infrastructure/PlayCourt.Infrastructure.csproj
PlayCourt.ApiTests/PaymentServiceTests.cs
PlayCourt.API/appsettings.json
PlayCourt.API/appsettings.Development.example.json
```

### Kiem chung

```text
dotnet test PlayCourt.sln
Ket qua: Passed, 92/92 tests.
Ghi chu: Local database can apply migration AddCourtOwnerRejectionReason before manual booking test neu gap loi cot RejectionReason.
```

---

## Cap nhat ngay 30/06/2026 - DE180519 Notification API

### Da hoan thanh

- [x] Them Notification API cho branch `feature/de180519-notification`.
- [x] Them DTO, service interface, writer noi bo, service implementation va controller cho Notification.
- [x] Ho tro user xem danh sach thong bao co paging/filter, xem unread count, mark read, mark all read va delete.
- [x] Tich hop tao notification vao Booking, Payment, Match va CourtOwner approval/rejection.
- [x] Giu notification theo dung user dang dang nhap, khong expose endpoint tao notification cong khai.
- [x] Payment success notification co check idempotent de tranh tao trung khi sync/webhook lap lai.

### File lien quan

```text
PlayCourt.API/Controllers/NotificationsController.cs
PlayCourt.Application/DTOs/Notifications/NotificationDtos.cs
PlayCourt.Application/Interfaces/INotificationService.cs
PlayCourt.Application/Interfaces/INotificationWriter.cs
PlayCourt.Infrastructure/Services/NotificationService.cs
PlayCourt.Infrastructure/Services/NotificationWriter.cs
PlayCourt.Infrastructure/Services/BookingService.cs
PlayCourt.Infrastructure/Services/PaymentService.cs
PlayCourt.Infrastructure/Services/MatchService.cs
PlayCourt.Infrastructure/Services/CourtOwnerService.cs
PlayCourt.Infrastructure/DependencyInjection.cs
```

### Kiem chung

```text
Commit fe5da07 da ghi nhan feature notification. Docs cap nhat bo sung sau commit va khong sua docs/REFLECTION.md.
```

---

## Cap nhat ngay 01/07/2026 - DE180405 PayOS Pending Booking Expiration

### Da hoan thanh

- [x] Them `BookingStatus.Expired` de bieu dien booking qua han thanh toan.
- [x] Them cau hinh `BookingExpiration` gom timeout Pending payment, scan interval va batch size.
- [x] Them `IBookingExpirationService`, `BookingExpirationService` va `BookingExpirationWorker`.
- [x] Worker tu dong tim Booking Pending qua timeout cau hinh, chuyen sang Expired va ghi `BookingStatusHistory`.
- [x] Payment PayOS Pending gan voi booking bi expire duoc danh dau `Failed`.
- [x] Dung conditional update de tranh expire nham booking vua duoc PayOS sync/webhook confirm.
- [x] Them migration `AddExpiredBookingStatus` cap nhat check constraint cho BookingStatus va BookingStatusHistory.
- [x] Them test cho expire booking, release court slot va PayOS terminal failure khong confirm booking.

### File lien quan

```text
PlayCourt.Application/Settings/BookingExpirationSettings.cs
PlayCourt.Application/Interfaces/IBookingExpirationService.cs
PlayCourt.Infrastructure/Services/BookingExpirationService.cs
PlayCourt.Infrastructure/Services/BookingExpirationWorker.cs
PlayCourt.Domain/Enums/DomainEnums.cs
PlayCourt.Domain/Entities/Booking.cs
PlayCourt.Infrastructure/Data/PlayCourtDbContext.cs
PlayCourt.Infrastructure/Data/Migrations/20260701074643_AddExpiredBookingStatus.cs
PlayCourt.Infrastructure/DependencyInjection.cs
PlayCourt.API/appsettings.json
PlayCourt.API/appsettings.Development.example.json
PlayCourt.ApiTests/BookingExpirationServiceTests.cs
PlayCourt.ApiTests/BookingServiceTests.cs
PlayCourt.ApiTests/PaymentServiceTests.cs
```

### Kiem chung

```text
dotnet format PlayCourt.sln --verify-no-changes
Ket qua: Passed.

dotnet build PlayCourt.sln
Ket qua: Build succeeded, 0 warning, 0 error.

dotnet test PlayCourt.sln
Ket qua: Passed, 88/88 tests.

Ghi chu: Khong sua docs/REFLECTION.md; khong commit plan trong docs/superpowers/plans.
```
