using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Chat.Api.Models;

public class User
{
    public int Id { get; set; }

    public string Name { get; set; } = ""; // ✔ Build hatasını çözer

    [Required, MaxLength(50)]
    public string Nickname { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public List<Message>? Messages { get; set; }
}
