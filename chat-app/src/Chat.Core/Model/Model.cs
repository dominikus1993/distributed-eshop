namespace Chat.Core.Model;

public record ChatMessage(Guid Id, string UserName, string Message, DateTime SentAt);