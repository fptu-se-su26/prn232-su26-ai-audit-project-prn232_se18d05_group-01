# Prompt Log

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
| Ngày cập nhật gần nhất | 04/06/2026 |

---

## 2. Mục đích của file Prompt Log

File này dùng để ghi lại các prompt quan trọng đã sử dụng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

Sinh viên/nhóm cần ghi lại:

- Đã hỏi AI điều gì.
- Mục đích sử dụng prompt.
- Công cụ AI đã sử dụng.
- AI đã trả lời hoặc gợi ý gì.
- Kết quả đó có được áp dụng vào bài hay không.
- Sinh viên/nhóm đã kiểm tra, chỉnh sửa hoặc cải tiến gì sau khi nhận kết quả từ AI.

---

## 3. Công cụ AI đã sử dụng

Đánh dấu các công cụ AI đã sử dụng.

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

## 4. Bảng tổng hợp prompt đã sử dụng

| STT | Ngày | Công cụ AI | Mục đích | Prompt tóm tắt | Kết quả chính | Có sử dụng vào bài không? | Minh chứng |
|---:|---|---|---|---|---|---|---|
| 1 | 14/05/2026 | Codex | Cập nhật tài liệu ban đầu | Điền thông tin project, ngày bắt đầu/kết thúc và danh sách công cụ AI | README và tài liệu trong `docs/` được cập nhật thông tin nền ban đầu | Có | `README.md`, `docs/` |
| 2 | 21/05/2026 | Codex | Hướng dẫn tạo model và DbContext | Tạo entity model, enum, DbContext, relationship, index, constraint và migration cho PlayCourt | Hoàn thiện model, DbContext và migration EF Core | Có | `PlayCourt.Domain/`, `PlayCourt.Infrastructure/Data/` |
| 3 | 21/05/2026 | Codex | Setup application layer | Tách DI theo layer, thêm middleware exception, ApiResponse và service placeholder | Program.cs gọn hơn, các layer có extension registration riêng và có response/error shape dùng chung | Có | `PlayCourt.API/`, `PlayCourt.Application/`, `PlayCourt.Infrastructure/` |
| 4 | 03/06/2026 | Codex | Triển khai Register API | Tạo API đăng ký tài khoản theo Clean Architecture | Có endpoint register, service, DTO, validation và BCrypt | Có | `PlayCourt.API/Controllers/AuthController.cs`, `PlayCourt.Infrastructure/Services/AuthService.cs` |
| 5 | 03/06/2026 | Codex | Triển khai Login API | Tạo API đăng nhập bằng email/số điện thoại và trả JWT | Có endpoint login, JWT service, cấu hình bearer auth và test | Có | `PlayCourt.API/Controllers/AuthController.cs`, `PlayCourt.Infrastructure/Services/JwtTokenService.cs` |
| 6 | 03/06/2026 | Codex | Triển khai Email OTP infrastructure | Tạo VerificationToken table, OTP service và dev email logger | Có entity, enum, migration, service OTP và email logging | Có | `VerificationToken.cs`, `VerificationTokenService.cs`, migration `AddVerificationTokenTable` |
| 7 | 03/06/2026 | Codex | Triển khai Verify Email | Tạo verify/resend OTP endpoints và SMTP email service | Register gửi OTP, verify email, resend OTP, MailKit SMTP | Có | `AuthController.cs`, `AuthService.cs`, `SmtpEmailService.cs` |
| 8 | 03/06/2026 | Codex | Triển khai Password Management | Tạo forgot/reset/change password endpoints | Dùng lại OTP PasswordReset, SMTP email và BCrypt | Có | `AuthController.cs`, `AuthService.cs`, `SmtpEmailService.cs` |
| 9 | 03/06/2026 | Codex | Triển khai User Profile API | Tạo GET/PUT `/api/users/me` tách riêng AuthService | Có UsersController, UserService, DTO, validation và test | Có | `UsersController.cs`, `UserService.cs`, `UsersControllerTests.cs` |
| 10 | 04/06/2026 | Codex | Triển khai Sport Management API | Tạo API quản lý môn thể thao, admin create/update/toggle active, validate code/name/player count và thêm test | Có SportsController, DTO, ISportService, SportService, controller test và service test | Có | `SportsController.cs`, `SportService.cs`, `SportServiceTests.cs` |

---

## 5. Prompt chi tiết

> Sinh viên/nhóm có thể nhân bản mẫu “Prompt số...” nhiều lần tùy số lượng prompt thực tế đã sử dụng.

