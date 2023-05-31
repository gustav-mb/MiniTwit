using MiniTwit.Core.Data;
using MiniTwit.Core.IRepositories;
using MiniTwit.Infrastructure;
using MiniTwit.Infrastructure.Repositories;
using MiniTwit.Security.Hashing;
using MiniTwit.Service;

var builder = WebApplication.CreateBuilder(args);

// Add user-secrets if running in container
builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);

// Configure MongoDB
builder.Services.Configure<MiniTwitDatabaseSettings>(builder.Configuration.GetSection(nameof(MiniTwitDatabaseSettings)));
builder.Services.Configure<MiniTwitDatabaseSettings>(options => options.ConnectionString = builder.Configuration.GetConnectionString("MiniTwit")!);

// Configure Hashing
builder.Services.Configure<HashSettings>(builder.Configuration.GetSection(nameof(HashSettings)));
builder.Services.AddScoped<IHasher, Argon2Hasher>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Services
builder.Services.AddSingleton<IMiniTwitContext, MiniTwitContext>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFollowerRepository, FollowerRepository>();
builder.Services.AddScoped<ITweetRepository, TweetRepository>();
builder.Services.AddScoped<IServiceManager, ServiceManager>();

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
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
