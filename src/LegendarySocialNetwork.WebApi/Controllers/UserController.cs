using AutoMapper;
using LegendarySocialNetwork.Application.Features.Auth.Register;
using LegendarySocialNetwork.Application.Features.User;
using LegendarySocialNetwork.DataClasses.Dtos;
using LegendarySocialNetwork.DataClasses.Responses;
using LegendarySocialNetwork.Services;
using LegendarySocialNetwork.WebApi.Auxiliary;
using LegendarySocialNetwork.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LegendarySocialNetwork.WebApi.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserController : ApiControllerBase
{
    private readonly IMapper _mapper;
    private readonly IPasswordService _pass;
    public UserController(IMapper mapper, IPasswordService pass)
    {
        _mapper = mapper;
        _pass = pass;
    }
    [AllowAnonymous]
    [HttpPost("[action]")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var password = _pass.HashPassword(request.Password);
        request.Password = password;

        var result = await Mediator.Send(request);
        if (result.Succeeded)
        {
            var claims = new List<Claim> {
                   new Claim(ClaimTypes.NameIdentifier, result.Value)
                };
            var jwt = JwtHelper.GenerateJWToken(claims);
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return Ok(new RegisterRes(result.Value, token));
        }
        return BadRequest();
    }

    [Authorize]
    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var result = await Mediator.Send(new GetUserQueryRequest(id.ToString()));
        if (!result.Succeeded) return BadRequest(new ErrorRes(result.Error));

        var res = _mapper.Map<UserDto>(result.Value);
        return Ok(res);
    }

    [Authorize]
    [HttpGet("[action]")]
    public async Task<IActionResult> Search(string? firstName = null, string? lastName = null)
    {
        var dbRes = await Mediator.Send(new SearchUsersQueryRequest(firstName, lastName));
        if (!dbRes.Succeeded) return BadRequest(new ErrorRes(dbRes.Error));

        var res = _mapper.Map<List<UserDto>>(dbRes.Value);
        return Ok(res);
    }
}
