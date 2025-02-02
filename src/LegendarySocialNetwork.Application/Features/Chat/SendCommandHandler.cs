using LegendarySocialNetwork.Application.Common.Models;
using MediatR;
using Newtonsoft.Json;
using System.Text;

namespace LegendarySocialNetwork.Application.Features.Chat
{
    public record SendCommandRequest(string СompanionId, string Text) : IRequest<Result<Unit>>;
  
    public class SendCommandHandler : IRequestHandler<SendCommandRequest, Result<Unit>>
    {
        private readonly IHttpClientFactory _clientFactory;

        public SendCommandHandler(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;       
        }

        async Task<Result<Unit>> IRequestHandler<SendCommandRequest, Result<Unit>>.Handle(SendCommandRequest request, CancellationToken cancellationToken)
        {
            var client = _clientFactory.CreateClient("messages_client");

            var content = SerializeJson(new { request.Text });

            var result = await client.PostAsync($"/api/dialog/{request.СompanionId}/send", content, cancellationToken);

            if (!result.IsSuccessStatusCode)
            {
                var bodyContent = await result.Content.ReadAsStringAsync();
                return Result<Unit>.Failure(bodyContent ?? result.ReasonPhrase!);
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
