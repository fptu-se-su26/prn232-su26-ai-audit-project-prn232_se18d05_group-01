import os
import json
import re

SWAGGER_PATH = r"D:\Users\huynpde180519\fpt\SUMMER_26\PRN232\playcount_source\playcount-heroui-fe\swagger.json"
INTERFACES_DIR = r"D:\Users\huynpde180519\fpt\SUMMER_26\PRN232\playcount_source\prn232-su26-ai-audit-project-prn232_se18d05_group-01\PlayCourt.Application\Interfaces"
CONTROLLERS_DIR = r"D:\Users\huynpde180519\fpt\SUMMER_26\PRN232\playcount_source\prn232-su26-ai-audit-project-prn232_se18d05_group-01\PlayCourt.API\Controllers"
ENUMS_FILE = r"D:\Users\huynpde180519\fpt\SUMMER_26\PRN232\playcount_source\prn232-su26-ai-audit-project-prn232_se18d05_group-01\PlayCourt.Domain\Enums\DomainEnums.cs"
OUTPUT_CONTRACT = r"D:\Users\huynpde180519\fpt\SUMMER_26\PRN232\playcount_source\prn232-su26-ai-audit-project-prn232_se18d05_group-01\docs\API_CONTRACT_FOR_FE.md"
OUTPUT_ENUMS = r"D:\Users\huynpde180519\fpt\SUMMER_26\PRN232\playcount_source\prn232-su26-ai-audit-project-prn232_se18d05_group-01\docs\API_ENUMS_FOR_FE.md"

def extract_generic_type_and_method(line):
    """
    Parses a line like Task<PagedResponse<IReadOnlyCollection<BookingResponseDto>>> GetVenueBookingsAsync(...)
    Returns (return_type, method_name) or None
    """
    match = re.search(r"Task\s*<\s*", line)
    if not match:
        return None
    start_idx = match.end()
    
    count = 1
    end_idx = -1
    for idx in range(start_idx, len(line)):
        if line[idx] == '<':
            count += 1
        elif line[idx] == '>':
            count -= 1
            if count == 0:
                end_idx = idx
                break
                
    if end_idx == -1:
        return None
        
    return_type = line[start_idx:end_idx].strip()
    
    # Extract method name from the remaining part of the line
    remaining = line[end_idx + 1:]
    method_match = re.search(r"\s+(\w+)\s*\(", remaining)
    if method_match:
        return return_type, method_match.group(1)
        
    return None

def parse_service_interfaces():
    """
    Scans C# interfaces to map (InterfaceName, MethodName) to their return types.
    Returns a dict: { (InterfaceName, MethodName): ReturnTypeString }
    """
    service_map = {}
    if not os.path.exists(INTERFACES_DIR):
        print(f"Interfaces directory not found: {INTERFACES_DIR}")
        return service_map

    for file_name in os.listdir(INTERFACES_DIR):
        if not file_name.endswith(".cs"):
            continue
        interface_name = file_name[:-3] # Remove ".cs"
        file_path = os.path.join(INTERFACES_DIR, file_name)
        with open(file_path, "r", encoding="utf-8") as f:
            for line in f:
                res = extract_generic_type_and_method(line)
                if res:
                    return_type, method_name = res
                    # Clean return type representation for frontend (e.g. replace Task/IReadOnlyCollection if needed)
                    # For clarity we will keep PagedResponse<IReadOnlyCollection<BookingResponseDto>> as is
                    service_map[(interface_name, method_name.lower())] = return_type
                    if method_name.endswith("Async"):
                        service_map[(interface_name, method_name[:-5].lower())] = return_type

    return service_map

def extract_role_from_attributes(attrs_str, default_roles="Public"):
    if not attrs_str:
        return default_roles
    
    if "[AllowAnonymous]" in attrs_str:
        return "Public"
        
    auth_matches = re.findall(r"\[Authorize\s*(?:\((.*?)\))?\]", attrs_str)
    if not auth_matches:
        return default_roles
        
    roles = []
    for arg_str in auth_matches:
        if not arg_str:
            roles.append("Authenticated")
            continue
        
        policy_match = re.search(r"Policy\s*=\s*ApiPolicies\.(\w+)", arg_str)
        if policy_match:
            roles.append(policy_match.group(1))
            continue
            
        policy_str_match = re.search(r"Policy\s*=\s*\"([^\"]+)\"", arg_str)
        if policy_str_match:
            roles.append(policy_str_match.group(1))
            continue

        roles_match = re.search(r"Roles\s*=\s*\"([^\"]+)\"", arg_str)
        if roles_match:
            roles.append(roles_match.group(1))
            continue
            
        roles.append("Authenticated")
        
    return ", ".join(set(roles)) if roles else "Authenticated"

