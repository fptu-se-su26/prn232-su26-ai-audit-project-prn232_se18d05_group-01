using PlayCourt.API;
using PlayCourt.Application;
using PlayCourt.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Gom đăng ký service theo từng layer để Program.cs gọn và team dễ mở rộng.
builder.Services
    .AddApiServices()
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Gom HTTP pipeline trong PlayCourt.API/DependencyInjection.cs.
app.UseApiPipeline();

app.Run();

public partial class Program;
