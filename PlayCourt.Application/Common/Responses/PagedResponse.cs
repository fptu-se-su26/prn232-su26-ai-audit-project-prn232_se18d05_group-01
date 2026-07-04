namespace PlayCourt.Application.Common.Responses
{
    /// <summary>
    /// Wrapper response dành cho danh sách kết quả có phân trang.
    /// Bổ sung TotalCount so với ApiResponse&lt;T&gt; thông thường.
    /// </summary>
    public sealed class PagedResponse<T>
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;
        public T? Data { get; init; }
        public IEnumerable<string> Errors { get; init; } = [];

        /// <summary>Tổng số bản ghi khớp bộ lọc (không tính phân trang).</summary>
        public int TotalCount { get; init; }

        /// <summary>Tổng số trang dựa trên PageSize.</summary>
        public int TotalPages { get; init; }

        /// <summary>Trang hiện tại (1-indexed).</summary>
        public int PageIndex { get; init; }

        /// <summary>Số bản ghi tối đa mỗi trang.</summary>
        public int PageSize { get; init; }

        public static PagedResponse<T> Ok(
            T? data,
            int totalCount,
            int pageIndex,
            int pageSize,
            string message = "Success")
        {
            var totalPages = pageSize > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0;
            return new PagedResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public static PagedResponse<T> Fail(string message, IEnumerable<string>? errors = null)
        {
            return new PagedResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? []
            };
        }
    }
}
