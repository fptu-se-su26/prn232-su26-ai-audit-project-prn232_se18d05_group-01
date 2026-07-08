# PlayCourt API Contract Extraction Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Create comprehensive API Contract, Enums translation, and Frontend Task Mapping documents in `docs/` for Frontend developers without code types output.

**Architecture:** We will use a script-based extraction process. A Python script will parse `swagger.json` and scan the C# Controller and Application directories to resolve actual response DTO shapes, roles, and HTTP endpoints. We will then refine and enrich this extracted data into Markdown documents.

**Tech Stack:** Python 3 (json, os, re modules), Markdown, C# / ASP.NET Core Web API parsing.

## Global Constraints
- Target output files: `docs/API_CONTRACT_FOR_FE.md`, `docs/API_ENUMS_FOR_FE.md`, `docs/API_FE_TASK_MAPPING.md`.
- No TypeScript types are to be generated.
- Strict adherence to source API paths, field names, and structures. No arbitrary route renaming.
- Identify missing definitions as `NEED_VERIFY` and specify the files to review.

---

### Task 1: Create and Run Extraction Script
**Files:**
- Create: `scratch/extract_metadata.py`
- Run: python `scratch/extract_metadata.py`

**Interfaces:**
- Consumes: `D:\Users\huynpde180519\fpt\SUMMER_26\PRN232\playcount_source\playcount-heroui-fe\swagger.json`
- Consumes: C# codebase directory structure

- [ ] **Step 1: Write the metadata extractor script**
  Create `scratch/extract_metadata.py` with Python code to load `swagger.json`, parse paths, find parameters, schemas, and map them. Additionally, read the `PlayCourt.API/Controllers` directory to scan for `[Authorize(Roles = "...")]` and check service response wrappers.
- [ ] **Step 2: Run the script to extract initial data**
  Run the script using python command.
- [ ] **Step 3: Verify the output files are generated**
  Check the workspace files to see if a raw layout or JSON data is created.
- [ ] **Step 4: Commit**
  ```bash
  git add scratch/extract_metadata.py
  git commit -m "tool: add python script for API metadata extraction"
  ```

### Task 2: Build and Write API Contract Document
**Files:**
- Create/Overwrite: `docs/API_CONTRACT_FOR_FE.md`

**Interfaces:**
- Consumes: Outputs from Task 1 (extracted metadata)

- [ ] **Step 1: Structure the API contract by Module / Tag**
  Group endpoints into tags like Auth, User, Court, CourtOwner, Slot, Booking, Amenity, Rating, Payment.
- [ ] **Step 2: Detail each endpoint**
  For each endpoint, write its HTTP method, path, required authorization, roles, request parameters, request body, and actual response wrapper (`ApiResponse<T>` / `PagedResponse<T>`).
- [ ] **Step 3: Document DTO property details**
  List all fields of the involved DTOs with their types (string, number, boolean, array, etc.) and check source code if Swagger misses schemas.
- [ ] **Step 4: Verify specific flows**
  Document upload images flow, PayOS payment callback/redirect flow, and refresh token flow.
- [ ] **Step 5: Verify no placeholders and commit**
  Ensure all sections are completely filled or marked with `NEED_VERIFY` with files to check.
  ```bash
  git add docs/API_CONTRACT_FOR_FE.md
  git commit -m "docs: generate API_CONTRACT_FOR_FE.md contract document"
  ```

### Task 3: Build and Write API Enums Document
**Files:**
- Create/Overwrite: `docs/API_ENUMS_FOR_FE.md`

**Interfaces:**
- Consumes: C# Domain Enums definitions

- [ ] **Step 1: Scan for C# Enums in Domain project**
  Identify all Enum declarations in `PlayCourt.Domain/Enums` or throughout the codebase.
- [ ] **Step 2: Map Enum fields to numeric values and string names**
  Verify the integer values assigned to each enum option (defaulting to 0, 1, 2... if not explicitly set).
- [ ] **Step 3: Formulate Vietnamese labels for each enum**
  Provide intuitive Vietnamese labels for display purposes.
- [ ] **Step 4: Write the docs/API_ENUMS_FOR_FE.md markdown file**
  Organize enums in table layouts.
- [ ] **Step 5: Commit**
  ```bash
  git add docs/API_ENUMS_FOR_FE.md
  git commit -m "docs: generate API_ENUMS_FOR_FE.md system enums document"
  ```

### Task 4: Map Frontend Screens to Endpoints
**Files:**
- Create/Overwrite: `docs/API_FE_TASK_MAPPING.md`

**Interfaces:**
- Consumes: `docs/API_CONTRACT_FOR_FE.md`

- [ ] **Step 1: Identify all PlayCourt FE Screens based on standard product structure**
  Determine screens like Home, Court Search, Court Details, Slot Selection, Checkout, Booking History, Owner Dashboard, Admin Dashboard.
- [ ] **Step 2: Map each screen to required API Endpoints**
  List endpoints to call for loading data, submit forms, or update status.
- [ ] **Step 3: Write to docs/API_FE_TASK_MAPPING.md**
  Write down the mapping in clear tables or lists.
- [ ] **Step 4: Commit**
  ```bash
  git add docs/API_FE_TASK_MAPPING.md
  git commit -m "docs: generate API_FE_TASK_MAPPING.md screens mapping document"
  ```

### Task 5: Document Integrity & Self-Review
**Files:**
- Modify: `docs/API_CONTRACT_FOR_FE.md`
- Modify: `docs/API_ENUMS_FOR_FE.md`
- Modify: `docs/API_FE_TASK_MAPPING.md`

- [ ] **Step 1: Check document alignment**
  Ensure enum types in the contract document point to the definitions in `API_ENUMS_FOR_FE.md`.
- [ ] **Step 2: Verify `NEED_VERIFY` notes**
  Check C# source files directly to resolve as many `NEED_VERIFY` notes as possible.
- [ ] **Step 3: Run validation and final checks**
  Run markdown linting or check visually.
- [ ] **Step 4: Commit changes**
  ```bash
  git add docs/*.md
  git commit -m "docs: finalize and polish PlayCourt FE developer API documentation"
  ```
