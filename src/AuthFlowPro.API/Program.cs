using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AuthFlowPro.Infrastructure;
using AuthFlowPro.Application;
using AuthFlowPro.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using AuthFlowPro.Infrastructure.Data;
using AuthFlowPro.Application.Permission;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var jwtSettings = builder.Configuration.GetSection("Jwt");


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!)),
            ClockSkew = TimeSpan.Zero // optional: prevents time drift allowing expired tokens briefly
        };
    });


builder.Services.AddAuthorization(options =>
{
    foreach (var role in RolePermissions.PermissionsByRole)
    {
        foreach (var permission in role.Value.Distinct())
        {
            options.AddPolicy(permission, policy =>
                policy.RequireClaim("permission", permission));
        }
    }
});

builder.Services.AddEndpointsApiExplorer(); // Required
builder.Services.AddSwaggerGen();

var app = builder.Build();

await DbSeeder.SeedAdminAsync(app.Services, builder.Configuration);

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    await DbSeeder.SeedDefaultRolesAndPermissionsAsync(roleManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthFlowPro API v1");
    });

}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// app.MapGet("/api/Test", () => "Hello from Program.cs");



app.Run();


