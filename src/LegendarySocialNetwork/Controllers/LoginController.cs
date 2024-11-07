using LegendarySocialNetwork.Auxillary;
using LegendarySocialNetwork.Database;
using LegendarySocialNetwork.DataClasses.Internals;
using LegendarySocialNetwork.DataClasses.Requests;
using LegendarySocialNetwork.DataClasses.Responses;
using LegendarySocialNetwork.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace LegendarySocialNetwork.Controllers;

public class LoginController : ControllerBase
{
    private readonly IDatabaseContext _db;
    private readonly IPasswordService _pass;
    private readonly JWTSettings _jwtSettings;
    public LoginController(IDatabaseContext db, IPasswordService pass, IOptions<JWTSettings> jwtSettings)
    {
        _db = db;
        _pass = pass;
        _jwtSettings = jwtSettings.Value;
    }
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginReq data)
    {
        var result = await _db.GetLoginAsync(data.Id);
        if (result.Succeeded)
        {
            var isPasswordOk = _pass.VerifyHashedPassword(result.Value!.Password, data.Password);
            if (isPasswordOk)
            {
                var jwt = JwtHelper.GenerateJWToken();
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
