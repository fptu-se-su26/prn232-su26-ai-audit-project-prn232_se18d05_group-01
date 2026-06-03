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
| Phase 05 | 22/06/2026 - 28/06/2026 | Testing & Debug | Not Started |
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
- [ ] Xử lý CRUD
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

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
AI hỗ trợ đề xuất cách setup layer, gom DependencyInjection theo API/Application/Infrastructure, thêm middleware exception và common ApiResponse. Nhóm tự review dependency direction, scope thay đổi và chạy kiểm chứng trước khi commit.
```

## Commit/Screenshot minh chứng

```text
Commit sẽ cập nhật sau khi hoàn tất.
```

## Ghi chú

```text
Thay đổi này gồm backend foundation, Register API, Login API, Email OTP infrastructure và Verify Email. Kết quả kiểm chứng: dotnet build passed và dotnet test passed.
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

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Thêm test controller cho Register success/fail | Nguyen Phan Huy | `PlayCourt.ApiTests/AuthControllerTests.cs` | `dotnet test PlayCourt.sln --no-build` |
| 2 | Chạy build toàn solution | Nguyen Phan Huy | `PlayCourt.sln` | Build passed |
| 3 | Chạy test toàn solution | Nguyen Phan Huy | `PlayCourt.ApiTests` | 12/12 tests passed |
| 4 | Thêm test Login controller và JWT role claim | Nguyen Phan Huy | `PlayCourt.ApiTests/AuthControllerTests.cs`, `PlayCourt.ApiTests/JwtTokenServiceTests.cs` | Tests passed |
| 5 | Tạo và apply migration VerificationTokens | Nguyen Phan Huy | `PlayCourt.Infrastructure/Data/Migrations/20260603081015_AddVerificationTokenTable.cs` | `dotnet ef database update` succeeded |
| 6 | Thêm test Verify Email và Resend Verify Email controller | Nguyen Phan Huy | `PlayCourt.ApiTests/AuthControllerTests.cs` | Tests passed |

## AI có hỗ trợ không?

- [x] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
AI hỗ trợ viết test đơn giản và phân tích lỗi, nhóm tự kiểm tra lại bằng build/test.
```

## Commit/Screenshot minh chứng

```text
Kết quả: dotnet build PlayCourt.sln passed, dotnet test PlayCourt.sln --no-build passed.
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
| 8 | Test cơ bản | Partial | `PlayCourt.ApiTests` | Chưa có integration test SQL Server cho OTP service |

---

## 4.2. Các chức năng chưa hoàn thành

| STT | Chức năng | Lý do chưa hoàn thành | Hướng cải thiện |
|---:|---|---|---|
| 1 | Frontend hoàn chỉnh | Chưa nằm trong scope hiện tại | Làm sau khi backend ổn định |
| 2 | Integration test SQL Server | Cần database test riêng | Tạo test database hoặc container SQL Server |
| 3 | Forgot Password endpoint | Hiện mới có OTP infrastructure và verify email | Triển khai ở task tiếp theo |

---

## 4.3. Tổng hợp AI hỗ trợ trong project

| Hạng mục | AI có hỗ trợ không? | Mức độ hỗ trợ | Ghi chú |
|---|---|---|---|
| Requirement | Có | Trung bình | Tóm tắt yêu cầu và role |
| Design | Có | Nhiều | Gợi ý layer và flow |
| Database | Có | Nhiều | Entity, DbContext, migration |
| Coding | Có | Nhiều | Backend foundation, Register API, Login API, Email OTP infrastructure và Verify Email |
| Debug | Có | Trung bình | Kiểm tra lỗi package, build, validation |
| Testing | Có | Ít | Smoke test, controller test, JWT claim test, verify/resend test và migration update |
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
Cần bổ sung refresh token, Forgot Password endpoint và integration test SQL Server.
```

---

# 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyen Phan Huy | 03/06/2026 |
