namespace Chat.App.Request;

public record AddChatMessageRequest(Guid Id, string UserName, string Message, DateTime SentAt);
