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

    // POST api/messages
    [HttpPost]
    public async Task<ActionResult<Message>> Create([FromBody] CreateMessageRequest req, CancellationToken ct)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var user = await _db.Users.FindAsync([req.UserId], ct);
        if (user is null) return BadRequest("User not found");

        string label = "neutral";
        double score = 0.0;

        try
        {
            var (lab, sc) = await _ai.AnalyzeAsync(req.Text, ct);
            label = lab;
            score = sc;
        }
        catch (Exception ex)
        {
            // Logla ve nötr skorla devam et
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

    // GET api/messages/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Message>> Get(int id, CancellationToken ct)
    {
        var message = await _db.Messages
            .AsNoTracking()
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return message is null ? NotFound() : Ok(message);
    }

    // GET api/messages/by-user/1?skip=0&take=50
    [HttpGet("by-user/{userId:int}")]
    public async Task<ActionResult<IEnumerable<Message>>> ByUser(
        int userId, [FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken ct = default)
    {
        take = Math.Clamp(take, 1, 200);

        var messages = await _db.Messages
            .AsNoTracking()
            .Where(m => m.UserId == userId)
            .OrderBy(m => m.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);

        return Ok(messages);
    }
}
