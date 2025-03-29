using Application.Services;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilesController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfilesController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _profileService.Register(registerDto);
            
            if (result.IsFailed)
            {
                if (result.HasError(error => error.Message == "User with login exists"))
                {
                    return Conflict(result.Errors);
                }
                return BadRequest(result.Errors);
            }

            return CreatedAtAction(
                nameof(GetProfile), 
                new { userId = result.Value.Id }, 
                result.Value);
        }

        [HttpPost("{subscriberId}/subscribe/{subscribeToId}")]
        public async Task<IActionResult> Subscribe(Guid subscriberId, Guid subscribeToId)
        {
            var result = await _profileService.Subscribe(subscriberId, subscribeToId);
            
            if (result.IsFailed)
            {
                if (result.HasError(error => error.Message.Contains("User with id not exists")))
                {
                    return NotFound(result.Errors);
                }
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProfile(Guid userId)
        {
            var result = await _profileService.GetProfile(userId);
            
            if (result.IsFailed)
            {
                if (result.HasError(error => error.Message == "User does not have access to this profile"))
                {
                    return Forbid();
                }
                return NotFound();
            }

            return Ok(result.Value);
        }
    }
}