namespace Chat.Core.Model;

public record ChatMessage(string UserName, string Message, DateTime SentAt);