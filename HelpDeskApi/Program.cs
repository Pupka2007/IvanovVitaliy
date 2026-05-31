using HelpDeskApi.Repositories;
using HelpDeskApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Явно указываем порты
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); // HTTP порт
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Добавляем Swagger

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var dataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "requests.json");
Directory.CreateDirectory(Path.GetDirectoryName(dataPath)!);

builder.Services.AddSingleton<IRequestRepository>(new JsonRequestRepository(dataPath));
builder.Services.AddScoped<IRequestService, RequestService>();

var app = builder.Build();

// Включаем Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HelpDesk API v1");
    c.RoutePrefix = "swagger"; // Swagger будет доступен по /swagger
});

app.UseCors();
app.MapControllers();

Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
Console.WriteLine("║     HelpDesk API - Система учета обращений в техподдержку   ║");
Console.WriteLine("║                    Автор: Ivanov Vitaliy                     ║");
Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
Console.WriteLine("║  Доступные адреса:                                           ║");
Console.WriteLine($"║  • Swagger UI: http://localhost:5000/swagger                 ║");
Console.WriteLine($"║  • Все обращения: http://localhost:5000/api/requests        ║");
Console.WriteLine($"║  • Обращение по ID: http://localhost:5000/api/requests/{{id}} ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
Console.WriteLine();

app.Run();