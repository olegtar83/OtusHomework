namespace LegendarySocialNetwork.DataClasses.Responses;

public class RegisterRes
{
    public RegisterRes(string userId, string token)
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Token = token;
    }

    public string UserId { get; set; }

    public string Token { get; set; }
}
