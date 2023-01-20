using IdentityDemo.Identity.Models;
using IdentityDemo.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using MimeKit;

namespace IdentityDemo.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options => options.AddDefaultPolicy(policy =>
                policy.WithOrigins(configuration.GetValue<string>("Configuration:Cors:Origins"))
                    .WithMethods(configuration.GetValue<string>("Configuration:Cors:Methods"))
                    .WithHeaders(configuration.GetValue<string>("Configuration:Cors:Headers"))));

            return services;
        }

        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            //TODO Sesto passo
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "IdentityDemo", Version = "v1" });

                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Insert the Bearer Token",
                    Name = HeaderNames.Authorization,
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference= new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
        {
            //TODO Quinto passo
            services.Configure<MailboxAddress>(configuration, "Configuration:SmtpFrom");
            services.Configure<SmtpConfiguration>(configuration, "Configuration:SmtpConfiguration");
            services.Configure<JwtSettings>(configuration, "Configuration:JwtSettings");
            services.Configure<IdentityUser<int>>(configuration, "Configuration:AdminInfo");
            services.Configure<List<string>>(configuration, "Configuration:Roles");

            return services;
        }

        private static void Configure<T>(this IServiceCollection services, IConfiguration configuration, string sectionName)
            where T : class
        {
            var section = configuration.GetSection(sectionName);
            services.Configure<T>(section);
        }
    }
}
