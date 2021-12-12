using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyTalker.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(new { Messages = "empty" });
    }
}