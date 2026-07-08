# PlayCourt FrontEnd Screen to API Endpoint Mapping

Tài liệu này ánh xạ các màn hình và tác vụ nghiệp vụ trên FrontEnd sang các API backend tương ứng trong dự án PlayCourt.

---

## 1. Phân hệ Khách hàng / Người chơi (Player Flow)

### 1.1. Xác thực & Tài khoản (Auth & Account)
| Tên Giao diện / Tác vụ | API Endpoint | HTTP Method | Ghi chú |
| :--- | :--- | :--- | :--- |
| **Đăng ký tài khoản mới** | `/api/Auth/register` | `POST` | Gửi thông tin đăng ký (bao gồm vai trò Player hoặc CourtOwner) |
| **Đăng nhập hệ thống** | `/api/Auth/login` | `POST` | Trả về `accessToken` và `refreshToken` |
| **Gửi lại mã xác thực email** | `/api/Auth/resend-verify-email` | `POST` | Gửi email kích hoạt tài khoản |
| **Xác thực email** | `/api/Auth/verify-email` | `POST` | Kích hoạt tài khoản qua token nhận được từ email |
| **Yêu cầu đặt lại mật khẩu** | `/api/Auth/forgot-password` | `POST` | Nhập email nhận OTP khôi phục mật khẩu |
| **Đặt lại mật khẩu với OTP** | `/api/Auth/reset-password` | `POST` | Nhập mật khẩu mới kèm OTP nhận được |
| **Làm mới Token (Silent Refresh)** | `/api/Auth/refresh-token` | `POST` | Gọi ngầm khi Access Token hết hạn |
| **Đăng xuất** | `/api/Auth/logout` | `POST` | Xóa Session, hủy Refresh Token trên server |
| **Đổi mật khẩu** | `/api/Auth/change-password` | `POST` | Đổi mật khẩu khi đã đăng nhập |

### 1.2. Trang chủ & Tìm kiếm Sân (Home & Search Venues)
| Tên Giao diện / Tác vụ | API Endpoint | HTTP Method | Ghi chú |
| :--- | :--- | :--- | :--- |
| **Xem danh sách môn thể thao** | `/api/Sports` | `GET` | Hiển thị các bộ lọc môn thể thao (Cầu lông, Bóng đá, Tennis...) |
| **Danh sách cơ sở sân (Tìm kiếm & Lọc)** | `/api/Venues` | `GET` | Lọc theo tên, địa chỉ, latitude/longitude, khoảng cách... |
| **Danh sách cơ sở yêu thích của tôi** | `/api/Venues/favorites/my` | `GET` | Hiển thị các sân người dùng đã thả tim |
| **Thêm sân vào danh sách yêu thích** | `/api/Venues/{id}/favorites` | `POST` | Thả tim sân |
| **Xóa sân khỏi danh sách yêu thích** | `/api/Venues/{id}/favorites` | `DELETE` | Bỏ thả tim sân |

### 1.3. Chi tiết Sân & Đặt sân (Venue Details & Booking)
| Tên Giao diện / Tác vụ | API Endpoint | HTTP Method | Ghi chú |
| :--- | :--- | :--- | :--- |
| **Xem chi tiết cơ sở sân** | `/api/Venues/{id}` | `GET` | Thông tin mô tả, địa chỉ, hotline, giờ mở/đóng cửa... |
| **Xem danh sách dịch vụ đi kèm (Amenities)**| `/api/Amenities` | `GET` | Hiển thị các tiện ích (Gửi xe, Wifi, Nước uống, Tắm rửa...) |
| **Xem danh sách sân nhỏ thuộc cơ sở** | `/api/venues/{venueId}/courts` | `GET` | Lấy các sân đơn lẻ (ví dụ Sân cầu lông 1, Sân 2...) |
| **Xem khung giờ hoạt động của cơ sở** | `/api/venues/{id}/opening-hours` | `GET` | Xem giờ mở cửa từng ngày trong tuần |
| **Kiểm tra lịch rảnh của một sân nhỏ** | `/api/courts/{courtId}/availability` | `GET` | FE truyền `StartAt` và `EndAt` để check khung giờ còn trống |
| **Xem giá tiền theo khung giờ** | `/api/courts/{courtId}/pricing-rules` | `GET` | Lấy các quy tắc giá (Price per hour) để tính toán số tiền |
| **Tạo đơn đặt sân (Booking)** | `/api/Bookings` | `POST` | Khách chọn sân, ngày, giờ bắt đầu, giờ kết thúc và bấm Đặt |
| **Xem chi tiết đơn đặt sân** | `/api/Bookings/{id}` | `GET` | Hiển thị chi tiết thông tin thanh toán, sân đặt, thời gian |

