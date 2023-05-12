using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using Backend.Data;
using Backend.Data.Entities.Auth;
using Backend.Data.Repositories;
using Backend.Interfaces.Repositories;
using Backend.Interfaces.Services;
using Backend.Services;
using Backend.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var frontEndOrigins = "_frontEndOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(frontEndOrigins, policy =>
    {
        policy.WithOrigins(builder.Configuration["FrontendAddress"]).AllowCredentials().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("Pagination");
    });
});

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 5;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters.ValidAudience = builder.Configuration["JWT:ValidAudience"];
        options.TokenValidationParameters.ValidIssuer = builder.Configuration["JWT:ValidIssuer"];
        options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]));
    });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    options.LogTo(Console.WriteLine, LogLevel.Information);
});

builder.Services.AddTransient<IJwtTokenService, JwtTokenService>();

builder.Services.AddTransient<ITeamService, TeamService>();
builder.Services.AddTransient<ITeamRepository, TeamRepository>();

builder.Services.AddTransient<ITournamentService, TournamentService>();
builder.Services.AddTransient<ITournamentRepository, TournamentRepository>();
builder.Services.AddTransient<ITournamentMatchRepository, TournamentMatchRepository>();

builder.Services.AddTransient<IGameService, GameService>();
builder.Services.AddTransient<IGameRepository, GameRepository>();
builder.Services.AddTransient<IGameTeamRepository, GameTeamRepository>();
builder.Services.AddTransient<ISetRepository, SetRepository>();

builder.Services.AddTransient<ILogRepository, LogRepository>();
builder.Services.AddTransient<ILogService, LogService>();

builder.Services.AddScoped<AuthDbSeeder>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyNames.ResourceOwner, policy => policy.Requirements.Add(new ResourceOwnerRequirement()));
    options.AddPolicy(PolicyNames.ResourceManager, policy => policy.Requirements.Add(new ResourceManagerRequirement()));
});

builder.Services.AddSingleton<IAuthorizationHandler, ResourceOwnerAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, ResourceManagerAuthorizationHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(frontEndOrigins);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

var dbSeeder = app.Services.CreateScope().ServiceProvider.GetRequiredService<AuthDbSeeder>();
await dbSeeder.SeedAsync();

app.Run();
