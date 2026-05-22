using PlayCourt.Application.Common.Responses;

namespace PlayCourt.API.Middlewares
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Cho request chạy tiếp qua các middleware/controller phía sau.
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unhandled exception occurred.");

                if (context.Response.HasStarted)
                {
                    // Nếu response đã bắt đầu gửi, để ASP.NET Core xử lý tiếp exception.
                    throw;
                }

                // Format lỗi bất ngờ theo ApiResponse để client nhận shape thống nhất.
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = ApiResponse<object?>.Fail(
                    "An unexpected error occurred.",
                    ["Internal server error."]);

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
