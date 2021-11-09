using System.Collections.Generic;
using Chat.Core.Dto;
using Chat.Core.Repostories;
using Chat.Core.Services;

namespace Chat.Core.UseCases;

public class GetAllChatMessagesUseCase
{
    private IMessagesRepository _repo;
    private IMessageOrderProvider _orderProvider;

    public GetAllChatMessagesUseCase(IMessagesRepository repo, IMessageOrderProvider orderProvider)
    {
        _repo = repo;
        _orderProvider = orderProvider;
    }

    public async IAsyncEnumerable<ChatMessageDto> Execute()
    {
        var messages = await _repo.GetAllMessages().ToListAsync();
        await foreach (var item in _orderProvider.Sort(messages))
        {
            yield return new ChatMessageDto(item);
        }
    }
}
