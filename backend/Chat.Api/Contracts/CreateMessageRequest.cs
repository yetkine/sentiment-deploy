namespace Chat.Api.Contracts;

public record CreateMessageRequest(int UserId, string Text);
