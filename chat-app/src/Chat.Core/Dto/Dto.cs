using Chat.Core.Model;

namespace Chat.Core.Dto;

public record ChatMessageDto(Guid Id, string UserName, string Message, DateTime SentAt)
{
    public ChatMessageDto(ChatMessage message) : this(message.Id, message.UserName, message.Message, message.SentAt)
    {

    }
}