def parse_controllers(service_map):
    controller_info = {}
    if not os.path.exists(CONTROLLERS_DIR):
        print(f"Controllers directory not found: {CONTROLLERS_DIR}")
        return controller_info

    for file_name in os.listdir(CONTROLLERS_DIR):
        if not file_name.endswith("Controller.cs") or file_name == "WeatherForecastController.cs":
            continue
        file_path = os.path.join(CONTROLLERS_DIR, file_name)
        with open(file_path, "r", encoding="utf-8") as f:
            content = f.read()

        class_match = re.search(r"class\s+(\w+Controller)\s*:\s*ControllerBase", content)
        if not class_match:
            continue
        controller_name = class_match.group(1)
        route_prefix = f"api/{controller_name[:-10]}"

        class_route_match = re.search(r"\[Route\(\"([^\"]+)\"\)\]\s*public\s+sealed\s+class", content)
        if class_route_match:
            route_prefix = class_route_match.group(1).replace("[controller]", controller_name[:-10])

        var_to_interface = {}
        fields_matches = re.findall(r"private\s+readonly\s+(I\w+Service|I\w+Gateway)\s+(\w+)\s*;", content)
        for interface_type, var_name in fields_matches:
            var_to_interface[var_name] = interface_type

        constructor_match = re.search(rf"public\s+{controller_name}\s*\((.*?)\)", content, re.DOTALL)
        if constructor_match:
            params = constructor_match.group(1)
            param_matches = re.findall(r"(I\w+Service|I\w+Gateway)\s+(\w+)", params)
            for interface_type, var_name in param_matches:
                var_to_interface[var_name] = interface_type
                var_to_interface[f"_{var_name}"] = interface_type

        class_header = content.split("public sealed class")[0]
        class_roles = extract_role_from_attributes(class_header, default_roles="Public")

        methods = re.split(r"(public\s+async\s+Task<IActionResult>|public\s+IActionResult)\s+(\w+)\s*\(", content)
        
        for i in range(1, len(methods), 3):
            action_type = methods[i]
            action_name = methods[i+1]
            remaining = methods[i+2]
            
            param_match = re.match(r"(.*?)\)\s*^.*?\{", remaining, re.DOTALL | re.MULTILINE)
            if not param_match:
                param_match = re.match(r"(.*?)\)\s*\{", remaining, re.DOTALL)
            
            params_str = param_match.group(1) if param_match else ""
            
            body_start = remaining.find("{")
            body_content = ""
            if body_start != -1:
                brace_count = 1
                for j in range(body_start + 1, len(remaining)):
                    if remaining[j] == "{":
                        brace_count += 1
                    elif remaining[j] == "}":
                        brace_count -= 1
                        if brace_count == 0:
                            body_content = remaining[body_start:j+1]
                            break

            prev_block = methods[i-1]
            lines = prev_block.strip().split("\n")
            attr_lines = []
            for line in reversed(lines):
                line_strip = line.strip()
                if not line_strip:
                    continue
                if line_strip.startswith("[") or line_strip.endswith("]"):
                    attr_lines.insert(0, line_strip)
                else:
                    break
            attrs_str = " ".join(attr_lines)

            http_match = re.search(r"\[Http(Get|Post|Put|Delete|Patch)(?:\(\"([^\"]+)\"\))?\]", attrs_str)
            if not http_match:
                continue
                
            http_method = http_match.group(1)
            sub_route = http_match.group(2) or ""

            if sub_route.startswith("~/"):
                full_path = sub_route[2:]
            elif sub_route.startswith("api/") or sub_route.startswith("/api/"):
                full_path = sub_route
            else:
                if sub_route.startswith("/"):
                    full_path = f"{route_prefix}{sub_route}"
                elif sub_route:
                    full_path = f"{route_prefix}/{sub_route}"
                else:
                    full_path = route_prefix

            full_path = re.sub(r":\w+", "", full_path)
            full_path = "/" + full_path.replace("[controller]", controller_name[:-10]).strip("/")
            
            roles = extract_role_from_attributes(attrs_str, default_roles=class_roles)

            inferred_response = "ApiResponse<object>"
            
            service_calls = re.findall(r"await\s+(\w+)\.(\w+)\(", body_content)
            for var_name, method_name in service_calls:
                interface_name = var_to_interface.get(var_name)
                if interface_name:
                    map_key = (interface_name, method_name.lower())
                    if map_key in service_map:
                        inferred_response = service_map[map_key]
                        break
            
            if inferred_response == "ApiResponse<object>":
                if "ApiResponse<" in body_content:
                    direct_match = re.search(r"ApiResponse<\s*([^>]+?)\s*>", body_content)
                    if direct_match:
                        inferred_response = f"ApiResponse<{direct_match.group(1).strip()}>"
                elif "PagedResponse<" in body_content:
                    direct_match = re.search(r"PagedResponse<\s*([^>]+?)\s*>", body_content)
                    if direct_match:
                        inferred_response = f"PagedResponse<{direct_match.group(1).strip()}>"

            key = (full_path.lower(), http_method.lower())
            controller_info[key] = {
                "roles": roles,
                "response": inferred_response,
                "action": action_name,
                "original_path": full_path
            }

    return controller_info