### 1.4. Thanh toán (Payment - PayOS Flow)
| Tên Giao diện / Tác vụ | API Endpoint | HTTP Method | Ghi chú |
| :--- | :--- | :--- | :--- |
| **Tạo link thanh toán PayOS** | `/api/payments/bookings/{bookingId}/payos` | `POST` | FE gọi sau khi đặt sân thành công để lấy link thanh toán chuyển hướng người dùng sang cổng PayOS |
| **Kiểm tra/Đồng bộ trạng thái thanh toán**| `/api/payments/bookings/{bookingId}/sync-payos`| `POST` | FE gọi khi người dùng quay lại trang (Redirect URL) để kiểm tra giao dịch thực tế |
| **Xem lịch sử thanh toán của đơn đặt** | `/api/payments/bookings/{bookingId}` | `GET` | Hiển thị các giao dịch liên quan đến booking |

### 1.5. Quản lý Đặt chỗ & Đánh giá (Booking History & Reviews)
| Tên Giao diện / Tác vụ | API Endpoint | HTTP Method | Ghi chú |
| :--- | :--- | :--- | :--- |
| **Lịch sử đặt sân của tôi** | `/api/Bookings/me` | `GET` | Hiển thị danh sách các sân đã đặt (Chờ thanh toán, Đã xác nhận, Đã chơi...) |
| **Khách hàng tự hủy đơn đặt** | `/api/Bookings/{id}/cancel` | `PATCH` | Cho phép hủy khi trạng thái còn hợp lệ (chưa quá hạn/đã thanh toán tùy rule) |
| **Viết đánh giá sau khi chơi** | `/api/Reviews` | `POST` | Đánh giá sao (1-5) kèm nội dung chữ và hình ảnh |
| **Thêm ảnh vào bài đánh giá** | `/api/Reviews/{id}/images` | `POST` | Tải ảnh lên đính kèm đánh giá |
| **Xóa ảnh khỏi bài đánh giá** | `/api/Reviews/{id}/images/{imageId}` | `DELETE` | Xóa ảnh đã upload |
| **Xem các đánh giá của cơ sở** | `/api/venues/{venueId}/reviews` | `GET` | Hiển thị ở trang chi tiết cơ sở |
| **Xem các đánh giá của một sân cụ thể** | `/api/courts/{courtId}/reviews` | `GET` | Lọc đánh giá theo sân nhỏ |
| **Báo cáo đánh giá xấu/vi phạm** | `/api/reviews/{id}/report` | `POST` | Người chơi hoặc chủ sân báo cáo bài đánh giá không trung thực |

### 1.6. Cộng đồng & Kèo đấu (Social - Matchmaking)
| Tên Giao diện / Tác vụ | API Endpoint | HTTP Method | Ghi chú |
| :--- | :--- | :--- | :--- |
| **Tìm kiếm kèo đấu** | `/api/Matches` | `GET` | Tìm các kèo đang tuyển người chơi cùng |
| **Xem kèo đấu đề xuất cho tôi** | `/api/Matches/recommended` | `GET` | Gợi ý kèo dựa trên trình độ (SkillLevel) và vị trí của người dùng |
| **Tạo kèo đấu mới** | `/api/Matches` | `POST` | Người dùng lên lịch tạo kèo và tuyển người chơi cùng |
| **Xem chi tiết kèo đấu** | `/api/Matches/{id}` | `GET` | Hiển thị thông tin kèo đấu, danh sách thành viên tham gia |
| **Cập nhật thông tin kèo đấu** | `/api/Matches/{id}` | `PUT` | Sửa mô tả kèo, trình độ yêu cầu... |
| **Hủy kèo đấu** | `/api/Matches/{id}/cancel` | `PATCH` | Hủy kèo khi không đủ người chơi hoặc lý do khác |
| **Gửi yêu cầu tham gia kèo** | `/api/Matches/{id}/join-requests` | `POST` | Người chơi xin slot tham gia kèo |
| **Rời khỏi kèo đấu** | `/api/Matches/{id}/participants/me` | `DELETE`| Người chơi tự xin rút khỏi kèo đấu |
| **Xem danh sách yêu cầu xin vào kèo** | `/api/Matches/{id}/join-requests` | `GET` | Chủ kèo duyệt danh sách người đăng ký |
| **Duyệt/Từ chối người chơi xin vào kèo** | `/api/Matches/{id}/join-requests/{requestId}`| `PATCH` | Chấp nhận hoặc từ chối đơn đăng ký của người khác |
| **Mời người chơi khác tham gia kèo** | `/api/Matches/{id}/invitations` | `POST` | Gửi lời mời trực tiếp đến người dùng khác |
| **Xem danh sách lời mời của tôi** | `/api/Matches/invitations/me` | `GET` | Xem ai đã mời mình chơi kèo |
| **Đồng ý/Từ chối lời mời chơi kèo** | `/api/Matches/invitations/{invitationId}`| `PATCH` | Chấp nhận hoặc từ chối lời mời từ người khác |

