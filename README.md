# SE AI Audit Project Template

## 1. Project Information

| Item | Description |
|---|---|
| Course | PRN232 |
| Class | SE18D05 |
| Semester | SU26 |
| Group | Group 01 |
| Topic | PlayCount API - Sport Court Booking System |
| Repository | https://github.com/fptu-se-su26/prn232-su26-ai-audit-project-prn232_se18d05_group-01 |

---

## 2. Team Members

| No | Student ID | Full Name | GitHub Username | Role | Main Responsibility |
|---:|---|---|---|---|---|
| 1 | DE180519 | Nguyen Phan Huy | TBD | Leader | Backend project setup, architecture, API coordination |
| 2 | DE180405 | Phan Thanh Vuong | ptvuong2505 | Member | Backend feature development |
| 3 | DE180313 | Nguyen Van Hai | vohai04 | Member | Backend feature development |
| 4 | DE180310 | Phan Quoc Khanh | PQKhanh294 | Member | Testing and documentation |
| 5 | TBD | Trinh Viet Hoang | TBD | Member | Testing and documentation |

---

## 3. Project Structure

```text
PlayCount.API/
PlayCount.Application/
PlayCount.Domain/
PlayCount.Infrastructure/
PlayCount.ApiTests/
docs/
.github/
PlayCount.sln
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
feature/se123456-login-page
bugfix/se123456-login-validation
docs/se123456-update-ai-audit-log
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

Prerequisites:

- .NET SDK 8.0 or newer
- Visual Studio 2022, Visual Studio Code, JetBrains Rider, or another C# IDE

Restore packages:

```bash
dotnet restore PlayCount.sln
```

Build solution:

```bash
dotnet build PlayCount.sln
```

Run API:

```bash
dotnet run --project PlayCount.API/PlayCount.API.csproj --launch-profile http
```

Run tests:

```bash
dotnet test PlayCount.sln
```

Local API URLs:

| Service | URL |
|---|---|
| HTTP API | [http://localhost:5187](http://localhost:5187) |
| HTTPS API | [https://localhost:7174](https://localhost:7174) |
| Swagger HTTP | [http://localhost:5187/swagger](http://localhost:5187/swagger) |
| Swagger HTTPS | [https://localhost:7174/swagger](https://localhost:7174/swagger) |

These URLs only work after the API is running.

Copy all setup commands:

```bash
dotnet restore PlayCount.sln
dotnet build PlayCount.sln
dotnet run --project PlayCount.API/PlayCount.API.csproj --launch-profile http
```

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
