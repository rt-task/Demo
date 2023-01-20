using FluentValidation;
using IdentityDemo.Dal;
using IdentityDemo.Identity.Context;
using IdentityDemo.Mapping;
using IdentityDemo.Services.Concrete;
using IdentityDemo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityDemo.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection ConfigureDIRules(this IServiceCollection services, IConfiguration configuration)
        {
            //TODO Decimo punto!
            services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("App")));

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IIdentityWorkerService, IdentityWorkerService>();

            return services;
        }
    }
}