---

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 14/05/2026 |
| Công cụ AI | Codex |
| Mục đích | Cập nhật tài liệu ban đầu cho project |
| Phần việc liên quan | Report / Documentation / Other |
| Mức độ sử dụng | Hỏi chỉnh sửa tài liệu |

#### 5.1. Prompt nguyên văn

```text
Điền thông tin cơ sở ban đầu cho các file trong docs, ngày bắt đầu 11/05/2026, project kéo dài 8 tuần, công cụ AI chắc chắn dùng gồm ChatGPT, Gemini, Antigravity, Codex, GitHub Copilot, Cursor, Claude. Các mục reflection sẽ viết sau khi kết thúc project.
```

#### 5.2. Bối cảnh khi viết prompt

Mô tả ngắn gọn vì sao sinh viên/nhóm cần dùng prompt này.

```text
Nhóm cần chuẩn hóa tài liệu audit ban đầu trước khi bắt đầu ghi log chi tiết trong quá trình thực hiện project.
```

#### 5.3. Kết quả AI trả về

Tóm tắt nội dung AI đã trả lời hoặc gợi ý.

```text
AI gợi ý điền thông tin project, tính ngày kết thúc sau 8 tuần, đánh dấu các công cụ AI dự kiến sử dụng và để các mục reflection ở trạng thái cập nhật sau.
```

#### 5.4. Kết quả đã áp dụng vào bài

Mô tả phần nào từ kết quả AI đã được sử dụng vào bài tập/project.