def clean_ref(ref):
    if not ref:
        return None
    return ref.split("/")[-1]

def parse_swagger_schemas(swagger_data):
    schemas = swagger_data.get("components", {}).get("schemas", {})
    parsed_schemas = {}
    
    for schema_name, schema in schemas.items():
        if schema.get("type") == "object":
            properties = schema.get("properties", {})
            required_fields = schema.get("required", [])
            fields = []
            for prop_name, prop in properties.items():
                prop_type = prop.get("type", "object")
                ref = clean_ref(prop.get("$ref") or prop.get("items", {}).get("$ref"))
                is_nullable = prop.get("nullable", False)
                is_required = prop_name in required_fields
                
                if prop_type == "array":
                    item_type = prop.get("items", {}).get("type", "object")
                    if ref:
                        prop_type = f"Array<{ref}>"
                    else:
                        prop_type = f"Array<{item_type}>"
                elif ref:
                    prop_type = ref

                validations = []
                if "minLength" in prop:
                    validations.append(f"minLength: {prop['minLength']}")
                if "maxLength" in prop:
                    validations.append(f"maxLength: {prop['maxLength']}")
                if "minimum" in prop:
                    validations.append(f"min: {prop['minimum']}")
                if "maximum" in prop:
                    validations.append(f"max: {prop['maximum']}")
                if prop.get("format"):
                    validations.append(f"format: {prop['format']}")
                
                fields.append({
                    "name": prop_name,
                    "type": prop_type,
                    "required": is_required,
                    "nullable": is_nullable,
                    "validations": ", ".join(validations) if validations else ""
                })
            parsed_schemas[schema_name] = fields
        elif "enum" in schema:
            parsed_schemas[schema_name] = {
                "type": "enum",
                "values": schema["enum"]
            }
            
    return parsed_schemas

