using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using LegendarySocialNetwork.Assistant.Service;
using Microsoft.AspNetCore.Mvc;

namespace LegendarySocialNetwork.Assistant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssistantController : ControllerBase
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IPgVectorService _pgVector;
        private readonly ILogger<AssistantController> _logger;

        public AssistantController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IPgVectorService pgVector,
            ILogger<AssistantController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _pgVector = pgVector;
            _logger = logger;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] AskRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var matches = await _pgVector.SearchAsync(request.Text);
                _logger.LogInformation(
                    "Ask {Query} found {Count} messages:\n{Matches}",
                    request.Text,
                    matches.Count,
                    string.Join("\n", matches.Select((match, i) =>
                        $"{i + 1}. d={match.Distance:F4} {match.UserId}: {match.Text}")));

                using var response = await _httpClient.PostAsJsonAsync(
                    $"{_configuration["ModelUrl"]}/chat/completions",
                    new ChatRequest(
                        _configuration["ModelName"]!,
                        [
                            new ChatMessage(
                                "user",
                                $"""
                                These chat messages were found for: {request.Text}
                                Comment on them in Russian, in your own words, and mention what they say.
                                Do not say they do not exist. Do not say the phrase is missing.
                                Do not be moralistic, judgemental, or critical. Use humor.

                                {string.Join("\n", matches.Select((match, i) => $"{i + 1}. {match.Text}"))}
                                """)
                        ],
                        Temperature: 0.4,
                        MaxTokens: 120),
                    JsonOptions,
                    cancellationToken);

                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, body);

                var answer = JsonSerializer.Deserialize<ChatResponse>(body, JsonOptions)
                    ?.Choices.FirstOrDefault()?.Message.Content;
                return Ok(new AskResponse(answer, matches));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ask failed");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToString());
            }
        }
    }

    public record AskRequest(string Text);

    public record AskResponse(string? Answer, IReadOnlyList<DocumentMatch> Messages);

    public record ChatRequest(
        string Model,
        IReadOnlyList<ChatMessage> Messages,
        double Temperature,
        [property: JsonPropertyName("max_tokens")] int MaxTokens);

    public record ChatMessage(string Role, string Content);

    public record ChatResponse(List<ChatChoice> Choices);

    public record ChatChoice(ChatMessage Message);
}
