using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Rs256JwtAuth.Repositories;
using System.Security.Cryptography;

namespace Rs265JwtAuth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Load public key (for validation)
            var publicKey = File.ReadAllText(Path.Combine(builder.Environment.ContentRootPath, "Keys", "public.key"));
            var rsa = RSA.Create();
            rsa.ImportFromPem(publicKey);


            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new RsaSecurityKey(rsa),
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidateLifetime = true
                };
            });


            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
                options.AddPolicy("EmployeeOnly", p => p.RequireRole("Employee"));
            });


            builder.Services.AddControllers();
            builder.Services.AddSingleton<EmployeeRepository>();
            builder.Services.AddSingleton<InventoryRepository>();
            builder.Services.AddSingleton<RefreshTokenRepository>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new  OpenApiInfo { Title = "Rs256JwtAuth API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new  OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In =  ParameterLocation.Header,
                    Type =  SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });


            var app = builder.Build();


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllers();
            app.Run();

        }
    }
}
