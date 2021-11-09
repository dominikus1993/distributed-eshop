using Chat.Core.Model;
using Chat.Core.Repostories;
using LanguageExt;
using static LanguageExt.Prelude;
namespace Chat.Infrastructure.Repositories;

internal class FakeMessageRepository : IMessagesRepository
{
    private readonly List<ChatMessage> _messages = new List<ChatMessage>
        {
            new ChatMessage (Guid.NewGuid(), "janpawel3", "xDD",  DateTime.Now.AddDays(-2) ),
            new ChatMessage (Guid.NewGuid(), "papaj", "xD3D",  DateTime.Now.AddDays(-3) ),
            new ChatMessage (Guid.NewGuid(), "karolwojtylak", "xDD2",  DateTime.Now.AddDays(-1) ),
        };

    public async Task<Either<Exception, Unit>> AddMessage(ChatMessage message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Task.Delay(100);
        _messages.Add(message);
        return Right(Unit.Default);
    }

    public async IAsyncEnumerable<ChatMessage> GetAllMessages(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Task.Yield();
        foreach (var message in _messages)
        {
            yield return message;
        }
    }
}