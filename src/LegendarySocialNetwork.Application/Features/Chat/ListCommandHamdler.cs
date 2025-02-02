

using LegendarySocialNetwork.Application.Common.Models;
using MediatR;
using Newtonsoft.Json;

namespace LegendarySocialNetwork.Application.Features.Chat
{
    public record ListCommandRequest(string СompanionId) : IRequest<Result<List<ChatResp>>>;

    public class ListCommandHandler : IRequestHandler<ListCommandRequest, Result<List<ChatResp>>>
    {
        private readonly IHttpClientFactory _clientFactory;

        public ListCommandHandler(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<Result<List<ChatResp>>> Handle(ListCommandRequest request, CancellationToken cancellationToken)
        {
            var client = _clientFactory.CreateClient("messages_client");

            var response = await client.GetAsync($"/api/dialog/{request.СompanionId}/list", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var bodyContent = await response.Content.ReadAsStringAsync();
                return Result<List<ChatResp>>.Failure(bodyContent ?? response.ReasonPhrase!);
            }

            var content = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<List<ChatResp>>(content);

            return Result<List<ChatResp>>.Success(result!);
        }
    }
        public class ChatResp
        {
            public required string From { get; set; }
            public required string To { get; set; }
            public required string Text { get; set; }
        }
}
