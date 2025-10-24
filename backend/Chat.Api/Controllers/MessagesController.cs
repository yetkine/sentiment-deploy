using Chat.Api.Contracts;
using Chat.Api.Data;
using Chat.Api.Models;
using Chat.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chat.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly SentimentClient _ai;

    public MessagesController(AppDbContext db, SentimentClient ai)
    {
        _db = db;
        _ai = ai;
    }

    // Yeni mesaj oluşturur ve duygu analizi sonucunu ekler
    [HttpPost]
    public async Task<ActionResult<Message>> Create(CreateMessageRequest req, CancellationToken ct)
    {
        var user = await _db.Users.FindAsync(req.UserId);
        if (user is null)
            return BadRequest("User not found");

        // AI servisini çağır (hata durumunda fallback)
        string label = "neutral";
        double score = 0.0;

        try
        {
            var sa = await _ai.AnalyzeAsync(req.Text, ct);
            label = sa.label;
            score = sa.score;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Sentiment Error] {ex.Message}");
        }

        var msg = new Message
        {
            UserId = req.UserId,
            Text = req.Text,
            SentimentLabel = label,
            SentimentScore = score,
            CreatedAt = DateTime.UtcNow
        };

        _db.Messages.Add(msg);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(Get), new { id = msg.Id }, msg);
    }

    // ID'ye göre tek mesajı getirir
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Message>> Get(int id)
    {
        var message = await _db.Messages.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
        return message is null ? NotFound() : Ok(message);
    }

    // Belirli bir kullanıcıya ait mesajları getirir
    [HttpGet("by-user/{userId:int}")]
    public async Task<ActionResult<IEnumerable<Message>>> ByUser(int userId)
    {
        var messages = await _db.Messages
            .Where(m => m.UserId == userId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();

        return Ok(messages);
    }
}
