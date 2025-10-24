// Program.cs
using Chat.Api.Data;
using Chat.Api.Models;
using Chat.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Db ---
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// --- Controllers + Swagger ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- CORS ---
var prodOrigins = (builder.Configuration["Cors:Origins"] ?? "")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

string[] devOrigins =
{
    "http://localhost:5173",
    "http://127.0.0.1:5173",
    "http://localhost:3000"
};

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("default", p =>
    {
        if (prodOrigins.Length > 0)
            p.WithOrigins(prodOrigins).AllowAnyHeader().AllowAnyMethod();
        else
            p.WithOrigins(devOrigins).AllowAnyHeader().AllowAnyMethod();
    });
});

// --- AI HttpClient ---
var aiBase = builder.Configuration["AiService:BaseUrl"] ?? "http://127.0.0.1:8000";
builder.Services.AddHttpClient<SentimentClient>(client =>
{
    client.BaseAddress = new Uri(aiBase);
    client.Timeout = TimeSpan.FromSeconds(20);
});

var app = builder.Build();

// --- DB create + seed (TEK BLOK) ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Render’da migration yoksa:
    await db.Database.EnsureCreatedAsync();

    // Varsayılan kullanıcı yoksa ekle (Nickname ZORUNLU!)
    if (!await db.Users.AnyAsync())
    {
        db.Users.Add(new User
        {
            Name = "Demo User",
            Nickname = "demo",          // <— NOT NULL alanı doldur
            CreatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }
}

// --- Middleware ---
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("default");
app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => "ok");

app.Run();
