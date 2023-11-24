using BackendAPIMongo;
using BackendAPIMongo.Model;
using BackendAPIMongo.Repository;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = "http://localhost:5173";

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const String AuthScheme = "token";

builder.Services.AddAuthentication(AuthScheme)
    .AddCookie(AuthScheme, Options =>
    {
        Options.ExpireTimeSpan = TimeSpan.FromHours(12);
        Options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(builder =>
{
    builder.AddPolicy("user", policy =>
    {
        policy.RequireAuthenticatedUser()
        .AddAuthenticationSchemes(AuthScheme)
        .AddRequirements()
        .RequireClaim("role", "user");
    });
});

builder.Services.AddCors(options =>
{
options.AddPolicy(name: allowedOrigins,
    policy =>
    {
        policy.WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});




builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.Configure<MongoDBRestSettings>(builder.Configuration.GetSection(nameof(MongoDBRestSettings)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(allowedOrigins);
app.UseAuthentication();
app.UseAuthorization();


#region GET and POST endpoints
app.MapPost("/register", async (User user, IUserRepository iUserRepository) =>
{
    // This is used to validate the user object (typesafety).
    // Example: if user.Email is not an email, it will return an error
    var validationContext = new ValidationContext(user, serviceProvider: null, items: null);
    var validationResults = new List<ValidationResult>();
    var isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

    if (!isValid)
    {
        return Results.BadRequest(validationResults);
    }

    // Register user after validation
    try
    {
        await iUserRepository.Register(user);
        return Results.Ok("User registered succesfully");
    }
    catch (Exception e)
    {
        return Results.BadRequest(e.Message);
    }

}).AllowAnonymous();


app.MapPost("/login", async (User user, IUserRepository iUserRepository, HttpContext ctx) =>
{
    // Authenticate user and then create a token
    if (await iUserRepository.Authenticate(user))
    {
        var claims = new List<Claim>
        {
            new Claim("role", "user")
        };
        var identity = new ClaimsIdentity(claims, AuthScheme);
        var userIdentity = new ClaimsPrincipal(identity);

        await ctx.SignInAsync(AuthScheme, userIdentity);
        return Results.Ok("Logged in succesfully");
    }
    return Results.BadRequest("Invalid username or password");

}).AllowAnonymous();


app.MapPost("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(AuthScheme);
    return Results.Ok("Logged out succesfully");
}).RequireAuthorization("user");


app.MapGet("/check-login", async (HttpContext context) =>
{
    if (context.User.Identity.IsAuthenticated)
    {
        return Results.Ok("User is logged in");
    }
    else
    {
        return Results.Unauthorized();
    }
}).RequireAuthorization("user");


app.Run();


#endregion
