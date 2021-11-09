using Chat.Core.Dto;
using Chat.Core.Model;
using Chat.Core.Repostories;
using LanguageExt;

namespace Chat.Core.UseCases
{
    public class AddMessageUseCase
    {
        private IMessagesRepository _repo;

        public AddMessageUseCase(IMessagesRepository repo)
        {
            _repo = repo;
        }
        public async Task<Either<Exception, Unit>> Execute(ChatMessageDto messgae, CancellationToken cancellationToken = default)
        {
            var msg = new ChatMessage(messgae.Id, messgae.UserName, messgae.Message, messgae.SentAt);
            return await _repo.AddMessage(msg, cancellationToken);
        }
    }
}