def get_vietnamese_label_for_enum(enum_name, field_name):
    labels = {
        "UserRole": {
            "Admin": "Quản trị viên",
            "Player": "Người chơi",
            "CourtOwner": "Chủ sân"
        },
        "UserStatus": {
            "Active": "Hoạt động",
            "Locked": "Bị khóa",
            "Inactive": "Chưa kích hoạt"
        },
        "Gender": {
            "Male": "Nam",
            "Female": "Nữ",
            "Other": "Khác"
        },
        "CourtOwnerVerificationStatus": {
            "Pending": "Chờ duyệt",
            "Approved": "Đã duyệt",
            "Rejected": "Đã từ chối"
        },
        "SkillLevel": {
            "Beginner": "Mới chơi",
            "Intermediate": "Trung bình",
            "Advanced": "Nâng cao"
        },
        "VenueStatus": {
            "Pending": "Chờ duyệt",
            "Approved": "Đã hoạt động",
            "Rejected": "Từ chối",
            "Suspended": "Tạm dừng"
        },
        "CourtStatus": {
            "Available": "Sẵn sàng",
            "Maintenance": "Bảo trì",
            "Inactive": "Ngưng hoạt động"
        },
        "VenueStaffRole": {
            "Manager": "Quản lý",
            "Receptionist": "Lễ tân",
            "Accountant": "Kế toán"
        },
        "BookingStatus": {
            "Pending": "Chờ thanh toán",
            "Confirmed": "Đã xác nhận",
            "CancelledByUser": "Khách hủy",
            "CancelledByOwner": "Chủ sân hủy",
            "Completed": "Hoàn thành",
            "Expired": "Hết hạn"
        },
        "PaymentType": {
            "BookingPayment": "Thanh toán đặt sân",
            "Refund": "Hoàn tiền",
            "Payout": "Rút tiền/Quyết toán"
        },
        "PaymentStatus": {
            "Pending": "Chờ thanh toán",
            "Success": "Thành công",
            "Failed": "Thất bại"
        },
        "MatchStatus": {
            "Open": "Đang tuyển",
            "Full": "Đã đầy",
            "Cancelled": "Đã hủy",
            "Completed": "Đã đá/Đã chơi"
        },
        "MatchJoinRequestStatus": {
            "Pending": "Chờ duyệt",
            "Approved": "Đã tham gia",
            "Rejected": "Bị từ chối"
        },
        "MatchInvitationStatus": {
            "Pending": "Chờ phản hồi",
            "Accepted": "Đồng ý",
            "Declined": "Từ chối",
            "Cancelled": "Đã thu hồi"
        },
        "ReviewStatus": {
            "Visible": "Hiển thị",
            "Hidden": "Bị ẩn",
            "Reported": "Bị báo cáo"
        },
        "NotificationType": {
            "Booking": "Đặt sân",
            "Match": "Kèo đấu",
            "Payment": "Thanh toán",
            "Review": "Đánh giá",
            "System": "Hệ thống"
        },
        "NotificationReferenceType": {
            "Booking": "Đặt sân",
            "Match": "Kèo đấu",
            "Payment": "Thanh toán",
            "Review": "Đánh giá",
            "Venue": "Cơ sở sân"
        },
        "VerificationTokenPurpose": {
            "EmailVerification": "Xác thực email",
            "PasswordReset": "Đặt lại mật khẩu"
        }
    }
    return labels.get(enum_name, {}).get(field_name, field_name)

def generate_enums_doc():
    if not os.path.exists(ENUMS_FILE):
        print(f"Enums file not found: {ENUMS_FILE}")
        return

    with open(ENUMS_FILE, "r", encoding="utf-8") as f:
        content = f.read()

    enum_blocks = re.findall(r"public\s+enum\s+(\w+)\s*(?::\s*\w+)?\s*\{(.*?)\}", content, re.DOTALL)
    
    markdown = "# PlayCourt System Enums Contract\n\n"
    markdown += "Tài liệu này định nghĩa toàn bộ Enum trong hệ thống PlayCourt, giá trị số (numeric value), tên code, và nhãn hiển thị Tiếng Việt đề xuất trên UI.\n\n"

    for enum_name, enum_body in enum_blocks:
        markdown += f"## {enum_name}\n\n"
        markdown += "| Code Option | Numeric Value | Label Tiếng Việt | Ghi chú |\n"
        markdown += "| :--- | :--- | :--- | :--- |\n"
        
        items = re.findall(r"(\w+)\s*=\s*(-?\d+)", enum_body)
        if not items:
            items_raw = re.findall(r"(\w+)(?:\s*,)?", enum_body)
            items = [(item.strip(), idx) for idx, item in enumerate(items_raw) if item.strip()]

        for field_name, value in items:
            field_name = field_name.strip()
            vn_label = get_vietnamese_label_for_enum(enum_name, field_name)
            markdown += f"| `{field_name}` | `{value}` | {vn_label} | |\n"
        markdown += "\n"

    with open(OUTPUT_ENUMS, "w", encoding="utf-8") as f:
        f.write(markdown)
    print(f"Generated Enums doc at: {OUTPUT_ENUMS}")

def format_dto_fields(fields):
    if not fields:
        return "*Không có properties hoặc body trống.*\n"
    md = "| Field | Type | Required | Nullable | Validations |\n"
    md += "| :--- | :--- | :--- | :--- | :--- |\n"
    for f in fields:
        req = "✅ Yes" if f["required"] else "❌ No"
        nul = "✅ Yes" if f["nullable"] else "❌ No"
        val = f["validations"] if f["validations"] else "-"
        md += f"| `{f['name']}` | `{f['type']}` | {req} | {nul} | {val} |\n"
    return md

