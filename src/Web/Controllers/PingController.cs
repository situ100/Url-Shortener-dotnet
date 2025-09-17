using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class PingController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { message = "pong" });
}
