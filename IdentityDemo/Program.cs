using IdentityDemo.Identity.Configuration;
using IdentityDemo.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .ConfigureOptions(builder.Configuration)
    .ConfigureSwagger()
    .ConfigureDIRules(builder.Configuration)
    .RegisterValidators()
    .ConfigureCors(builder.Configuration);

builder.Services
    .AddAuthDbContext(builder.Configuration)
    .ConfigureIdentity()
    .ConfigureAuthentication(builder.Configuration)
    .ConfigureAuthorization();

builder.Services.AddHostedService<IdentityAndDbSetup>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
