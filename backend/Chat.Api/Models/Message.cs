namespace Chat.Api.Models;

public class Message
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    public string Text { get; set; } = default!;
    public string SentimentLabel { get; set; } = "neutral";
    public double SentimentScore { get; set; } = 0.0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
