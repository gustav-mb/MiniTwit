using Microsoft.AspNetCore.Mvc;
using MiniTwit.Core;
using MiniTwit.Core.IRepositories;
using MiniTwit.Core.MongoDB.DependencyInjection;
using MiniTwit.Infrastructure;
using MiniTwit.Infrastructure.Repositories;
using MiniTwit.Security.Hashing;
using MiniTwit.Security.Authentication;
using MiniTwit.Service;
using MiniTwit.Service.Data;
using MiniTwit.Server.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add user-secrets if running in container
builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);

// Suppress auto generation of BadRequest on model invalidation binding
builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

// Add Services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFollowerRepository, FollowerRepository>();
builder.Services.AddScoped<ITweetRepository, TweetRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IServiceManager, ServiceManager>();

// Configure MongoDB
var dbName = builder.Configuration.GetDatabaseName()!;
builder.Services.AddMongoContext<IMiniTwitContext, MiniTwitContext>(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString(dbName)!;
    options.DatabaseName = dbName;
});

// Configure Hashing
builder.Services.Configure<Argon2HashSettings>(builder.Configuration.GetSection(nameof(Argon2HashSettings)));
builder.Services.AddScoped<IHasher, Argon2Hasher>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwagger();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>()!;
builder.Services.AddJwtAuthentication(options => 
{
    options.Issuer = jwtSettings.Issuer;
    options.Audience = jwtSettings.Audience;
    options.Key = jwtSettings.Key;
    options.TokenExpiryMin = jwtSettings.TokenExpiryMin;
    options.RefreshTokenExpiryMin = jwtSettings.RefreshTokenExpiryMin;
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(config =>
    {
        config.SwaggerEndpoint("/swagger/v1/swagger.json", "MiniTwit API");
        config.DocumentTitle = "MiniTwit API - Swagger";
        config.RoutePrefix = string.Empty;
        config.DisplayRequestDuration();
    });

    // Seed database
    app.SeedDatabase();
}

// Cross-origin Request Blocked
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(_ => true) // allow any origin
    .AllowCredentials());

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }