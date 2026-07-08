# Design Spec: API Contract Extraction for FrontEnd (PlayCourt Project)

**Date:** 2026-07-08
**Status:** Approved by User

## 1. Goal
Create comprehensive, developer-friendly API documentation so that Frontend developers can integrate with the PlayCourt Backend API without reading the C# backend source code. 

We will generate three Markdown documents in the `docs/` folder:
1. `docs/API_CONTRACT_FOR_FE.md`: Detailed API endpoints, methods, auth, parameters, requests, actual response shapes (`ApiResponse<T>` / `PagedResponse<T>`), properties, and error cases.
2. `docs/API_ENUMS_FOR_FE.md`: All system enums mapped to numeric values, string names, and recommended Vietnamese labels.
3. `docs/API_FE_TASK_MAPPING.md`: A mapping from typical frontend screens and workflows to the corresponding backend API endpoints.

## 2. Inputs & Context
- `D:\Users\huynpde180519\fpt\SUMMER_26\PRN232\playcount_source\playcount-heroui-fe\swagger.json`: OpenAPI definition of the endpoints (parameters, paths, raw DTO structures). Note that it lacks details on actual response wrappers like `ApiResponse<T>` and controller-level security policies (`[Authorize]`).
- Backend source code (referenced in `repomix-output.md` and available in `PlayCourt.*` folders):
  - Controllers in `PlayCourt.API/Controllers/` to extract route, HTTP method, and `[Authorize]` attributes.
  - Services/Interfaces in `PlayCourt.Application/` and `PlayCourt.Domain/` to extract actual service return types, DTO shapes, and Enums.

## 3. Extraction Methodology (Automated + Manual)
We will run a Python script to scan the codebase and parse the OpenAPI schema:
- **OpenAPI Parsing**: Extract all paths, HTTP methods, operation tags, path/query parameters, request body schema references.
- **Controller & Service Scanning**:
  - Parse C# Controllers to detect `[Authorize(Roles = "...")]` attributes, mapping them to the specific roles.
  - Match action method invocations with service return types to identify if they return `ApiResponse<T>`, `PagedResponse<T>`, or other models.
  - Scan `PlayCourt.Domain/Enums` (or equivalent files in `PlayCourt.Domain`) to extract enum names and values.
- **Manual Enrichment**:
  - Review custom workflows (e.g., PayOS payment flows, file uploads, token refresh mechanisms).
  - Add Vietnamese labels for enums.
  - Map endpoints to Frontend screens based on standard PlayCourt workflows.

## 4. Deliverables Checklist
- [ ] `docs/API_CONTRACT_FOR_FE.md`
- [ ] `docs/API_ENUMS_FOR_FE.md`
- [ ] `docs/API_FE_TASK_MAPPING.md`
- [ ] Self-review of documents to ensure no placeholders (`TBD`/`TODO`), and mark unresolved items with `NEED_VERIFY`.
