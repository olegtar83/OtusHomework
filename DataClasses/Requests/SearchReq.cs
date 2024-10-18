using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LegendarySocialNetwork.DataClasses.Requests;

public class SearchReq
{
    [ValidateNever]
    public string FirstName { get; set; } = string.Empty;
    [ValidateNever]
    public string LastName { get; set; } = string.Empty;

}
