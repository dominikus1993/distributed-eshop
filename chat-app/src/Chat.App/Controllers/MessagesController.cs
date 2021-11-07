using Chat.Core.Dto;
using Chat.Core.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Chat.App.Controllers;

[ApiController]
[Route("[controller]")]
public class MessagesController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;

    public MessagesController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetMessages")]
    public IAsyncEnumerable<ChatMessageDto> Get([FromServices]GetAllChatMessagesUseCase uc)
    {
        return uc.Execute();
    }
}
