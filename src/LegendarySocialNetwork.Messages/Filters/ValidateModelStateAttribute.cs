﻿
using LegendarySocialNetwork.Messages.DataClasses.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace LegendarySocialNetwork.Messages.Filters;

public class ValidateModelStateAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Validates Model automaticaly 
    /// </summary>
    /// <param name="context"></param>
    /// <inheritdoc />
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = new List<string>();
            foreach (var item in context.ModelState)
            {
                if (item.Value.Errors.Count > 0)
                {
                    var err = item.Value.Errors.Select(x => x.ErrorMessage).Aggregate((x, y) => x + "; " + y);
                    errors.Add($"[{item.Key}]:{err}");
                }
            }
            var result = JsonSerializer.Serialize(new ErrorRes(errors.Aggregate((x, y) => x + "; " + y)), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            context.Result = new BadRequestObjectResult(new ErrorRes(errors.Aggregate((x, y) => x + "; " + y)));
        }
    }
}