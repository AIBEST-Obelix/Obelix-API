using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Obelix.Api.EventBus.Abstraction.Extensions;
// using Obelix.Api.EventBus.RabbitMQ;
using Obelix.Api.Services.Identity.Data.Data;
using Obelix.Api.Services.Identity.Data.Models.Identity;
using Obelix.Api.Services.Identity.Services;
// using Obelix.Api.Services.Identity.WebHost.Handlers;
using Obelix.Api.Services.Identity.WebHost.Profiles;
using Obelix.Api.Services.Identity.WebHost.SwaggerConfiguration;
using Obelix.Api.Services.Shared.Data.Interceptors;
using Obelix.Api.Services.Shared.Data.Models.Identity;
// using Obelix.Api.Services.Shared.IntegrationEvent;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<ApplicationDbContext>("identity-db", default, options =>
{
    options.AddInterceptors(new SoftDeleteInterceptor());
});

builder.Services
    .AddIdentity<User, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
    
    
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:AccessTokenSecret"]!)),
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(UserPolicies.UserPermissions, policy =>
        policy.RequireRole(UserRoles.User)); 
    options.AddPolicy(UserPolicies.AccountantPermissions, policy =>
        policy.RequireRole(UserRoles.Accountant));
    options.AddPolicy(UserPolicies.AdminPermissions, policy =>
        policy.RequireRole(UserRoles.Admin));
    options.AddPolicy(UserPolicies.NormalPermissions, policy => 
        policy.RequireRole(UserRoles.User, UserRoles.Accountant));
    options.AddPolicy(UserPolicies.ElevatedPermissions, policy => 
        policy.RequireRole(UserRoles.Accountant, UserRoles.Admin));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddServices();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();


app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Logger.LogInformation("Starting the app.");

app.Run();