using LegendarySocialNetwork.Application.Common.Models;
using MediatR;
using Newtonsoft.Json;
using System.Text;

namespace LegendarySocialNetwork.Application.Features.Chat
{
    public record SendCommand(string СompanionId, string Text) : IRequest<Result<Unit>>;
  
    public class SendCommandHandler : IRequestHandler<SendCommand, Result<Unit>>
    {
        private readonly IHttpClientFactory _clientFactory;

        public SendCommandHandler(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;       
        }

        async Task<Result<Unit>> IRequestHandler<SendCommand, Result<Unit>>.Handle(SendCommand request, CancellationToken cancellationToken)
        {
            var client = _clientFactory.CreateClient("messages_client");


            var content = SerializeJson(request.Text);

            var result = await client.PostAsync($"/dialog{request.СompanionId}/send", content, cancellationToken);

            if (!result.IsSuccessStatusCode)
            {
                var errorMessage = await result.Content.ReadAsStringAsync();
                return Result<Unit>.Failure(errorMessage);
            }

            return Result<Unit>.Success(Unit.Value);
        }

        private StringContent SerializeJson(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
