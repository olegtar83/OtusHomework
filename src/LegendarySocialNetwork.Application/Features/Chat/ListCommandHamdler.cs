

using LegendarySocialNetwork.Application.Common.Models;
using MediatR;

namespace LegendarySocialNetwork.Application.Features.Chat
{
    public record ListCommand(string СompanionId) : IRequest<Result<Unit>>;

    public class ListCommandHandler : IRequestHandler<ListCommand, Result<Unit>>
    {
        private readonly IHttpClientFactory _clientFactory;

        public ListCommandHandler(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        async Task<Result<Unit>> IRequestHandler<ListCommand, Result<Unit>>.Handle(ListCommand request, CancellationToken cancellationToken)
        {
            var client = _clientFactory.CreateClient("messages_client");

            var result = await client.GetAsync($"/dialog{request.СompanionId}/list", cancellationToken);

            if (!result.IsSuccessStatusCode)
            {
                var errorMessage = await result.Content.ReadAsStringAsync();
                return Result<Unit>.Failure(errorMessage);
            }

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
