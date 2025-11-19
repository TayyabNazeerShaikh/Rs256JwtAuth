using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Rs256JwtAuth.Models;
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
        private readonly RefreshTokenRepository _refreshTokenRepo;
        private readonly IWebHostEnvironment _env;


        public AuthController(IConfiguration config, EmployeeRepository repo, RefreshTokenRepository refreshTokenRepo, IWebHostEnvironment env)
        {
            _config = config;
            _repo = repo;
            _refreshTokenRepo = refreshTokenRepo;
            _env = env;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] Models.LoginRequest request)
        {
            var user = _repo.GetAll().FirstOrDefault(x => x.Email == request.Email);
            if (user == null) return Unauthorized("Invalid email");

            var tokenString = GenerateJwtToken(user);

            var refreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                UserEmail = user.Email,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            _refreshTokenRepo.Add(refreshToken);

            return Ok(new { token = tokenString, refreshToken = refreshToken.Token });
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshTokenRequest request)
        {
            var existingToken = _refreshTokenRepo.GetByToken(request.RefreshToken);
            if (existingToken == null || !existingToken.IsActive)
            {
                return Unauthorized("Invalid refresh token");
            }

            var user = _repo.GetAll().FirstOrDefault(x => x.Email == existingToken.UserEmail);
            if (user == null) return Unauthorized("User not found");

            // Revoke the used refresh token (rotation)
            _refreshTokenRepo.Revoke(request.RefreshToken);

            var tokenString = GenerateJwtToken(user);

            var newRefreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                UserEmail = user.Email,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            _refreshTokenRepo.Add(newRefreshToken);

            return Ok(new { token = tokenString, refreshToken = newRefreshToken.Token });
        }

        private string GenerateJwtToken(Employee user)
        {
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


            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
