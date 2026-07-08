# PlayCourt System Enums Contract

Tài liệu này định nghĩa toàn bộ Enum trong hệ thống PlayCourt, giá trị số (numeric value), tên code, và nhãn hiển thị Tiếng Việt đề xuất trên UI.

## UserRole

| Code Option | Numeric Value | Label Tiếng Việt | Ghi chú |
| :--- | :--- | :--- | :--- |
| `Admin` | `0` | Quản trị viên | |
| `Player` | `1` | Người chơi | |
| `CourtOwner` | `2` | Chủ sân | |

## UserStatus

| Code Option | Numeric Value | Label Tiếng Việt | Ghi chú |
| :--- | :--- | :--- | :--- |
| `Active` | `0` | Hoạt động | |
| `Locked` | `1` | Bị khóa | |
| `Inactive` | `2` | Chưa kích hoạt | |

## Gender

| Code Option | Numeric Value | Label Tiếng Việt | Ghi chú |
| :--- | :--- | :--- | :--- |
| `Male` | `0` | Nam | |
| `Female` | `1` | Nữ | |
| `Other` | `2` | Khác | |

## CourtOwnerVerificationStatus

| Code Option | Numeric Value | Label Tiếng Việt | Ghi chú |
| :--- | :--- | :--- | :--- |
| `Pending` | `0` | Chờ duyệt | |
| `Approved` | `1` | Đã duyệt | |
| `Rejected` | `2` | Đã từ chối | |

## SkillLevel

| Code Option | Numeric Value | Label Tiếng Việt | Ghi chú |
| :--- | :--- | :--- | :--- |
| `Beginner` | `0` | Mới chơi | |
| `Intermediate` | `1` | Trung bình | |
| `Advanced` | `2` | Nâng cao | |

## VenueStatus

| Code Option | Numeric Value | Label Tiếng Việt | Ghi chú |
| :--- | :--- | :--- | :--- |
| `Pending` | `0` | Chờ duyệt | |
| `Approved` | `1` | Đã hoạt động | |
| `Rejected` | `2` | Từ chối | |
| `Suspended` | `3` | Tạm dừng | |

## CourtStatus

| Code Option | Numeric Value | Label Tiếng Việt | Ghi chú |
| :--- | :--- | :--- | :--- |
| `Available` | `0` | Sẵn sàng | |
| `Maintenance` | `1` | Bảo trì | |
| `Inactive` | `2` | Ngưng hoạt động | |

## VenueStaffRole

| Code Option | Numeric Value | Label Tiếng Việt | Ghi chú |
| :--- | :--- | :--- | :--- |
| `Manager` | `0` | Quản lý | |
| `Receptionist` | `1` | Lễ tân | |
| `Accountant` | `2` | Kế toán | |

## BookingStatus

| Code Option | Numeric Value | Label Tiếng Việt | Ghi chú |
| :--- | :--- | :--- | :--- |
| `Pending` | `0` | Chờ thanh toán | |
| `Confirmed` | `1` | Đã xác nhận | |
| `CancelledByUser` | `2` | Khách hủy | |
| `CancelledByOwner` | `3` | Chủ sân hủy | |
| `Completed` | `4` | Hoàn thành | |
| `Expired` | `5` | Hết hạn | |

## PaymentType

| Code Option | Numeric Value | Label Tiếng Việt | Ghi chú |
| :--- | :--- | :--- | :--- |
| `BookingPayment` | `0` | Thanh toán đặt sân | |
| `Refund` | `1` | Hoàn tiền | |
| `Payout` | `2` | Rút tiền/Quyết toán | |

## PaymentStatus

| Code Option | Numeric Value | Label Tiếng Việt | Ghi chú |
| :--- | :--- | :--- | :--- |
| `Pending` | `0` | Chờ thanh toán | |
| `Success` | `1` | Thành công | |
| `Failed` | `2` | Thất bại | |

## MatchStatus

| Code Option | Numeric Value | Label Tiếng Việt | Ghi chú |
| :--- | :--- | :--- | :--- |
| `Open` | `0` | Đang tuyển | |
| `Full` | `1` | Đã đầy | |
| `Cancelled` | `2` | Đã hủy | |
| `Completed` | `3` | Đã đá/Đã chơi | |

## MatchJoinRequestStatus

| Code Option | Numeric Value | Label Tiếng Việt | Ghi chú |
| :--- | :--- | :--- | :--- |
| `Pending` | `0` | Chờ duyệt | |
| `Approved` | `1` | Đã tham gia | |
| `Rejected` | `2` | Bị từ chối | |

## MatchInvitationStatus

| Code Option | Numeric Value | Label Tiếng Việt | Ghi chú |
| :--- | :--- | :--- | :--- |
| `Pending` | `0` | Chờ phản hồi | |
| `Accepted` | `1` | Đồng ý | |
| `Declined` | `2` | Từ chối | |
| `Cancelled` | `3` | Đã thu hồi | |

## ReviewStatus

| Code Option | Numeric Value | Label Tiếng Việt | Ghi chú |
| :--- | :--- | :--- | :--- |
| `Visible` | `0` | Hiển thị | |
| `Hidden` | `1` | Bị ẩn | |
| `Reported` | `2` | Bị báo cáo | |

## NotificationType

| Code Option | Numeric Value | Label Tiếng Việt | Ghi chú |
| :--- | :--- | :--- | :--- |
| `Booking` | `0` | Đặt sân | |
| `Match` | `1` | Kèo đấu | |
| `Payment` | `2` | Thanh toán | |
| `Review` | `3` | Đánh giá | |
| `System` | `4` | Hệ thống | |

## NotificationReferenceType

| Code Option | Numeric Value | Label Tiếng Việt | Ghi chú |
| :--- | :--- | :--- | :--- |
| `Booking` | `0` | Đặt sân | |
| `Match` | `1` | Kèo đấu | |
| `Payment` | `2` | Thanh toán | |
| `Review` | `3` | Đánh giá | |
| `Venue` | `4` | Cơ sở sân | |

## VerificationTokenPurpose

| Code Option | Numeric Value | Label Tiếng Việt | Ghi chú |
| :--- | :--- | :--- | :--- |
| `EmailVerification` | `0` | Xác thực email | |
| `PasswordReset` | `1` | Đặt lại mật khẩu | |

