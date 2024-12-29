namespace LegendarySocialNetwork.Domain.Entities;

public class AccountEntity
{
    public required string Id { get; set; }
    public required string Password { get; set; }
    public required string Name { get; set; }
}
