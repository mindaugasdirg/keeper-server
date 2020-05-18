using System.Threading.Tasks;
using Keeper.WebApi.Models;
using Keeper.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Keeper.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProfileController : ControllerBase
    {
        private IUsersService usersService;

        public ProfileController(IUsersService _usersService)
        {
            usersService = _usersService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] string secret)
        {
            if(string.IsNullOrWhiteSpace(secret))
                return BadRequest("Missing profile's secret");
            var key = await usersService.CreateAsync(secret);
            if(string.IsNullOrWhiteSpace(key))
                return StatusCode(500, "Unable to create new synchronization profile");
            return Ok(key);
        }

        [HttpGet("check/{key}")]
        public async Task<IActionResult> ExistsAsync(string key)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            if(await usersService.ExistsAsync(key))
                return Ok();
            return NotFound();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginForm login)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            if (login is null || string.IsNullOrWhiteSpace(login.Key) || string.IsNullOrWhiteSpace(login.Secret))
                return BadRequest("Login object must have key and secret properties");

            var result = await usersService.LoginAsync(login);
            if(string.IsNullOrWhiteSpace(result))
                return Unauthorized("Incorrect credentials");
            return Ok(result);
        }
    }
}