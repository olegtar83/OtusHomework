using LegendarySocialNetwork.Infrastructure.Common.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace LegendarySocialNetwork.Database;
internal sealed class PgOptionsSetup : IConfigureOptions<DatabaseOptions>
{
    private const string SectionName = "DatabaseSettings";

    private readonly IConfiguration _configuration;
    public PgOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public void Configure(DatabaseOptions options)
    {
        _configuration.GetSection(SectionName)
            .Bind(options);
    }
}