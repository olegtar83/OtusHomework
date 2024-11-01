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
using System.Security.Claims;
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
                var jwt = GenerateJWToken(result.Value!.Id);
                var token = new JwtSecurityTokenHandler().WriteToken(jwt);
                return Ok(new LoginRes(token));
            }
        }
        else
        {
            return BadRequest(result.Errors.First().ToString());
        }
        return BadRequest("No login");
    }

    private JwtSecurityToken GenerateJWToken(string userId)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        return new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: signinCredentials
        );
    }
}
