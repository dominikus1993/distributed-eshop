using Chat.Core.Model;
using LanguageExt;

namespace Chat.Core.Repostories;

public interface IMessagesRepository
{
    Task<Either<Exception, Unit>> AddMessage(ChatMessage message, CancellationToken cancellationToken = default);
    IAsyncEnumerable<ChatMessage> GetAllMessages(CancellationToken cancellationToken = default);
}