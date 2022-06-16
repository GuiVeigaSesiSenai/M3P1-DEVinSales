using DevInSales.Context;
using DevInSales.DTOs;
using DevInSales.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevInSales.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly SqlContext _context;

        public AuthenticationController(SqlContext context)
        {
            _context = context;
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult> AuthenticateAsync([FromBody] UserAuthDTO dto)

        {
            var user = await _context.User.Include(x => x.Profile).FirstOrDefaultAsync(x => x.Email == dto.Email && x.Password == dto.Password);

            if (user == null) return BadRequest(new { Message = "Usuário ou senha inválidos." });

            var token = TokenService.GenerateToken(user);

            var login = new
            {
                token,
                User = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.ProfileId
                }
            };
            
            return Ok(new{ login });
        }
    }
}