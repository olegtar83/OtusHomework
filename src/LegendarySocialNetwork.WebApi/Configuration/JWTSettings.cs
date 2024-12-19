namespace LegendarySocialNetwork.WebApi.Configuration;

public class JWTSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public double DurationInMinutes { get; set; }
}
