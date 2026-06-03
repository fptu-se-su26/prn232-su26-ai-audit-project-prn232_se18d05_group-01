# SE AI Audit Project Template

## 1. Project Information

| Item | Description |
|---|---|
| Course | PRN232 |
| Class | SE18D05 |
| Semester | SU26 |
| Group | Group 01 |
| Topic | PlayCourt API - Sport Court Booking System |
| Repository | https://github.com/fptu-se-su26/prn232-su26-ai-audit-project-prn232_se18d05_group-01 |
| Frontend | https://wykowjbu.github.io/play-count-fe/ |

---

## 2. Team Members

| No | Student ID | Full Name | GitHub Username | Role | Main Responsibility |
|---:|---|---|---|---|---|
| 1 | DE180519 | Nguyen Phan Huy | Wykowjbu | Leader | Backend project setup, architecture, API coordination |
| 2 | DE180405 | Phan Thanh Vuong | ptvuong2505 | Member | Backend feature development |
| 3 | DE180313 | Nguyen Van Hai | vohai04 | Member | Backend feature development |
| 4 | DE180310 | Phan Quoc Khanh | PQKhanh294 | Member | Testing and documentation |
| 5 | DE190946 | Trinh Viet Hoang | HoangTrinhyeuoi | Member | Testing and documentation |

---

## 3. Project Structure

```text
PlayCourt.API/
PlayCourt.Application/
PlayCourt.Domain/
PlayCourt.Infrastructure/
PlayCourt.ApiTests/
docs/
.github/
PlayCourt.sln
README.md
```

---

## 4. Required AI Audit Documents

Each group must maintain the following documents:

```text
docs/AI_AUDIT_LOG.md
docs/PROMPTS.md
docs/REFLECTION.md
docs/CHANGELOG.md
```

---

## 5. Workflow

Students must follow this workflow:

```text
Issue → Branch → Commit → Pull Request → Review → Merge
```

Direct push to the `main` branch should be avoided.

---

## 6. Branch Naming Convention

```text
feature/studentid-task-name
bugfix/studentid-error-name
docs/studentid-update-audit-log
test/studentid-test-case-name
```

Example:

```text
feature/de180519-login-page
bugfix/de180519-login-validation
docs/de180519-update-ai-audit-log
```

---

## 7. Commit Message Convention

```text
[StudentID] type: short description
```

Examples:

```text
[DE180519] feat: add login page
[DE180519] fix: fix login validation
[DE180519] docs: update AI audit log
[DE180519] test: add login test cases
```

Common types:

```text
feat     - Add a new feature
fix      - Fix a bug
docs     - Update documentation
test     - Add or update tests
refactor - Improve code without changing behavior
style    - Format code without changing logic
chore    - Update config, packages, tools, or build files
```

---

## 8. How to Run

### Prerequisites

Đảm bảo máy bạn đã cài đặt:

