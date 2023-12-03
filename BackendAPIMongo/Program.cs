using BackendAPIMongo;
using BackendAPIMongo.Model;
using BackendAPIMongo.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = "http://localhost:5173";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string AuthScheme = "token";

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
builder.Services.AddSingleton<IBabyNameRepository, BabyNameRepository>();
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


#region Authentication endpoints
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
            new Claim(ClaimTypes.Email, user.Email),
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
#endregion

// Get User object of logged in user (for profile page)
app.MapGet("/user", async (IUserRepository iUserRepository, HttpContext context) =>
{
    if (context.User.Identity.IsAuthenticated)
    {
        var email = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var user = await iUserRepository.GetUser(new User { Email = email });
        return Results.Ok(
            new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.IsAdmin,
                user.Partner,
                user.LikedBabyNames
            });
    }
    else
    {
        return Results.Unauthorized();
    }

}).RequireAuthorization("user");


app.MapPost("/add-partner", async (IUserRepository iUserRepository, HttpContext context) =>
{
    if (context.User.Identity.IsAuthenticated)
    {
        var userEmail = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        // Read the request body and deserialize it into UserRequest
        using (var reader = new StreamReader(context.Request.Body))
        {
            var requestBody = await reader.ReadToEndAsync();
            var userRequest = JsonConvert.DeserializeObject<UserRequest>(requestBody);

            if (userRequest == null || string.IsNullOrEmpty(userRequest.Email))
            {
                return Results.BadRequest("Invalid or missing email in the request body.");
            }

            if (userRequest.Email == userEmail)
            {
                return Results.BadRequest("You cannot add yourself as a partner.");
            }

            var user = await iUserRepository.GetUser(new User { Email = userEmail });
            await iUserRepository.AddPartner(user, userRequest.Email);

            return Results.Ok("Partner added successfully");
        }
    }

    return Results.Unauthorized();
}).RequireAuthorization("user");


#region BabyName endpoints

app.MapGet("/babynames", async ([FromQuery] int page, IBabyNameRepository iBabyNameRepository) =>
{
    var babyNamesList = await iBabyNameRepository.GetBabyNames(page);

    return Results.Ok(babyNamesList);
}).AllowAnonymous();


app.MapGet("/babynames/filter", async ([FromQuery] int page, [FromQuery] bool isMale, [FromQuery] bool isFemale, [FromQuery] bool isInternational, IBabyNameRepository iBabyNameRepository) =>
{
    var babyNamesList = await iBabyNameRepository.GetBabyNames(page, isMale, isFemale, isInternational);

    return Results.Ok(babyNamesList);
}).AllowAnonymous();

#endregion

app.MapDelete("/remove-partner", async (IUserRepository iUserRepository, HttpContext context) =>
{
    if (context.User.Identity.IsAuthenticated)
    {
        var userEmail = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        // Read the request body and deserialize it into UserRequest
        using (var reader = new StreamReader(context.Request.Body))
        {
            var requestBody = await reader.ReadToEndAsync();
            var userRequest = JsonConvert.DeserializeObject<UserRequest>(requestBody);

            if (userRequest == null || string.IsNullOrEmpty(userRequest.Email))
            {
                return Results.BadRequest("Invalid or missing email in the request body.");
            }

            var user = await iUserRepository.GetUser(new User { Email = userEmail });
            await iUserRepository.RemovePartner(user, userRequest.Email);

            return Results.Ok("Partner removed successfully");
        }
    }

    return Results.Unauthorized();
}).RequireAuthorization("user");


// Likes or dislikes a babyname.
// If the babyname is already in the users likedBabyNames list, it will be removed and the count will be decreased.
// If not , it will be added and the count will be increased.
app.MapPut("/babynames/like", async (IUserRepository iUserRepository, IBabyNameRepository iBabyNameRepository, BabyName babyName, HttpContext context) =>
{
    if (context.User.Identity.IsAuthenticated)
    {
        var userEmail = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        var user = await iUserRepository.GetUser(new User { Email = userEmail });

        if (user == null)
        {
            return Results.BadRequest("User does not exist");
        }

        // If the babyname is already in the users likedBabyNames list, it will be removed and the count will be decreased.
        if (user.LikedBabyNames.Any(bn => bn.Id == babyName.Id))
        {
            var unlikeBabyName = await iBabyNameRepository.RemoveLike(babyName);

            await iUserRepository.UnlikeBabyname(user, babyName);

            return Results.Ok();
        }
        else // If not , it will be added and the count will be increased.
        {
            var likeBabyName = await iBabyNameRepository.AddLike(babyName);

            if (likeBabyName == 0)
            {
                return Results.BadRequest("Babyname does not exist");
            }

            await iUserRepository.LikeBabyname(user, babyName);

            return Results.Ok();
        }
    }

    return Results.Unauthorized();
}).RequireAuthorization("user");

app.MapPut("/update-user-email", async (IUserRepository iUserRepository, HttpContext context) =>
{
    if (context.User.Identity.IsAuthenticated)
    {
        string? userEmail = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        // Read the request body and deserialize it into UserRequest
        using (StreamReader? reader = new StreamReader(context.Request.Body))
        {
            string? requestBody = await reader.ReadToEndAsync();
            UserRequest? userRequest = JsonConvert.DeserializeObject<UserRequest>(requestBody);

            if (userRequest == null || string.IsNullOrEmpty(userRequest.Email))
            {
                return Results.BadRequest("Invalid or missing email in the request body.");
            }

            User? user = await iUserRepository.GetUser(new User { Email = userEmail });
            await iUserRepository.ChangeEmailAddressAsync(user, userRequest.Email);

            return Results.Ok("Email updated successfully");
        }
    }

    return Results.Unauthorized();
}).RequireAuthorization("user");

#region Admin endpoints

app.MapGet("/statistics/user-count", async (IUserRepository iUserRepository, HttpContext context) =>
{
    if (context.User.Identity.IsAuthenticated)
    {
        long userCount = await iUserRepository.GetUserCountAsync();
        return Results.Ok(userCount);
    }
    else
    {
        return Results.Unauthorized();
    }

}).RequireAuthorization("user"); // TODO: Add admin authorization

#endregion

app.Run();