using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using LegendarySocialNetwork.Database;
using LegendarySocialNetwork.DataClasses.Internals;
using LegendarySocialNetwork.DataClasses.Requests;
using LegendarySocialNetwork.DataClasses.Responses;
using LegendarySocialNetwork.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LegendarySocialNetwork.Controllers;

public class LoginController : ControllerBase
{
    private readonly IDatabaseContext _db;
    private readonly IPasswordService _pass;
    private readonly JWTSettings _jwtSettings;
    public LoginController(IDatabaseContext db,  IPasswordService pass, IOptions<JWTSettings> jwtSettings)
    {
        _db = db;
        _pass = pass;
        _jwtSettings = jwtSettings.Value;
    }
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginReq data)
    {
        var login = await _db.GetLoginAsync(data.Id);
        if (login.isSuccess)
        {
            var isPasswordOk = _pass.VerifyHashedPassword(login.account.Password, data.Password);
            if (isPasswordOk)
            {
                var jwt = GenerateJWToken(login.account.Id);
                var token = new JwtSecurityTokenHandler().WriteToken(jwt);
                return Ok(new LoginRes(token));
            }
        } else
        {
            return BadRequest(login.msg);
        }
        return BadRequest("No login");
    }

    private JwtSecurityToken GenerateJWToken(string userId)
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
            signingCredentials: signingCredentials);
        return jwtSecurityToken;
    }
}
