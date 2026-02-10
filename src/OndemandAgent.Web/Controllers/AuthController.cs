using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OndemandAgent.Web.Data;
using OndemandAgent.Web.Dtos;
using OndemandAgent.Web.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OndemandAgent.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (await _context.Tenants.AnyAsync(t => t.Email == request.Email))
                return BadRequest("Bu e-posta adresi zaten kayıtlı.");

            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                CompanyName = request.CompanyName, 
                PasswordHash = HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kayıt başarılı. Giriş yapabilirsiniz." });
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Email == request.Email);
            
            if (tenant == null || tenant.PasswordHash != HashPassword(request.Password))
                return Unauthorized("E-posta veya şifre hatalı.");

            var tokenString = GenerateJwtToken(tenant);

            return Ok(new LoginResponse { Token = tokenString });
        }

        private string GenerateJwtToken(Tenant tenant)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, tenant.Id.ToString()), 
                    new Claim(ClaimTypes.Email, tenant.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        [HttpGet("me")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> GetMe()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

            var tenant = await _context.Tenants.FindAsync(Guid.Parse(userIdString));
            if (tenant == null) return NotFound();

            return Ok(new { 
                tenant.CompanyName, 
                tenant.Email, 
                MemberSince = tenant.CreatedAt 
            });
        }
    }
}