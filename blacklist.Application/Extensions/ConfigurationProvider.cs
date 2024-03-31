
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
//using Swashbuckle.AspNetCore.Filters;
using System.Text;

namespace blacklist.Application.Extensions
{
    public static class ConfigurationProvider
    {
        public static void AddConfigurations(this IServiceCollection services, IConfiguration config)
        {
            services.AddEndpointsApiExplorer();
        }

        /// <summary>
        /// adds jwt beare token to the authenticaiton pipleline
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        public static void AddJwtSecurity(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateAudience = true,
                       ValidateIssuer = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = config["JwtSettings:Issuer"],
                       ValidAudience = config["JwtSettings:Audience"],
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:SecretKey"])),
                       ClockSkew = TimeSpan.Zero
                   };
               });
        }

        /// <summary>
        /// adds swagger configurations
        /// </summary>
        /// <param name="services"></param>
        public static void AddSwaggerConfig(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "VATSOP.PRESENTATION API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Jwt authorization header using the bearer scheme (\"bearer {token} \")",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                        },
                        new List<string>()
                    }
                });
            });
        }
    }
}
