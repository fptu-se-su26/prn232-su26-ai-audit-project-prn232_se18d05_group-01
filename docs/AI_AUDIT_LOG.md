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
| Ngày sử dụng |  |
| Công cụ AI | ChatGPT / Gemini / Claude / GitHub Copilot / Cursor / Antigravity / Khác |
| Mục đích sử dụng |  |
| Phần việc liên quan | Requirement / Design / Database / Frontend / Backend / Testing / Debug / Report / Presentation / Other |
| Mức độ sử dụng | Hỗ trợ ý tưởng / Hỗ trợ một phần / Hỗ trợ nhiều / Sinh chính nội dung |

#### 4.1. Prompt đã sử dụng

```text
Dán nguyên văn prompt đã hỏi AI tại đây.
```

#### 4.2. Kết quả AI gợi ý

Tóm tắt nội dung AI đã trả lời hoặc gợi ý.

```text
Viết tại đây...
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

Mô tả rõ phần nào được sử dụng lại từ gợi ý của AI.

```text
Viết tại đây...
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

Mô tả sinh viên/nhóm đã thay đổi, kiểm tra, sửa lỗi hoặc cải tiến gì so với gợi ý ban đầu của AI.

```text
Viết tại đây...
```

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit |  |
| File liên quan |  |
| Screenshot |  |
| Kết quả chạy/test |  |
| Link video demo |  |
| Ghi chú khác |  |

#### 4.6. Nhận xét cá nhân/nhóm

Sinh viên/nhóm học được gì sau lần sử dụng AI này?

```text
Viết tại đây...
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

## 5. Bảng tổng hợp mức độ sử dụng AI

Đánh dấu mức độ AI hỗ trợ ở từng hạng mục.

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Phân tích yêu cầu |  |  |  |  |  |
| Viết user story/use case |  |  |  |  |  |
| Thiết kế database |  |  | x |  | AI hướng dẫn tạo model và DbContext |
| Thiết kế kiến trúc hệ thống |  |  | x |  | AI hỗ trợ định hướng Domain, Application, Infrastructure và API layer |
| Thiết kế giao diện |  |  |  |  |  |
| Code frontend |  |  |  |  |  |
| Code backend |  |  | x |  | AI hỗ trợ entity, enum, EF Core configuration và setup layer |
| Debug lỗi |  | x |  |  | AI hỗ trợ kiểm tra duplicate index, filter, constraint và lỗi compile/test |
| Viết test case |  | x |  |  | AI hỗ trợ định hướng test smoke/verify DI |
| Kiểm thử sản phẩm |  | x |  |  | Nhóm tự chạy format, build và test để kiểm chứng |
| Tối ưu code |  |  |  |  |  |
| Viết báo cáo |  |  |  |  |  |
| Làm slide thuyết trình |  |  |  |  |  |

---

## 6. Các lỗi hoặc hạn chế từ AI

Ghi lại các trường hợp AI trả lời sai, thiếu, chưa phù hợp hoặc sinh code không chạy.

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 |  |  |  |
| 2 |  |  |  |
| 3 |  |  |  |

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
Viết tại đây...
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
Sẽ cập nhật khi project kết thúc.
```

### 9.2. Phần nào em/nhóm không sử dụng theo gợi ý của AI? Vì sao?

```text
Sẽ cập nhật khi project kết thúc.
```

### 9.3. Em/nhóm đã kiểm tra tính đúng đắn của kết quả AI như thế nào?

```text
Sẽ cập nhật khi project kết thúc.
```

### 9.4. Nếu không có AI, phần nào sẽ khó khăn nhất?

```text
Sẽ cập nhật khi project kết thúc.
```

### 9.5. Sau bài tập/project này, em/nhóm học được gì về môn học?

```text
Sẽ cập nhật khi project kết thúc.
```

### 9.6. Sau bài tập/project này, em/nhóm học được gì về cách sử dụng AI có trách nhiệm?

```text
Sẽ cập nhật khi project kết thúc.
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
| Nguyen Phan Huy | Sẽ cập nhật sau khi xác nhận |
