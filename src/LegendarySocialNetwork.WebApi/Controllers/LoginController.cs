using LegendarySocialNetwork.Application.Features.Auth.Login;
using LegendarySocialNetwork.DataClasses.Responses;
using LegendarySocialNetwork.Services;
using LegendarySocialNetwork.WebApi.Auxiliary;
using LegendarySocialNetwork.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LegendarySocialNetwork.WebApi.Controllers;

public class LoginController : ApiControllerBase
{
    private readonly IPasswordService _pass;
    public LoginController(IPasswordService pass)
    {
        _pass = pass;
    }
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginUserQueryRequest request)
    {
        var result = await Mediator.Send(request);
        if (result.Succeeded)
        {
            var isPasswordOk = _pass.VerifyHashedPassword(result.Value!.Password, request.Password);
            if (isPasswordOk)
            {
                var claims = new List<Claim> {
                   new Claim(ClaimTypes.NameIdentifier, result.Value.Id),
                   new Claim(ClaimTypes.Name, result.Value.Name)
                };

                var jwt = JwtHelper.GenerateJWToken(claims);
                var token = new JwtSecurityTokenHandler().WriteToken(jwt);
                return Ok(new LoginRes(token));
            }
        }
        else
        {
            return BadRequest(result.Error);
        }
        return BadRequest("No login");
    }


}
