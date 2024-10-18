using System.ComponentModel.DataAnnotations;

namespace LegendarySocialNetwork.DataClasses.Requests;

public class LoginReq
{
    [Required]
    public string Id { get; set; }
    [Required]
    public string Password { get; set; }
}
