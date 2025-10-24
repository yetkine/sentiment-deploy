// chat-backend/Chat.Api/Models/User.cs
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Chat.Api.Models;

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Nickname { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Bu satır kritik: JSON dönerken döngü/şişkinlik yaratmasın diye gizliyoruz
    [JsonIgnore] // istersen: [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<Message>? Messages { get; set; }
}
