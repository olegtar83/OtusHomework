using LegendarySocialNetwork.Auxillary;
using LegendarySocialNetwork.Database;
using LegendarySocialNetwork.DataClasses.Requests;
using LegendarySocialNetwork.DataClasses.Responses;
using LegendarySocialNetwork.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace LegendarySocialNetwork.Controllers;

public class LoginController : ControllerBase
{
    private readonly IDatabaseContext _db;
    private readonly IPasswordService _pass;
    public LoginController(IDatabaseContext db, IPasswordService pass)
    {
        _db = db;
        _pass = pass;
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
