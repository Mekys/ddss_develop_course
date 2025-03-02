using Application.Command;
using Application.Dto.Profile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProfile(CreateProfileRequestDto requestDto)
    {
        if (requestDto is null)
        {
            return BadRequest();
        }
        
        var result = await _mediator.Send(CreateProfileCommand.GetFrom(requestDto));

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return BadRequest(result.Errors);
    }
}