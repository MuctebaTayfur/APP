using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace APP.Infra.Base.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static readonly string secretKey = "e5aa290cc4c264d4ba8aa4118321a36d8353150e0ca4b3eff39c2a6e8e9cda97";

        public static IServiceCollection AddCustomJwtToken(IServiceCollection services)
        {
            var environmentName = "Development";

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var tokenValidationParameters = new TokenValidationParameters();

            if (environmentName == "Production")
            {
                tokenValidationParameters.ValidateIssuerSigningKey = true;
                tokenValidationParameters.IssuerSigningKey = signingKey;
                tokenValidationParameters.ValidateIssuer = true;
                tokenValidationParameters.ValidIssuer = "https://authapi";
                tokenValidationParameters.ValidateAudience = true;
                tokenValidationParameters.ValidAudience = "Audience";
                tokenValidationParameters.ValidateLifetime = false;
                tokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(30);
            }
            else
            {
                tokenValidationParameters.ValidateIssuerSigningKey = false;
                tokenValidationParameters.IssuerSigningKey = signingKey;
                tokenValidationParameters.ValidateIssuer = false;
                tokenValidationParameters.ValidIssuer = "https://authapi";
                tokenValidationParameters.ValidateAudience = false;
                tokenValidationParameters.ValidAudience = "Audience";
                tokenValidationParameters.ValidateLifetime = false;
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(options =>
             {
                 options.SaveToken = true;
                 options.RequireHttpsMetadata = false;
                 options.TokenValidationParameters = tokenValidationParameters;
                 options.IncludeErrorDetails = true;
                 options.Events = new JwtBearerEvents
                 {
                     OnMessageReceived = JwtBearerOnMessageReceived
                 };
             });
            return services;
        }

        private static Task JwtBearerOnMessageReceived(MessageReceivedContext context)
        {
            if (!context.Request.Query.TryGetValue("Authorization", out StringValues values))
            {
                return Task.CompletedTask;
            }

            if (values.Count > 1)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Fail($"Only one 'Authorization' query string parameter can be defined. However, {values.Count:N0} were included in the request.");
                return Task.CompletedTask;
            }

            var token = values.Single();
            if (string.IsNullOrWhiteSpace(token))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Fail("The 'Authorization' query string parameter was defined, but a value to represent the token was not included.");
                return Task.CompletedTask;
            }

            context.Token = token.Trim();
            return Task.CompletedTask;
        }
    }
}
