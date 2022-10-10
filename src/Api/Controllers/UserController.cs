using ApplicationService.Dto.Input;
using ApplicationService.Interface;
using ApplicationService.Interface.Validation;
using Domain.Model;
using Domain.ValueObject;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserAppService _userAppService;
        private readonly IPostAppService _postAppService;
        private readonly IFollowerAppService _followerAppService;

        private readonly IPostValidator _postValidator;
        private readonly IUserValidator _userValidator;

        public UserController(
            IUserAppService userAppService,
            IPostAppService postAppService,
            IFollowerAppService followerAppService,
            IPostValidator postValidator,
            IUserValidator userValidator)
        {
            _userAppService = userAppService;
            _postAppService = postAppService;
            _followerAppService = followerAppService;
            _postValidator = postValidator;
            _userValidator = userValidator;
        }

        [HttpPost]
        public async Task<IActionResult> AddUserAsync([FromBody] UserInputDto input)
        {
            var errors = _userValidator.IsValid(input);
            if (errors.Any())
            {
                var result = new Result<User>
                {
                    Errors = errors
                };
                return BadRequest(result);
            }

            var response = await _userAppService.AddAsync(input).ConfigureAwait(false);
            if (response.HasError)
            {
                return BadRequest(response);
            }

            return Created(nameof(AddFollowerAsync), response.Content);
        }


        [HttpGet]
        [Route("{username}/profile")]
        public async Task<IActionResult> GetUserProfileAsync(string username)
        {
            var response = await _userAppService.GetUserProfileAsync(username).ConfigureAwait(false);
            return Ok(response.Content);
        }

        [HttpPost]
        [Route("{username}/follow/{followusername}")]
        public async Task<IActionResult> AddFollowerAsync(string username, string followusername)
        {
            var response = await _followerAppService.AddAsync(username, followusername).ConfigureAwait(false);
            if (response.HasError)
            {
                return BadRequest(response);
            }

            return Accepted();
        }

        [HttpDelete]
        [Route("{username}/unfollow/{followusername}")]
        public async Task<IActionResult> DeleteFollowerAsync(string username, string followusername)
        {
            var response = await _followerAppService.DeleteAsync(username, followusername).ConfigureAwait(false);
            if (response.HasError)
            {
                return BadRequest(response);
            }

            return NoContent();
        }

        [HttpGet]
        [Route("{username}/posts")]
        public async Task<IActionResult> GetPostsByFilterAsync(string username = null, int fetch_next = 10, int offset = 0, bool all_posts = false)
        {
            var response = await _postAppService.GetPostsByFilterAsync(username, fetch_next, offset, all_posts).ConfigureAwait(false);
            return Ok(response.Content);
        }

        [HttpPost]
        [Route("{username}/posts")]
        public async Task<IActionResult> AddPostAsync(string username, [FromBody] PostInputDto input)
        {
            var errors = _postValidator.IsValid(input);
            if (errors.Any())
            {
                var result = new Result<Post>
                {
                    Errors = errors
                };
                return BadRequest(result);
            }

            var response = await _postAppService.AddAsync(username, input).ConfigureAwait(false);
            if (response.HasError)
            {
                return BadRequest(response);
            }

            return Created(nameof(AddPostAsync), response.Content);
        }
    }
}