def main():
    service_map = parse_service_interfaces()
    controller_map = parse_controllers(service_map)

    if not os.path.exists(SWAGGER_PATH):
        print(f"Swagger JSON not found at {SWAGGER_PATH}")
        return

    with open(SWAGGER_PATH, "r", encoding="utf-8") as f:
        swagger_data = json.load(f)

    schemas = parse_swagger_schemas(swagger_data)
    paths = swagger_data.get("paths", {})

    tagged_endpoints = {}

    for path, path_info in paths.items():
        if "weatherforecast" in path.lower():
            continue
        for method, method_info in path_info.items():
            if method not in ["get", "post", "put", "delete", "patch"]:
                continue

            tags = method_info.get("tags", ["General"])
            tag = tags[0]

            if tag not in tagged_endpoints:
                tagged_endpoints[tag] = []

            lookup_key = (path.lower(), method.lower())
            
            ctrl_match = controller_map.get(lookup_key)
            if not ctrl_match:
                for (ctrl_path, ctrl_method), ctrl_val in controller_map.items():
                    if ctrl_method == method.lower():
                        pattern_path = re.sub(r"\{\w+\}", r"[^/]+", ctrl_path)
                        if re.match(f"^{pattern_path}$", path.lower()):
                            ctrl_match = ctrl_val
                            break

            roles = ctrl_match["roles"] if ctrl_match else "NEED_VERIFY"
            real_response = ctrl_match["response"] if ctrl_match else "ApiResponse<object>"
            action_name = ctrl_match["action"] if ctrl_match else "NEED_VERIFY"

            # Parse path and query parameters
            path_params = []
            query_params = []
            parameters = method_info.get("parameters", [])
            for param in parameters:
                p_in = param.get("in")
                p_name = param.get("name")
                p_type = param.get("schema", {}).get("type", "string")
                p_req = param.get("required", False)
                p_desc = param.get("description", "")
                
                param_data = {
                    "name": p_name,
                    "type": p_type,
                    "required": p_req,
                    "description": p_desc
                }

                if p_in == "path":
                    path_params.append(param_data)
                elif p_in == "query":
                    query_params.append(param_data)

            # Request Body
            req_body_schema = None
            req_body_content = method_info.get("requestBody", {}).get("content", {})
            json_body = req_body_content.get("application/json", {}) or req_body_content.get("text/json", {})
            multipart_body = req_body_content.get("multipart/form-data", {})

            if json_body:
                ref = json_body.get("schema", {}).get("$ref")
                if not ref and json_body.get("schema", {}).get("type") == "array":
                    ref = json_body.get("schema", {}).get("items", {}).get("$ref")
                    req_body_schema = f"Array<{clean_ref(ref)}>" if ref else "Array<object>"
                else:
                    req_body_schema = clean_ref(ref)
            elif multipart_body:
                ref = multipart_body.get("schema", {}).get("$ref")
                if ref:
                    req_body_schema = f"Multipart ({clean_ref(ref)})"
                else:
                    req_body_schema = "Multipart/FormData (Upload File)"

            summary = method_info.get("summary") or method_info.get("description") or f"Action {action_name}"

            responses = method_info.get("responses", {})
            error_codes = [code for code in ["400", "401", "403", "404", "409"] if code in responses]
            if roles != "Public":
                if "401" not in error_codes: error_codes.append("401 (Unauthorized)")
                if "Public" not in roles and "Authenticated" not in roles and "403" not in error_codes: error_codes.append("403 (Forbidden)")

            tagged_endpoints[tag].append({
                "path": path,
                "method": method.upper(),
                "summary": summary,
                "roles": roles,
                "response": real_response,
                "path_params": path_params,
                "query_params": query_params,
                "request_body": req_body_schema,
                "error_codes": error_codes,
                "action": action_name
            })

    # Generate Markdown Output for API Contract
    markdown = "# PlayCourt API Contract for FrontEnd\n\n"
    markdown += "> **LƯU Ý:** Tài liệu này dành cho lập trình viên FrontEnd phát triển ứng dụng PlayCourt mà không cần đọc mã nguồn backend.\n"
    markdown += "> Toàn bộ các API đều trả về dạng bọc chuẩn:\n"
    markdown += "> - **ApiResponse<T>**: Định dạng phản hồi đơn.\n"
    markdown += "> - **PagedResponse<T>**: Định dạng phản hồi danh sách kèm phân trang.\n\n"
    
    markdown += "## 1. Định dạng Phản hồi Chung (Common Wrappers)\n\n"
    markdown += "### ApiResponse<T>\n"
    markdown += "```typescript\ninterface ApiResponse<T> {\n  success: boolean;   // true nếu thành công, false nếu có lỗi\n  message: string;   // Thông điệp kết quả hoặc thông điệp lỗi\n  data: T | null;    // Dữ liệu trả về (null nếu thất bại)\n  errors: string[];  // Danh sách các lỗi chi tiết (validate, nghiệp vụ)\n}\n```\n\n"
    
    markdown += "### PagedResponse<T>\n"
    markdown += "```typescript\ninterface PagedResponse<T> {\n  success: boolean;\n  message: string;\n  data: T | null;      // Thường là Array các DTO (ví dụ VenueDto[])\n  errors: string[];\n  totalCount: number;  // Tổng số bản ghi khớp bộ lọc trên server\n  totalPages: number;  // Tổng số trang\n  pageIndex: number;   // Trang hiện tại (1-indexed)\n  pageSize: number;    // Kích thước trang\n}\n```\n\n"

    markdown += "## 2. Danh sách Endpoints theo Module\n\n"

    for tag in sorted(tagged_endpoints.keys()):
        markdown += f"## Module: {tag}\n\n"
        
        for ep in tagged_endpoints[tag]:
            markdown += f"### `[{ep['method']}]` {ep['path']}\n"
            markdown += f"- **Mô tả:** {ep['summary']}\n"
            
            auth_str = "Public (Không yêu cầu)" if ep['roles'] == 'Public' else f"Yêu cầu đăng nhập (Role/Policy: `{ep['roles']}`)"
            markdown += f"- **Xác thực:** {auth_str}\n"
            
            if ep["path_params"]:
                markdown += "- **Path Parameters:**\n"
                for p in ep["path_params"]:
                    markdown += f"  - `{p['name']}` ({p['type']}): {p['description'] or 'Tham số đường dẫn'}\n"
            
            if ep["query_params"]:
                markdown += "- **Query Parameters:**\n"
                for p in ep["query_params"]:
                    req_str = " (Required)" if p["required"] else ""
                    markdown += f"  - `{p['name']}` ({p['type']}){req_str}: {p['description'] or 'Bộ lọc query'}\n"
            
            if ep["request_body"]:
                markdown += f"- **Request Body:** `{ep['request_body']}`\n"
            else:
                markdown += "- **Request Body:** Không có (Trống)\n"

            markdown += f"- **Response Shape:** `{ep['response']}`\n"
            
            if ep["error_codes"]:
                markdown += f"- **Error Cases:** {', '.join(ep['error_codes'])}\n"
                
            suggested_screen = "NEED_VERIFY"
            p_lower = ep['path'].lower()
            m_lower = ep['method'].lower()
            
            if "auth/login" in p_lower:
                suggested_screen = "Màn hình Đăng nhập (Login)"
            elif "auth/register" in p_lower:
                suggested_screen = "Màn hình Đăng ký (Register)"
            elif "auth/reset-password" in p_lower or "auth/forgot-password" in p_lower:
                suggested_screen = "Màn hình Đặt lại mật khẩu (Reset Password)"
            elif "auth/verify-email" in p_lower or "auth/resend-verify-email" in p_lower:
                suggested_screen = "Màn hình Xác thực Email (Verify Email)"
            elif "auth/change-password" in p_lower:
                suggested_screen = "Màn hình Cá nhân - Đổi mật khẩu"
            elif "auth/refresh-token" in p_lower or "auth/logout" in p_lower:
                suggested_screen = "Hệ thống xác thực ngầm (Auth Service)"
            elif "court-owners" in p_lower:
                suggested_screen = "Màn hình Admin - Quản lý Chủ sân"
            elif "courtschedules" in p_lower or "court-schedules" in p_lower or "schedules" in p_lower:
                suggested_screen = "Màn hình Chủ sân - Quản lý Lịch đóng/mở sân"
            elif "pricingrules" in p_lower or "pricing-rules" in p_lower:
                suggested_screen = "Màn hình Chủ sân - Thiết lập Bảng giá"
            elif "bookings/me" in p_lower:
                suggested_screen = "Màn hình Lịch sử đặt sân (Booking History)"
            elif "bookings" in p_lower and m_lower == "post":
                suggested_screen = "Màn hình Đặt sân - Thanh toán (Checkout)"
            elif "bookings" in p_lower and ("cancel" in p_lower or "confirm" in p_lower or "reject" in p_lower or "complete" in p_lower):
                suggested_screen = "Màn hình Quản lý đặt sân (Booking Details / History)"
            elif "bookings" in p_lower:
                suggested_screen = "Màn hình Lịch sử đặt sân (Booking History) / Quản lý đặt sân của cơ sở"
            elif "venues/my" in p_lower or "venues/stats" in p_lower:
                suggested_screen = "Màn hình Dashboard Chủ sân - Quản lý Cơ sở"
            elif "venues/admin" in p_lower or "venues/{id}/status" in p_lower:
                suggested_screen = "Màn hình Admin - Quản lý & Phê duyệt Cơ sở"
            elif "venues" in p_lower and m_lower == "get" and "favorites" in p_lower:
                suggested_screen = "Màn hình Sân yêu thích (Favorites)"
            elif "venues" in p_lower and m_lower == "get" and "{" not in p_lower:
                suggested_screen = "Màn hình Tìm kiếm Sân / Trang chủ (Home)"
            elif "venues" in p_lower and m_lower == "get" and "{" in p_lower:
                suggested_screen = "Màn hình Chi tiết Sân / Cơ sở"
            elif "venues" in p_lower and m_lower == "post":
                suggested_screen = "Màn hình Chủ sân - Đăng ký Cơ sở kinh doanh mới"
            elif "venues" in p_lower and (m_lower in ["put", "delete"]):
                suggested_screen = "Màn hình Chủ sân - Quản lý & Thiết lập Cơ sở"
            elif "venues" in p_lower and ("amenities" in p_lower or "images" in p_lower or "opening-hours" in p_lower or "staff" in p_lower or "favorites" in p_lower):
                suggested_screen = "Màn hình Chủ sân - Quản lý tiện ích / Ảnh / Giờ hoạt động / Nhân viên"
            elif "matches" in p_lower:
                suggested_screen = "Màn hình Giao lưu Kèo đấu (Matches / Social)"
            elif "notifications" in p_lower:
                suggested_screen = "Trung tâm thông báo (Notification Center)"
            elif "reviews" in p_lower:
                suggested_screen = "Màn hình Đánh giá & Phản hồi (Reviews)"
            elif "sports" in p_lower:
                suggested_screen = "Màn hình Admin - Quản lý môn thể thao / Màn hình cá nhân (chọn môn yêu thích)"
            elif "courts" in p_lower:
                if "availability" in p_lower or "rating-stats" in p_lower:
                    suggested_screen = "Màn hình Chi tiết Sân / Cơ sở"
                else:
                    suggested_screen = "Màn hình Chủ sân - Quản lý Sân nhỏ"
            elif "users" in p_lower:
                suggested_screen = "Màn hình Thông tin cá nhân (Profile)"
            elif "amenities" in p_lower:
                suggested_screen = "Màn hình Admin - Quản lý tiện ích hệ thống"
            elif "payments" in p_lower:
                suggested_screen = "Màn hình Đặt sân - Thanh toán (Checkout) / Lịch sử giao dịch"
            
            markdown += f"- **FE Screen đề xuất:** {suggested_screen}\n"
            markdown += "\n"

    markdown += "## 3. Chi tiết cấu trúc Request/Response DTO (Data Transfer Objects)\n\n"
    for schema_name, fields in sorted(schemas.items()):
        if isinstance(fields, dict) and fields.get("type") == "enum":
            continue
            
        markdown += f"### {schema_name}\n"
        markdown += format_dto_fields(fields)
        markdown += "\n"

    with open(OUTPUT_CONTRACT, "w", encoding="utf-8") as f:
        f.write(markdown)
    print(f"Generated API Contract doc at: {OUTPUT_CONTRACT}")
    generate_enums_doc()

if __name__ == "__main__":
    main()
