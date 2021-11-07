using Chat.Core.Model;

namespace Chat.Core.Dto;

public record ChatMessageDto(string UserName, string Message, DateTime SentAt)
{
    public ChatMessageDto(ChatMessage message) : this(message.UserName, message.Message, message.SentAt)
    {

    }
}