---

## 2. Phân hệ Chủ sân (Court Owner Flow)

### 2.1. Quản lý Cơ sở sân (Venue Management)
| Tên Giao diện / Tác vụ | API Endpoint | HTTP Method | Ghi chú |
| :--- | :--- | :--- | :--- |
| **Danh sách cơ sở sân tôi sở hữu** | `/api/venues/my` | `GET` | Hiển thị danh sách sân do mình quản lý |
| **Xem chi tiết sân tôi sở hữu** | `/api/venues/my/{id}` | `GET` | Thông tin chi tiết để cập nhật |
| **Tạo mới cơ sở sân** | `/api/venues` | `POST` | Thêm cơ sở sân mới vào hệ thống (chờ Admin duyệt) |
| **Cập nhật thông tin cơ sở sân** | `/api/venues/{id}` | `PUT` | Sửa tên, địa chỉ, hotline, mô tả... |
| **Thêm ảnh cho cơ sở** | `/api/venues/{id}/images` | `POST` | Upload hình ảnh cơ sở sân |
| **Xóa ảnh cơ sở** | `/api/venues/{id}/images/{imageId}`| `DELETE`| Xóa ảnh cơ sở |
| **Đặt ảnh đại diện cho cơ sở** | `/api/venues/{id}/images/{imageId}/set-cover`| `PATCH`| Chọn ảnh hiển thị chính |
| **Cập nhật thời gian mở cửa tuần** | `/api/venues/{id}/opening-hours`| `PUT` | Điều chỉnh lịch hoạt động mặc định |
| **Thêm tiện ích cho cơ sở** | `/api/venues/{id}/amenities/{amenityId}`| `POST` | Gắn tag tiện ích (ví dụ: Wifi, Quạt, Bãi đỗ xe...) |
| **Gỡ tiện ích khỏi cơ sở** | `/api/venues/{id}/amenities/{amenityId}`| `DELETE`| Gỡ tag tiện ích |

### 2.2. Quản lý Sân nhỏ & Giá (Courts & Pricing)
| Tên Giao diện / Tác vụ | API Endpoint | HTTP Method | Ghi chú |
| :--- | :--- | :--- | :--- |
| **Thêm mới sân nhỏ** | `/api/venues/{venueId}/courts` | `POST` | Tạo sân cụ thể (ví dụ Sân Cầu Lông Số 1) |
| **Cập nhật trạng thái/tên sân** | `/api/courts/{id}` | `PUT` | Đổi tên sân, cấu hình sân trong nhà/ngoài trời |
| **Xóa mềm sân nhỏ** | `/api/courts/{id}` | `DELETE` | Xóa sân (chuyển trạng thái ngừng hoạt động) |
| **Thêm bảng giá theo khung giờ** | `/api/courts/{courtId}/pricing-rules` | `POST` | Cấu hình bảng giá theo ngày/giờ (ví dụ: Giờ cao điểm 100k/h, thường 70k/h) |
| **Cập nhật bảng giá** | `/api/pricing-rules/{id}` | `PUT` | Thay đổi khoảng thời gian áp dụng hoặc mức giá |
| **Xóa bảng giá** | `/api/pricing-rules/{id}` | `DELETE` | Hủy áp dụng mức giá |