```text
Thông tin project, timeline, danh sách công cụ AI và ghi chú cập nhật sau được áp dụng vào các file tài liệu.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

Mô tả sinh viên/nhóm đã thay đổi, kiểm tra, sửa lỗi hoặc cải tiến gì so với kết quả AI trả về.

```text
Nhóm kiểm tra lại ngày tháng, tên project, danh sách công cụ và giữ các phần đánh giá cuối kỳ để cập nhật sau khi project kết thúc.
```

#### 5.6. Đánh giá chất lượng prompt

Đánh dấu các nhận xét phù hợp.

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [x] Cần tự kiểm tra và chỉnh sửa nhiều
- [ ] Kết quả AI có lỗi hoặc chưa chính xác

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Sẽ cập nhật sau khi commit |
| File liên quan | `README.md`, `docs/AI_AUDIT_LOG.md`, `docs/PROMPTS.md`, `docs/REFLECTION.md`, `docs/CHANGELOG.md` |
| Screenshot |  |
| Kết quả chạy/test |  |
| Link tài liệu/báo cáo | `docs/` |
| Ghi chú khác | Chỉ cập nhật thông tin nền ban đầu |

#### 5.8. Ghi chú thêm

```text
Các prompt tiếp theo sẽ được bổ sung trong quá trình làm project.
```

---

### Prompt số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 21/05/2026 |
| Công cụ AI | Codex |
| Mục đích | Hướng dẫn tạo domain model và DbContext |
| Phần việc liên quan | Database / Coding / Debug |
| Mức độ sử dụng | Hỏi giải thích / Hỏi review / Hỏi sinh code mẫu |

#### 5.1. Prompt nguyên văn

```text
Hãy hướng dẫn tôi tạo các entity model và cấu hình DbContext cho hệ thống đặt sân thể thao PlayCourt bằng ASP.NET Core và EF Core. Cần có DbSet, relationship, navigation properties, enum thay magic number, soft-delete query filter, index, check constraint và migration.
```

#### 5.2. Bối cảnh khi viết prompt

```text
Nhóm cần xây dựng tầng Domain và Infrastructure cho PlayCourt API, trong đó database cần có entity model rõ ràng và DbContext đủ cấu hình để chạy migration.
```

#### 5.3. Kết quả AI trả về

```text
AI gợi ý cấu trúc entity, enum, navigation properties, cấu hình Fluent API trong DbContext, soft-delete query filter, check constraints, index và migration.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Nhóm áp dụng để tạo/cập nhật entity model, enum, DbContext và migration cho database.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Nhóm tự kiểm tra lại bằng code review, dotnet format, dotnet build, dotnet ef migrations has-pending-model-changes và dotnet test. Một số cấu hình được chỉnh lại để tránh duplicate index và cảnh báo query filter của EF Core.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [x] Cần hỏi lại AI nhiều lần
- [x] Cần tự kiểm tra và chỉnh sửa nhiều
- [ ] Kết quả AI có lỗi hoặc chưa chính xác

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Sẽ cập nhật sau khi commit |
| File liên quan | `PlayCourt.Domain/Entities/`, `PlayCourt.Domain/Enums/DomainEnums.cs`, `PlayCourt.Infrastructure/Data/PlayCourtDbContext.cs`, `PlayCourt.Infrastructure/Data/Migrations/` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet build PlayCourt.sln --no-restore`, `dotnet test PlayCourt.sln --no-build` |
| Link tài liệu/báo cáo | `docs/AI_AUDIT_LOG.md`, `docs/PROMPTS.md`, `docs/CHANGELOG.md`, `docs/REFLECTION.md` |
| Ghi chú khác | Nội dung AI hỗ trợ tập trung vào model và DbContext |

#### 5.8. Ghi chú thêm

```text
Prompt này được ghi nhận vì ảnh hưởng trực tiếp đến phần model và database configuration của project.
```

---

### Prompt số 3

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 21/05/2026 |
| Công cụ AI | Codex |
| Mục đích | Setup application layer và dependency injection foundation |
| Phần việc liên quan | Design / Coding / Testing / Debug |
| Mức độ sử dụng | Hỏi ý tưởng / Hỏi review / Hỏi sinh code mẫu |

#### 5.1. Prompt nguyên văn

```text
Tôi muốn setup application layer cho project PlayCourt API. Hãy đề xuất cách tổ chức theo từng layer để Program.cs ngắn gọn hơn, API có DependencyInjection và middleware xử lý exception, Application có common response/interface/DTO placeholder, Infrastructure đăng ký DbContext và service implementation. Sau đó review diff so với nhánh dev, chỉ commit phần code liên quan và kiểm tra format/build/test.
```

#### 5.2. Bối cảnh khi viết prompt

```text
Sau khi có Domain và Infrastructure nền tảng, nhóm cần tiếp tục chuẩn hóa Application layer để các feature sau có nơi đặt DTO, interface, service contract và response shape dùng chung.
```

#### 5.3. Kết quả AI trả về

```text
AI gợi ý tạo extension method AddApiServices, UseApiPipeline, AddApplicationServices và AddInfrastructureServices; thêm ExceptionHandlingMiddleware; thêm ApiResponse<T>; tạo IService/Service placeholder; xoá Class1.cs mặc định và cập nhật package reference cần thiết.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Nhóm áp dụng các phần liên quan đến setup layer, gom registration/pipeline khỏi Program.cs và giữ các class placeholder tối thiểu để các thành viên khác dễ mở rộng.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Nhóm kiểm tra lại dependency direction giữa các layer, loại bỏ test không phù hợp với contract hiện tại, chạy dotnet format, dotnet build và dotnet test trước khi chuẩn bị commit.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [x] Cần hỏi lại AI nhiều lần
- [x] Cần tự kiểm tra và chỉnh sửa nhiều
- [ ] Kết quả AI có lỗi hoặc chưa chính xác

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Sẽ cập nhật sau khi commit |
| File liên quan | `PlayCourt.API/Program.cs`, `PlayCourt.API/DependencyInjection.cs`, `PlayCourt.API/Middlewares/ExceptionHandlingMiddleware.cs`, `PlayCourt.Application/Common/Responses/ApiResponse.cs`, `PlayCourt.Application/DependencyInjection.cs`, `PlayCourt.Application/Interfaces/IService.cs`, `PlayCourt.Infrastructure/DependencyInjection.cs`, `PlayCourt.Infrastructure/Services/Service.cs` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet format PlayCourt.sln --verify-no-changes --no-restore` passed; `dotnet build PlayCourt.sln` passed; `dotnet test PlayCourt.sln` passed 3/3 tests |
| Link tài liệu/báo cáo | `docs/AI_AUDIT_LOG.md`, `docs/PROMPTS.md`, `docs/CHANGELOG.md`, `docs/REFLECTION.md` |
| Ghi chú khác | Nội dung tập trung vào setup foundation, chưa triển khai nghiệp vụ cụ thể |

#### 5.8. Ghi chú thêm

```text
Prompt này được ghi nhận vì ảnh hưởng trực tiếp đến cấu trúc Application/API/Infrastructure layer và cách team mở rộng service sau này.
```

---

### Prompt số 4

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 03/06/2026 |
| Công cụ AI | Codex |
| Mục đích | Triển khai Register API |
| Phần việc liên quan | Backend / Testing |
| Mức độ sử dụng | Hỏi sinh code mẫu / Hỏi review |

#### 5.1. Prompt nguyên văn

```text
Implement Register API cho PlayCourt Backend. Frontend gửi fullName, email, phoneNumber, password, role và businessName. Dùng ApiResponse<T>, BCrypt, Clean Architecture, tạo User, UserProfile và CourtOwnerProfile nếu Owner.
```

#### 5.2. Bối cảnh khi viết prompt

```text
Project cần endpoint đăng ký tài khoản để Player và Owner có thể tạo tài khoản mới.
```

#### 5.3. Kết quả AI trả về

```text
AI gợi ý tạo DTO, service interface, service implementation, controller, DI registration và kiểm tra build/test.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Nhóm áp dụng để triển khai endpoint POST /api/auth/register, validate input, hash password và lưu dữ liệu vào SQL Server qua EF Core.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Nhóm chỉnh lại theo entity thật của project, không dùng SQLite test package và đảm bảo controller không chứa business logic.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [x] Cần tự kiểm tra và chỉnh sửa nhiều

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Sẽ cập nhật sau khi commit |
| File liên quan | `PlayCourt.API/Controllers/AuthController.cs`, `PlayCourt.Application/DTOs/Auth/`, `PlayCourt.Infrastructure/Services/AuthService.cs` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet build PlayCourt.sln`, `dotnet test PlayCourt.sln --no-build` |
| Link tài liệu/báo cáo | `docs/AI_AUDIT_LOG.md`, `docs/PROMPTS.md`, `docs/CHANGELOG.md` |
| Ghi chú khác | Register API dùng SQL Server và BCrypt |

#### 5.8. Ghi chú thêm

```text
Prompt này giúp nhóm hoàn thành một chức năng backend quan trọng của hệ thống.
```

---

### Prompt số 5

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 03/06/2026 |
| Công cụ AI | Codex |
| Mục đích | Triển khai Login API với JWT |
| Phần việc liên quan | Backend / Testing |
| Mức độ sử dụng | Hỏi sinh code mẫu / Hỏi review |

#### 5.1. Prompt nguyên văn

```text
Implement Login API with JWT cho PlayCourt backend. Login bằng email hoặc số điện thoại, verify password bằng BCrypt, chỉ cho user Active đăng nhập, trả JWT access token, không thêm LastLoginAt, không thêm migration.
```

#### 5.2. Bối cảnh khi viết prompt

```text
Sau khi có Register API, project cần chức năng đăng nhập để frontend nhận token và gọi các API cần phân quyền.
```

#### 5.3. Kết quả AI trả về

```text
AI gợi ý tạo LoginRequestDto, LoginResponseDto, IJwtTokenService, JwtTokenService, cập nhật AuthService, AuthController, cấu hình JWT và thêm test.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Nhóm áp dụng để thêm endpoint POST /api/auth/login, sinh JWT token, kiểm tra role claim và cấu hình middleware authentication.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Nhóm kiểm tra lại package JWT, middleware order, role claim và đảm bảo không thay đổi database/migration.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [x] Prompt nêu rõ ràng buộc
- [x] Prompt tạo ra kết quả tốt
- [x] Cần tự kiểm tra và chỉnh sửa

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Sẽ cập nhật sau khi commit |
| File liên quan | `PlayCourt.API/Controllers/AuthController.cs`, `PlayCourt.Application/DTOs/Auth/LoginRequestDto.cs`, `PlayCourt.Application/DTOs/Auth/LoginResponseDto.cs`, `PlayCourt.Infrastructure/Services/JwtTokenService.cs` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet build PlayCourt.sln`, `dotnet test PlayCourt.sln --no-build` |
| Link tài liệu/báo cáo | `docs/AI_AUDIT_LOG.md`, `docs/PROMPTS.md`, `docs/CHANGELOG.md` |
| Ghi chú khác | Không thêm migration và không thêm LastLoginAt |

#### 5.8. Ghi chú thêm

```text
Prompt này được ghi nhận vì Login API là chức năng chính của phần authentication.
```

---

### Prompt số 6

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 03/06/2026 |
| Công cụ AI | Codex |
| Mục đích | Triển khai Email OTP infrastructure |
| Phần việc liên quan | Backend / Database / Testing |
| Mức độ sử dụng | Hỏi sinh code mẫu / Hỏi review |

#### 5.1. Prompt nguyên văn

```text
Implement Email/OTP infrastructure cho PlayCourt Backend bằng EF Core Code First. Thêm VerificationTokenPurpose enum, VerificationToken entity, relationship với User, DbContext config, migration, OTP service và development email service log bằng ILogger.
```

#### 5.2. Bối cảnh khi viết prompt

```text
Project cần hạ tầng OTP để dùng lại cho verify email và forgot/reset password ở các task sau.
```

#### 5.3. Kết quả AI trả về

```text
AI gợi ý tạo bảng VerificationTokens, thêm enum purpose, thêm interface/service tạo OTP, verify OTP và development email service không gửi email thật.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Nhóm áp dụng để thêm entity, DbSet, Fluent API config, migration AddVerificationTokenTable, OTP hash bằng BCrypt và DI registration.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Nhóm kiểm tra không đổi database provider, không thêm SMTP package, không thêm endpoint ngoài scope và chạy database update.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [x] Prompt nêu rõ ràng buộc
- [x] Prompt tạo ra kết quả tốt
- [x] Cần tự kiểm tra bằng migration/build/test

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Sẽ cập nhật sau khi commit |
| File liên quan | `PlayCourt.Domain/Entities/VerificationToken.cs`, `PlayCourt.Domain/Enums/DomainEnums.cs`, `PlayCourt.Infrastructure/Data/PlayCourtDbContext.cs`, `PlayCourt.Infrastructure/Services/VerificationTokenService.cs` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet ef migrations add AddVerificationTokenTable`; `dotnet ef database update`; `dotnet build PlayCourt.sln`; `dotnet test PlayCourt.sln --no-build` |
| Link tài liệu/báo cáo | `docs/AI_AUDIT_LOG.md`, `docs/PROMPTS.md`, `docs/CHANGELOG.md` |
| Ghi chú khác | Chỉ làm infrastructure, chưa làm Verify Email/Forgot Password endpoint |

#### 5.8. Ghi chú thêm

```text
Prompt này được ghi nhận vì OTP infrastructure là nền tảng cho các chức năng xác thực email và reset password.
```

---

### Prompt số 7

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 03/06/2026 |
| Công cụ AI | Codex |
| Mục đích | Triển khai Verify Email bằng OTP và SMTP |
| Phần việc liên quan | Backend / Auth / Testing |
| Mức độ sử dụng | Hỏi sinh code mẫu / Hỏi review |

#### 5.1. Prompt nguyên văn

```text
Implement Verify Email feature. Register tạo OTP và gửi email, thêm endpoint /api/auth/verify-email và /api/auth/resend-verify-email, dùng ApiResponse<T>, MailKit SMTP, Gmail app password trong appsettings.Development.json local.
```

#### 5.2. Bối cảnh khi viết prompt

```text
Project đã có Register/Login và OTP infrastructure, cần hoàn thiện flow verify email để frontend có thể xác thực tài khoản người dùng.
```

#### 5.3. Kết quả AI trả về

```text
AI gợi ý thêm VerifyEmailRequestDto, ResendVerifyEmailRequestDto, EmailSettings, SmtpEmailService, cập nhật AuthService/RegisterAsync, thêm VerifyEmailAsync, ResendVerifyEmailAsync và controller tests.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Nhóm áp dụng để register gửi OTP qua SMTP, verify OTP set IsEmailVerified, resend OTP có cooldown 60 giây và cấu hình EmailSettings.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Nhóm giữ credential SMTP trong appsettings.Development.json local, shared appsettings chỉ dùng placeholder và kiểm tra không đưa password thật vào file tracked.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [x] Prompt nêu rõ ràng buộc bảo mật
- [x] Prompt tạo ra kết quả tốt
- [x] Cần tự kiểm tra bằng build/test

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Sẽ cập nhật sau khi commit |
| File liên quan | `PlayCourt.API/Controllers/AuthController.cs`, `PlayCourt.Infrastructure/Services/AuthService.cs`, `PlayCourt.Infrastructure/Services/SmtpEmailService.cs`, `PlayCourt.Application/Settings/EmailSettings.cs` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet build PlayCourt.sln`; `dotnet test PlayCourt.sln --no-build` |
| Link tài liệu/báo cáo | `docs/AI_AUDIT_LOG.md`, `docs/PROMPTS.md`, `docs/CHANGELOG.md` |
| Ghi chú khác | Không ghi SMTP password thật trong docs |

#### 5.8. Ghi chú thêm

```text
Prompt này được ghi nhận vì Verify Email là chức năng auth chính, dùng lại OTP infrastructure đã có.
```

---

### Prompt số 8

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 03/06/2026 |
| Công cụ AI | Codex |
| Mục đích | Triển khai Forgot/Reset/Change Password |
| Phần việc liên quan | Backend / Auth / Testing |
| Mức độ sử dụng | Hỏi sinh code mẫu / Hỏi review |

#### 5.1. Prompt nguyên văn

```text
Implement Forgot Password, Reset Password và Change Password cho PlayCourt. Dùng lại VerificationTokenPurpose.PasswordReset, VerificationTokenService, SMTP email service, BCrypt, endpoint change-password có Authorize và không thêm migration.
```

#### 5.2. Bối cảnh khi viết prompt

```text
Project đã có Register/Login, JWT, Email OTP infrastructure và Verify Email nên cần hoàn thiện flow quản lý mật khẩu.
```

#### 5.3. Kết quả AI trả về

```text
AI gợi ý thêm DTO request, mở rộng IAuthService/AuthService, thêm reset password email template, thêm 3 endpoint trong AuthController và controller tests.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Nhóm áp dụng để thêm `/api/auth/forgot-password`, `/api/auth/reset-password` và `/api/auth/change-password`, dùng OTP PasswordReset và hash password mới bằng BCrypt.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Nhóm giữ SQL Server, không thêm migration, trả message forgot password dạng chung và kiểm tra bằng build/test.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [x] Prompt nêu rõ ràng buộc không thêm migration
- [x] Prompt tạo ra kết quả tốt
- [x] Cần tự kiểm tra bằng build/test

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Sẽ cập nhật sau khi commit |
| File liên quan | `PlayCourt.API/Controllers/AuthController.cs`, `PlayCourt.Application/DTOs/Auth/`, `PlayCourt.Infrastructure/Services/AuthService.cs`, `PlayCourt.Infrastructure/Services/SmtpEmailService.cs` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet build PlayCourt.sln`; `dotnet test PlayCourt.sln --no-build` passed 19/19 |
| Link tài liệu/báo cáo | `docs/AI_AUDIT_LOG.md`, `docs/PROMPTS.md`, `docs/CHANGELOG.md` |
| Ghi chú khác | Không ghi SMTP password thật trong docs |

#### 5.8. Ghi chú thêm

```text
Prompt này được ghi nhận vì Password Management là chức năng auth chính và dùng lại hạ tầng OTP đã có.
```

---

### Prompt số 9

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 03/06/2026 |
| Công cụ AI | Codex |
| Mục đích | Triển khai User Profile API |
| Phần việc liên quan | Coding / Testing / Documentation |
| Mức độ sử dụng | Hỏi sinh code mẫu / Hỏi test case / Hỏi review |

#### 5.1. Prompt nguyên văn

```text
Implement User Profile APIs for PlayCourt API: GET /api/users/me and PUT /api/users/me. Allow authenticated users to get and update safe personal profile fields, keep logic separate from AuthService, use ApiResponse<T>, do not expose PasswordHash, do not update account/court-owner business fields, do not add migration, add controller tests and update docs.
```

#### 5.2. Bối cảnh khi viết prompt

```text
Project đã có User, UserProfile, CourtOwnerProfile, JWT authentication, ClaimTypes.NameIdentifier, ApiResponse<T>, manual DTO mapping và test controller bằng stub service.
```

#### 5.3. Kết quả AI trả về

```text
AI đề xuất tạo DTOs trong PlayCourt.Application/DTOs/Users, IUserService, UserService dùng EF Core, UsersController có Authorize và tests cho success/fail/unauthorized.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Nhóm áp dụng để thêm endpoint GET /api/users/me và PUT /api/users/me, trả profile hiện tại, cập nhật safe fields, đăng ký DI và thêm UsersControllerTests.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Nhóm chỉnh theo entity thật của project, giữ AuthService không đổi, không tạo migration, chỉ validate FullName/Gender trong service và kiểm chứng bằng build/test.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [x] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [x] Cần tự kiểm tra và chỉnh sửa nhiều
- [ ] Kết quả AI có lỗi hoặc chưa chính xác

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Sẽ cập nhật sau khi commit |
| File liên quan | `PlayCourt.API/Controllers/UsersController.cs`, `PlayCourt.Infrastructure/Services/UserService.cs`, `PlayCourt.Application/DTOs/Users/`, `PlayCourt.ApiTests/UsersControllerTests.cs` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet build PlayCourt.sln`; `dotnet test PlayCourt.sln --no-build` |
| Link tài liệu/báo cáo | `docs/CHANGELOG.md`, `docs/AI_AUDIT_LOG.md`, `docs/PROMPTS.md` |
| Ghi chú khác | Không thêm migration và không expose PasswordHash |

#### 5.8. Ghi chú thêm

```text
Prompt này được ghi nhận vì User Profile API là chức năng người dùng đăng nhập quan trọng và có yêu cầu bảo mật field rõ ràng.
```

---

### Prompt số 10

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 04/06/2026 |
| Công cụ AI | Codex |
| Mục đích | Triển khai Sport Management API |
| Phần việc liên quan | Coding / Testing |
| Mức độ sử dụng | Hỏi sinh code mẫu / Hỏi test case / Hỏi review |

#### 5.1. Prompt nguyên văn

```text
Implement Sport Management cho PlayCourt Backend. Tạo API quản lý môn thể thao, dùng ApiResponse<T>, tách logic vào SportService, admin mới được create/update/toggle active, có validate code/name/player count và thêm test.
```

#### 5.2. Bối cảnh khi viết prompt

```text
Project đã có entity Sport, DbContext, ApiResponse<T>, JWT authorization policy và pattern controller/service/test cho các feature trước.
```

#### 5.3. Kết quả AI trả về

```text
AI đề xuất thêm SportsController, DTOs trong PlayCourt.Application/DTOs/Sports, ISportService, SportService dùng EF Core và test cho controller/service.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Nhóm áp dụng để thêm API GET danh sách sport, GET sport theo id, POST tạo sport, PUT cập nhật sport và PATCH toggle active.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Nhóm kiểm tra lại phân quyền admin cho API thay đổi dữ liệu, validate duplicate code/name, chuẩn hóa sport code và chạy test toàn solution.
```

#### 5.6. Đánh giá chất lượng prompt

- [x] Prompt rõ ràng
- [x] Prompt có đủ bối cảnh
- [x] Prompt nêu rõ phân quyền
- [x] Prompt tạo ra kết quả tốt
- [x] Cần tự kiểm tra bằng build/test

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Sẽ cập nhật sau khi commit |
| File liên quan | `PlayCourt.API/Controllers/SportsController.cs`, `PlayCourt.Application/DTOs/Sports/`, `PlayCourt.Application/Interfaces/ISportService.cs`, `PlayCourt.Infrastructure/Services/SportService.cs`, `PlayCourt.ApiTests/SportsControllerTests.cs`, `PlayCourt.ApiTests/SportServiceTests.cs` |
| Screenshot |  |
| Kết quả chạy/test | `dotnet test PlayCourt.sln --no-build` passed 39/39 |
| Link tài liệu/báo cáo | `docs/AI_AUDIT_LOG.md`, `docs/PROMPTS.md`, `docs/CHANGELOG.md` |
| Ghi chú khác | Không thêm migration vì bảng Sport đã có |

#### 5.8. Ghi chú thêm

```text
Prompt này được ghi nhận vì Sport Management là chức năng backend dùng để quản lý danh mục môn thể thao trong hệ thống.
```

---

## 6. Prompt quan trọng nhất

Chọn một prompt có ảnh hưởng lớn nhất đến bài tập/project.

### 6.1. Prompt được chọn

```text
Hãy hướng dẫn tôi tạo các entity model và cấu hình DbContext cho hệ thống đặt sân thể thao PlayCourt bằng ASP.NET Core và EF Core. Cần có DbSet, relationship, navigation properties, enum thay magic number, soft-delete query filter, index, check constraint và migration.
```

### 6.2. Vì sao prompt này quan trọng?

```text
Prompt này quan trọng vì nó định hướng phần Domain model và EF Core DbContext, là nền tảng để tạo database bằng migration.
```

### 6.3. Kết quả prompt này mang lại

```text
Kết quả giúp nhóm hoàn thiện entity model, enum, navigation properties, DbContext configuration và migration.
```

### 6.4. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?

```text
Nhóm kiểm tra bằng format, build, EF pending model changes, test tự động và review lại các constraint/index trong DbContext.
```

### 6.5. Sinh viên/nhóm đã cải tiến gì từ kết quả AI?

```text
Nhóm chỉnh lại connection string template, loại bỏ duplicate index, thêm soft-delete filter phù hợp và đảm bảo migration đồng bộ với model.
```

---

## 7. Prompt chưa hiệu quả

Ghi lại ít nhất một prompt chưa tạo ra kết quả tốt hoặc chưa phù hợp.

### 7.1. Prompt chưa hiệu quả

```text
Viết test bằng SQLite cho Register API.
```

### 7.2. Vì sao prompt này chưa hiệu quả?

```text
Prompt này chưa hiệu quả vì project đang dùng SQL Server, thêm SQLite làm test provider có thể gây lệch công nghệ và thêm package không cần thiết.
```

Gợi ý nguyên nhân:

- Prompt quá ngắn.
- Thiếu bối cảnh bài toán.
- Không nêu rõ yêu cầu đầu ra.
- Không cung cấp ngôn ngữ lập trình/công nghệ đang dùng.
- Không đưa lỗi cụ thể.
- Không đưa ví dụ input/output.
- Không yêu cầu AI giải thích.
- Hỏi AI làm toàn bộ thay vì hỏi từng phần.

### 7.3. Cách cải thiện prompt

```text
Nêu rõ project chỉ dùng SQL Server và nếu viết test thì không thêm database provider khác.
```

### 7.4. Prompt sau khi cải tiến

```text
Hãy viết test đơn giản cho AuthController bằng fake IAuthService, không thêm SQLite hoặc database provider mới.
```

### 7.5. Kết quả sau khi cải tiến prompt

```text
Kết quả phù hợp hơn vì test chỉ kiểm tra HTTP response của controller, không thay đổi công nghệ database của project.
```

---

## 8. Bài học về cách viết prompt

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?

```text
Cần cung cấp mục tiêu, bối cảnh project, công nghệ đang dùng, input/output mong muốn, ràng buộc kiến trúc và cách kiểm chứng.
```

Gợi ý:

- Mục tiêu cần đạt.
- Bối cảnh bài toán.
- Công nghệ/ngôn ngữ lập trình đang dùng.
- Input/output mong muốn.
- Ràng buộc của đề bài.
- Lỗi đang gặp.
- Format kết quả mong muốn.
- Yêu cầu AI giải thích từng bước.

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Prompt càng rõ thì AI càng trả lời sát hơn. Nếu thiếu bối cảnh, AI dễ đề xuất giải pháp không phù hợp với project.
```

### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?

```text
Nhóm sẽ ghi rõ project dùng .NET 8, EF Core, SQL Server, Clean Architecture và yêu cầu không thêm package ngoài scope.
```

---

## 9. Phân loại prompt đã sử dụng

Đánh dấu số lượng prompt theo từng nhóm.

| Loại prompt | Số lượng | Ví dụ prompt tiêu biểu |
|---|---:|---|
| Prompt phân tích yêu cầu | 1 | Tóm tắt yêu cầu Register/Login API |
| Prompt giải thích kiến thức | 1 | Giải thích Clean Architecture và EF Core |
| Prompt thiết kế giải pháp | 2 | Thiết kế layer và Register flow |
| Prompt thiết kế database | 2 | Tạo entity model, DbContext và VerificationToken table |
| Prompt sinh code mẫu | 8 | Setup layer, Register API, Login API, Email OTP infrastructure, Verify Email, Password Management, User Profile API và Sport Management API |
| Prompt debug lỗi | 4 | Kiểm tra package/test chưa phù hợp, build bị khóa process API, null principal và DLL lock khi test profile |
| Prompt viết test case | 6 | Test AuthController, JwtTokenService, verify/resend endpoints, password management endpoints, user profile endpoints và sport management endpoints/service |
| Prompt review code | 1 | Review DI, response và build |
| Prompt tối ưu code | 1 | Rút gọn Program.cs và docs |
| Prompt viết báo cáo | 0 | Chưa ghi nhận riêng |
| Prompt chuẩn bị thuyết trình | 0 | Chưa thực hiện |
| Prompt khác | 0 |  |

