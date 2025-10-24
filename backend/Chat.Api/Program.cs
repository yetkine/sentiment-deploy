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

// ---- CORS ----
// Render/Prod için: "Cors__Origins" (virgülle ayrılmış) env var'ından oku
// Örn: https://sentiment-deploy.vercel.app
var prodOrigins = (builder.Configuration["Cors:Origins"] ?? "")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

// Dev için localhost adresleri
string[] devOrigins = new[]
{
    "http://localhost:5173",
    "http://127.0.0.1:5173",
    "http://localhost:3000"
};

// Tek bir "default" policy: prod’da env’den; yoksa dev listesi; o da yoksa (fallback) allow-any
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("default", p =>
    {
        if (prodOrigins.Length > 0)
            p.WithOrigins(prodOrigins).AllowAnyHeader().AllowAnyMethod();
        else
            p.WithOrigins(devOrigins).AllowAnyHeader().AllowAnyMethod();
        // İstersen .AllowCredentials() ekleyebilirsin (cookie/credential gerekiyorsa)
    });
});

// AI HttpClient
var aiBase = builder.Configuration["AiService:BaseUrl"] ?? "http://127.0.0.1:8000";
builder.Services.AddHttpClient<SentimentClient>(client =>
{
    client.BaseAddress = new Uri(aiBase);
    client.Timeout = TimeSpan.FromSeconds(20);
});

var app = builder.Build();

// Middleware pipeline (sıra önemli)
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("default");          // 👉 CORS, MapControllers'tan ÖNCE
app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => "ok");

app.Run();
