using AutoMapper;
using LegendarySocialNetwork.Auxillary;
using LegendarySocialNetwork.Database;
using LegendarySocialNetwork.Database.Entities;
using LegendarySocialNetwork.DataClasses.Dtos;
using LegendarySocialNetwork.DataClasses.Requests;
using LegendarySocialNetwork.DataClasses.Responses;
using LegendarySocialNetwork.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace LegendarySocialNetwork.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserController : ControllerBase
{
    private readonly IDatabaseContext _db;
    private readonly IMapper _mapper;
    private readonly IPasswordService _pass;
    public UserController(IDatabaseContext db, IMapper mapper, IPasswordService pass)
    {
        _db = db;
        _mapper = mapper;
        _pass = pass;
    }
    [AllowAnonymous]
    [HttpPost("user/register")]
    public async Task<IActionResult> Register([FromBody] RegisterReq data)
    {
        var user = _mapper.Map<UserEntity>(data);

        var password = _pass.HashPassword(data.Password);

        var result = await _db.RegisterAsync(user, password);
        if (result.Succeeded)
        {
            var jwt = JwtHelper.GenerateJWToken();
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return Ok(new RegisterRes(result.Value, token));
        }
        return BadRequest();
    }

    [Authorize]
    [HttpGet("user/get/{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var result = await _db.GetUserAsync(id.ToString());
        if (!result.Succeeded) return BadRequest(new ErrorRes(result.Error));

        var res = _mapper.Map<UserDto>(result.Value);
        return Ok(res);
    }

    [Authorize]
    [HttpGet("user/search")]
    public async Task<IActionResult> Search(SearchReq query)
    {
        var dbRes = await _db.SearchUserAsync(query.FirstName, query.LastName);
        if (!dbRes.Succeeded) return BadRequest(new ErrorRes(dbRes.Error));

        var res = _mapper.Map<List<UserDto>>(dbRes.Value);
        return Ok(res);
    }
}