---

## 10. Checklist chất lượng prompt

Sinh viên/nhóm tự kiểm tra chất lượng prompt đã dùng.

| Tiêu chí | Đã đạt? | Ghi chú |
|---|:---:|---|
| Prompt có mục tiêu rõ ràng | Đạt | Nêu rõ chức năng cần làm |
| Prompt có đủ bối cảnh | Đạt | Có mô tả project và công nghệ |
| Prompt có nêu công nghệ/ngôn ngữ sử dụng | Đạt | ASP.NET Core, EF Core, SQL Server |
| Prompt có nêu yêu cầu đầu ra | Đạt | Có file, endpoint, response mẫu |
| Prompt không yêu cầu AI làm toàn bộ bài một cách máy móc | Đạt | Nhóm vẫn review và chỉnh sửa |
| Prompt có yêu cầu AI giải thích hoặc phân tích | Đạt | Có yêu cầu theo Clean Architecture |
| Kết quả AI được kiểm tra lại | Đạt | Chạy build/test và review package |
| Kết quả AI được chỉnh sửa trước khi sử dụng | Đạt | Chỉnh theo entity và DbContext thật |
| Prompt quan trọng được ghi lại đầy đủ | Đạt | Có prompt model, layer, register |
| Prompt sai/chưa hiệu quả được rút kinh nghiệm | Đạt | Ghi nhận lỗi SQLite test package |

---

## 11. Cam kết sử dụng prompt minh bạch

Sinh viên/nhóm cam kết rằng:

- Các prompt quan trọng đã được ghi lại trung thực.
- Không che giấu việc sử dụng AI trong các phần quan trọng của bài.
- Không nộp nguyên văn kết quả AI nếu chưa kiểm tra và chỉnh sửa.
- Có khả năng giải thích các phần đã sử dụng từ AI.
- Chịu trách nhiệm với sản phẩm cuối cùng.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Nguyen Phan Huy | 03/06/2026 |
