using Microsoft.AspNetCore.Mvc;

namespace HelpDeskApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "Сервер работает!", timestamp = DateTime.Now });
    }
}