using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SecureAPI.API.Properties
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
            [HttpGet] public IActionResult Get() => Ok(new { ok = true });

    }
}
