using AutoMapper;
using LegendarySocialNetwork.Application.Features.Post.Feed;
using LegendarySocialNetwork.Application.Features.Post.Posts;
using LegendarySocialNetwork.WebApi.Controllers.Base;
using LegendarySocialNetwork.WebApi.DataClasses.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegendarySocialNetwork.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostController : ApiControllerBase
    {
        private readonly IMapper _mapper;
        public PostController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create(CreatePostCommandRequest request)
        {
            await Mediator.Send(request);
            return Ok("Успешно создан пост");
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> Update(UpdatePostCommandRequest request)
        {
            await Mediator.Send(request);
            return Ok("Успешно изменен пост");
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await Mediator.Send(new DeletePostCommandRequest(id));
            return Ok("Успешно удален пост");
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var res = await Mediator.Send(new GetPostCommandRequest(id));
            if (!res.Succeeded)
            {
                return BadRequest(res.Error.ToString());
            }
            var post = _mapper.Map<PostDto>(res.Value);
            return Ok(post);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Feed(string? id)
        {
            var res = await Mediator.Send(new GetFeedCommandRequest(id));
            var posts = _mapper.Map<List<PostDto>>(res.Value);
            return Ok(posts);
        }
    }
}
