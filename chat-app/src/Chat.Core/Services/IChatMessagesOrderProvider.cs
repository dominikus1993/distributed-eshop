using Chat.Core.Model;

namespace Chat.Core.Services;

// xDDDD
public interface IMessageOrderProvider
{
    IAsyncEnumerable<ChatMessage> Sort(IEnumerable<ChatMessage> messages);
}

public class DefaultMessageOrderProvider : IMessageOrderProvider
{
    public async IAsyncEnumerable<ChatMessage> Sort(IEnumerable<ChatMessage> messages)
    {
        foreach (var item in messages.OrderBy(m => m.SentAt))
        {
            await Task.Delay(100);
            yield return item;
        }
    }
}
