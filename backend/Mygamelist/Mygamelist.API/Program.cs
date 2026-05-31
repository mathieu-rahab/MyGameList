using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Mygamelist.API.Middlewares;
using Mygamelist.Business;
using Mygamelist.Core.Business;
using Mygamelist.Core.Repository;
using Mygamelist.DatabaseRepository;
using Mygamelist.DatabaseRepository.Context;
using Mygamelist.Utiles;
using Mygamelist.Hateos;

var builder = WebApplication.CreateBuilder(args);
EnvReader.Load(".env");


builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:3173",
                "http://localhost:4173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// DI
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICollectionService, CollectionService>();
builder.Services.AddScoped<IFriendshipService, FriendshipService>();
builder.Services.AddScoped<IFriendshipRepository, FriendshipRepository>();
builder.Services.AddScoped<ICollectionRepository, CollectionRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient<ISteamService, SteamService>();
builder.Services.AddScoped<ISteamService>(provider =>
{
    var steamKey = Environment.GetEnvironmentVariable("STEAM_KEY");
    return (steamKey == null) 
        ? throw new Exception("STEAM_KEY_NOT_FOUND")
        : new SteamService(steamKey, provider.GetRequiredService<HttpClient>(), provider.GetRequiredService<IMemoryCache>());
});

builder.Services.AddScoped<IHateosLinkGenerator, HateosLinkGenerator>();
builder.Services.AddHttpContextAccessor();

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Auth
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = "http://localhost:5131/",
        ValidAudience = "http://localhost:5131/",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("AUTH_KEY") ?? throw new Exception("AUTH_KEY_NOT_FOUND"))),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("isAdmin", p =>
        p.RequireClaim("userRole", "admin"));
    options.AddPolicy("isUser", p =>
        p.RequireClaim("userRole", "user"));
});

builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}


// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("DevCors");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/", () => "hello"); 
app.MapControllers();

app.UseHttpsRedirection();


app.Run();


