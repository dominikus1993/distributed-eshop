using System.Collections.Generic;
using Chat.Core.Dto;
using Chat.Core.Repostories;

namespace Chat.Core.UseCases;

public class GetAllChatMessagesUseCase
{
    private IMessagesRepository _repo;

    public GetAllChatMessagesUseCase(IMessagesRepository repo)
    {
        _repo = repo;
    }

    public IAsyncEnumerable<ChatMessageDto> Execute()
    {
        return _repo.GetAllMessages().Select(msg => new ChatMessageDto(msg));
    }
}
