using Chat.App.Request;
using Chat.Core.Dto;
using Chat.Core.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Chat.App.Controllers;

[ApiController]
[Route("messages")]
public class MessagesController : ControllerBase
{
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(ILogger<MessagesController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetMessages")]
    public IAsyncEnumerable<ChatMessageDto> Get([FromServices] GetAllChatMessagesUseCase uc)
    {
        return uc.Execute();
    }

    [HttpPost(Name = "AddMessage")]
    public async Task<IActionResult> Post([FromBody]AddChatMessageRequest request, [FromServices] AddMessageUseCase uc, CancellationToken cancellationToken)
    {
        var dto = new ChatMessageDto(request.Id, request.UserName, request.Message, request.SentAt);
        var res = await uc.Execute(dto, cancellationToken);
        return Accepted();
    }
}