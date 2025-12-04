using Microsoft.EntityFrameworkCore;
using RESTfull.Domain.Entities;
using RESTfull.Infrastructure.Data;



var builder = WebApplication.CreateBuilder(args);

// Make Kestrel listen on both HTTPS and HTTP addresses to match client
builder.WebHost.UseUrls("https://localhost:7187", "http://localhost:5064");

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    )
);

// CORS
var allowedOrigins = "_allowedOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowedOrigins,
        policy =>
        {
            policy.WithOrigins("https://localhost:7280", "http://localhost:5005")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});



// Репозитории
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
// или конкретные:
// builder.Services.AddScoped<IStudentRepository, StudentRepository>();

// Контроллеры и Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Выполняем миграции и заполняем тестовые данные перед запуском приложения
try
{
    using (var scope = app.Services.CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        try
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database migration/seed failed");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Database initialization error: {ex}");
}

// Подключаем CORS до MapControllers()
app.UseCors(allowedOrigins);

// Swagger (только в Dev)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

