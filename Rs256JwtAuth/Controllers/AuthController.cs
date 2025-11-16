using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Rs256JwtAuth.Repositories;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Rs256JwtAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly EmployeeRepository _repo;
        private readonly IWebHostEnvironment _env;


        public AuthController(IConfiguration config, EmployeeRepository repo, IWebHostEnvironment env)
        {
            _config = config;
            _repo = repo;
            _env = env;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _repo.GetAll().FirstOrDefault(x => x.Email == request.Email);
            if (user == null) return Unauthorized("Invalid email");


            var privateKeyPath = Path.Combine(_env.ContentRootPath, "Keys", "private.key");
            var privateKey = System.IO.File.ReadAllText(privateKeyPath);


            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKey);


            var creds = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);


            var claims = new[]
            {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
            };


            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:DurationInMinutes"])),
            signingCredentials: creds
            );


            var tokenString = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new { token = tokenString });
        }
    }
}
