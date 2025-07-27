using System.Text;
using Obelix.Api.EventBus.RabbitMQ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Obelix.Api.EventBus.Abstraction.Extensions;
using Obelix.Api.Services.Requests.Data.Data;
using Obelix.Api.Services.Requests.Services;
using Obelix.Api.Services.Requests.WebHost.Handlers;
using Obelix.Api.Services.Requests.WebHost.Profiles;
using Obelix.Api.Services.Requests.WebHost.SwaggerConfiguration;
using Obelix.Api.Services.Shared.Data.Interceptors;
using Obelix.Api.Services.Shared.Data.Models.Identity;
using Obelix.Api.Services.Shared.IntegrationEvent;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<ApplicationDbContext>("requests-db", default, options =>
{
    options.AddInterceptors(new SoftDeleteInterceptor());
});


builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:AccessTokenSecret"] !)),
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(UserPolicies.UserPermissions, policy =>
        policy.RequireRole(UserRoles.User));
    options.AddPolicy(UserPolicies.AdminPermissions, policy =>
        policy.RequireRole(UserRoles.Admin));
    options.AddPolicy(UserPolicies.NormalPermissions, policy => 
        policy.RequireRole(UserRoles.User));
    options.AddPolicy(UserPolicies.ElevatedPermissions, policy => 
        policy.RequireRole(UserRoles.Admin));
});

builder.AddRabbitMqEventBus("Obelix-eventbus")
    .AddSubscription<UserCreatedIntegrationEvent, UserCreatedIntegrationEventHandler>()
    .AddSubscription<ItemCreatedIntegrationEvent, ItemCreatedIntegrationEventHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

app.UseForwardedHeaders();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

else
{
    app.UseHttpsRedirection();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Logger.LogInformation("Starting the app.");

app.Run();