| Tool | Version | Download |
|---|---|---|
| .NET SDK | 8.0 trở lên | [Download](https://dotnet.microsoft.com/download/dotnet/8.0) |
| Visual Studio 2022 / VS Code / Rider | Latest | [VS 2022](https://visualstudio.microsoft.com/) |
| SQL Server | LocalDB / Express / Developer | (xem bên dưới) |
| EF Core Tools | 8.x | `dotnet tool install --global dotnet-ef` |

#### Chọn SQL Server phù hợp

- **LocalDB** (khuyến nghị cho dev): Đi kèm Visual Studio 2022. Không cần cài thêm gì.
- **SQL Server Express**: [Download miễn phí](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- **Docker**: `docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest`

### Step 1: Clone và Restore

```bash
git clone https://github.com/fptu-se-su26/prn232-su26-ai-audit-project-prn232_se18d05_group-01.git
cd prn232-su26-ai-audit-project-prn232_se18d05_group-01
dotnet restore PlayCourt.sln
```

### Step 2: Cấu hình Connection String

Mặc định repo đã có connection string LocalDB an toàn trong `PlayCourt.API/appsettings.json`, nên nếu máy có LocalDB thì có thể bỏ qua bước này và chạy luôn Quick Setup.

Nếu mỗi thành viên dùng SQL Server riêng, copy file mẫu rồi điền server/username/password của máy mình. File `appsettings.Development.json` đã được `.gitignore`, không commit username/password/server cá nhân:

```bash
copy PlayCourt.API\appsettings.Development.example.json PlayCourt.API\appsettings.Development.json
```

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=PlayCourtDb;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;Encrypt=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "YOUR_DEVELOPMENT_SECRET_KEY_AT_LEAST_32_CHARACTERS",
    "Issuer": "PlayCourt",
    "Audience": "PlayCourtClient",
    "ExpiresInMinutes": 60
  }
}
```

Thay các giá trị sau:

| Placeholder | Điền gì |
|---|---|
| `YOUR_SERVER` | Tên SQL Server của máy bạn, ví dụ `localhost`, `.\SQLEXPRESS`, `PHANHUY`, hoặc `localhost,1433` |
| `YOUR_USERNAME` | Username SQL Server, ví dụ `sa` |
| `YOUR_PASSWORD` | Password SQL Server của máy bạn |

> **Nếu dùng LocalDB**, có thể dùng connection string không cần username/password:
>
> ```
> Server=(localdb)\mssqllocaldb;Database=PlayCourtDb;Trusted_Connection=True;TrustServerCertificate=True
> ```

#### Cấu hình JWT

| Placeholder | Điền gì |
|---|---|
| `YOUR_DEVELOPMENT_SECRET_KEY_AT_LEAST_32_CHARACTERS` | Khóa bí mật dùng để ký JWT trong môi trường development, tối thiểu 32 ký tự |
| `Issuer` | Tên hệ thống phát hành token, mặc định `PlayCourt` |
| `Audience` | Client/API audience, mặc định `PlayCourtClient` |
| `ExpiresInMinutes` | Thời gian hết hạn token, mặc định `60` phút |

> Không commit secret thật hoặc production secret. Chỉ dùng placeholder/dev key trong file mẫu.

Không commit `appsettings.Development.json`. Chỉ commit `appsettings.json` với default an toàn và `appsettings.Development.example.json` làm mẫu.

### Step 3: Cài đặt EF Core Tools (nếu chưa có)

```bash
dotnet tool install --global dotnet-ef
```

Kiểm tra đã cài thành công:

```bash
dotnet ef --version
```

### Step 4: Tạo Database bằng Migration

```bash
dotnet ef database update --project PlayCourt.Infrastructure --startup-project PlayCourt.API
```

Lệnh trên sẽ tự động:
- Tạo database `PlayCourtDb` nếu chưa tồn tại
- Chạy migration `InitialCreate` để tạo tất cả 25 bảng

### Step 5: Build và chạy

```bash
dotnet build PlayCourt.sln
dotnet run --project PlayCourt.API/PlayCourt.API.csproj --launch-profile http
```

### Step 6: Verify

Mở trình duyệt và truy cập:

| Service | URL |
|---|---|
| Swagger UI | [http://localhost:5187/swagger](http://localhost:5187/swagger) |
| HTTPS Swagger | [https://localhost:7174/swagger](https://localhost:7174/swagger) |

#### Verify Login API with JWT

Sau khi chạy API, mở Swagger và test:

```http
POST /api/auth/login
```

Sample body:

```json
{
  "identifier": "player@example.com",
  "password": "123456"
}
```

Nếu login thành công, copy `accessToken`, bấm nút **Authorize** trong Swagger và nhập:

```text
Bearer <accessToken>
```

Sau đó có thể test các API yêu cầu authentication/authorization.

### Chạy Tests

```bash
dotnet test PlayCourt.sln
```

### Quick Setup (copy all commands)

Quick Setup dưới đây dùng LocalDB mặc định trong `PlayCourt.API/appsettings.json`. Nếu bạn dùng SQL Server riêng, tạo `PlayCourt.API/appsettings.Development.json` như Step 2 trước khi chạy lệnh tạo database.

```bash
# 1. Clone
git clone https://github.com/fptu-se-su26/prn232-su26-ai-audit-project-prn232_se18d05_group-01.git
cd prn232-su26-ai-audit-project-prn232_se18d05_group-01

# 2. Restore
dotnet restore PlayCourt.sln

# 3. Install EF Tools (skip if already installed)
dotnet tool install --global dotnet-ef

# 4. Create Database
dotnet ef database update --project PlayCourt.Infrastructure --startup-project PlayCourt.API

# 5. Build & Run
dotnet build PlayCourt.sln
dotnet run --project PlayCourt.API/PlayCourt.API.csproj --launch-profile http
```

### Troubleshooting

| Lỗi | Nguyên nhân | Cách sửa |
|---|---|---|
| `Cannot open database` | Database chưa được tạo | Chạy lại `dotnet ef database update ...` |
| `Login failed for user` | Connection string sai | Kiểm tra lại username/password trong `appsettings.Development.json` |
| `dotnet ef not found` | Chưa cài EF Core Tools | Chạy `dotnet tool install --global dotnet-ef` |
| `LocalDB not installed` | Chưa cài SQL Server LocalDB | Cài Visual Studio 2022 (chọn workload ASP.NET) hoặc [cài riêng LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) |
| `Build failed` | Thiếu .NET SDK 8.0 | [Download .NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) |
| `Jwt:Key is missing or invalid` | Chưa cấu hình `Jwt:Key` hoặc key quá ngắn | Kiểm tra `Jwt` config trong `appsettings.Development.json`, key nên tối thiểu 32 ký tự |

---

## 9. AI Usage Rule

Students are allowed to use AI tools such as ChatGPT, Gemini, Claude, GitHub Copilot, Cursor, Antigravity, or similar tools.

However, all important AI usage must be recorded in:

```text
docs/AI_AUDIT_LOG.md
docs/PROMPTS.md
docs/CHANGELOG.md
docs/REFLECTION.md
```

Students must be able to explain, verify, and defend all submitted work.
