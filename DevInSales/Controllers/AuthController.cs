using Microsoft.AspNetCore.Mvc;

namespace DevInSales.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("/authenticate")]
        public IActionResult Authenticate()
        {
            return Ok();
        }
    }
}