### 2.3. Lịch đặt sân & Bảo trì (Schedules & Operations)
| Tên Giao diện / Tác vụ | API Endpoint | HTTP Method | Ghi chú |
| :--- | :--- | :--- | :--- |
| **Xem lịch đặt sân của cơ sở** | `/api/venues/{venueId}/bookings`| `GET` | Theo dõi lịch đặt sân của khách hàng |
| **Chủ sân duyệt đơn đặt** | `/api/Bookings/{id}/confirm` | `PATCH` | Xác nhận lịch đặt sân của khách |
| **Chủ sân từ chối đơn đặt** | `/api/Bookings/{id}/reject` | `PATCH` | Từ chối đặt sân (đơn hết hạn, lý do đột xuất) |
| **Đánh dấu đơn đặt hoàn thành** | `/api/Bookings/{id}/complete` | `PATCH` | Xác nhận khách đã đến chơi xong |
| **Xem lịch đóng cửa bảo trì sân** | `/api/courts/{courtId}/schedules` | `GET` | Xem danh sách lịch đóng cửa sân nhỏ |
| **Lên lịch đóng cửa bảo trì sân** | `/api/courts/{courtId}/schedules` | `POST` | Khóa sân đột xuất / bảo trì (tránh khách đặt vào) |
| **Hủy lịch bảo trì sân** | `/api/court-schedules/{id}` | `DELETE` | Mở lại sân sớm hơn dự kiến |

### 2.4. Quản lý Nhân viên & Thống kê (Staff & Dashboard)
| Tên Giao diện / Tác vụ | API Endpoint | HTTP Method | Ghi chú |
| :--- | :--- | :--- | :--- |
| **Xem thống kê doanh thu chủ sân** | `/api/venues/stats` | `GET` | Hiển thị doanh thu, số lượt đặt sân, tỷ lệ lấp đầy sân |
| **Xem danh sách nhân viên cơ sở** | `/api/venues/{venueId}/staff` | `GET` | Quản lý nhân sự làm việc tại sân |
| **Thêm nhân viên mới** | `/api/venues/{venueId}/staff` | `POST` | Phân quyền nhân viên (Lễ tân, Quản lý...) |
| **Xóa nhân viên** | `/api/venues/{venueId}/staff/{staffId}`| `DELETE`| Gỡ quyền truy cập của nhân viên |

---

## 3. Phân hệ Quản trị viên (Admin Flow)

### 3.1. Phê duyệt Chủ sân & Cơ sở (Approvals)
| Tên Giao diện / Tác vụ | API Endpoint | HTTP Method | Ghi chú |
| :--- | :--- | :--- | :--- |
| **Danh sách đăng ký làm chủ sân** | `/api/court-owners` | `GET` | Xem hồ sơ đăng ký kinh doanh của chủ sân |
| **Xem chi tiết hồ sơ chủ sân** | `/api/court-owners/{id}` | `GET` | Xem giấy phép kinh doanh, thông tin liên hệ |
| **Duyệt/Từ chối tài khoản chủ sân** | `/api/court-owners/{id}/verification-status`| `PATCH` | Duyệt cho phép kinh doanh hoặc từ chối |
| **Danh sách tất cả cơ sở sân** | `/api/venues/admin` | `GET` | Quản lý toàn bộ cơ sở sân của hệ thống |
| **Xem chi tiết sân phía Admin** | `/api/venues/admin/{id}` | `GET` | Đối chiếu thông tin phê duyệt |
| **Duyệt/Đình chỉ hoạt động cơ sở sân**| `/api/venues/{id}/status` | `PATCH` | Phê duyệt hiển thị sân lên hệ thống hoặc tạm khóa do vi phạm |

### 3.2. Quản lý hệ thống (System Settings)
| Tên Giao diện / Tác vụ | API Endpoint | HTTP Method | Ghi chú |
| :--- | :--- | :--- | :--- |
| **Quản lý tiện ích (Amenities)** | `/api/Amenities` | `POST` / `PUT` / `DELETE` | Tạo, sửa, xóa các tag tiện ích hệ thống |
| **Quản lý môn thể thao (Sports)** | `/api/Sports` | `POST` / `PUT` | Tạo môn thể thao mới, đổi tên hoặc khóa hoạt động |
| **Kích hoạt/Khóa môn thể thao** | `/api/sports/{id}/toggle-active`| `PATCH` | Đóng/mở nhận kèo/đặt sân môn thể thao này |
| **Xem phản hồi vi phạm/Đánh giá vi phạm**| `/api/venues/{venueId}/reviews` | `GET` | Quản trị viên rà soát các review bị báo cáo xấu |
| **Ẩn/Hiện bài viết đánh giá** | `/api/admin/reviews/{id}/moderate`| `PUT` | Xử lý đánh giá vi phạm (ẩn khỏi app) |
| **Quản lý danh sách người dùng** | `/api/Users` | `GET` (NEED_VERIFY) | Lấy danh sách thành viên hệ thống |
| **Khóa tài khoản người dùng** | `/api/Users/{id}` | `DELETE` (NEED_VERIFY) | Khóa tài khoản vi phạm chính sách |
