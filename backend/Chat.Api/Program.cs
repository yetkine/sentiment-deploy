using Chat.Api.Data;
using Chat.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Db
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS (React & RN dev adresleri)
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("dev", p => p
        .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173", "http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

// AI HttpClient (FastAPI servis)
var aiBase = builder.Configuration["AiService:BaseUrl"] ?? "http://127.0.0.1:8000";
builder.Services.AddHttpClient<SentimentClient>(client =>
{
    client.BaseAddress = new Uri(aiBase); // örn: http://127.0.0.1:8000
    client.Timeout = TimeSpan.FromSeconds(20);
});

// >>>>>>> BURAYA KADAR service registration
var app = builder.Build(); // <<<<<<<< app burada oluşturuluyor

// Middleware pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("dev");

app.MapControllers();

// basit health check
app.MapGet("/health", () => "ok");

app.Run();
