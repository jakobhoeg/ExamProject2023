using BackendAPIMongo;
using BackendAPIMongo.Model;
using BackendAPIMongo.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = "http://localhost:5173";
var globalAllowedOrigins = "http://16.170.143.117:3000";

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

    builder.AddPolicy("admin", policy =>
    {
        policy.RequireAuthenticatedUser()
        .AddAuthenticationSchemes(AuthScheme)
        .AddRequirements()
        .RequireClaim("role", "admin");
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

    options.AddPolicy(name: globalAllowedOrigins,
        policy =>
        {
            policy.WithOrigins(globalAllowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});


builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IBabyNameRepository, BabyNameRepository>();
builder.Services.AddSingleton<IMatchedBabyNamesRepository, MatchedBabyNamesRepository>();
builder.Services.Configure<MongoDBRestSettings>(builder.Configuration.GetSection(nameof(MongoDBRestSettings)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(globalAllowedOrigins);

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
    // get the user from the database to determine if the user is an admin or not
    var dbUser = await iUserRepository.GetUser(user);

    if (dbUser.IsAdmin)
    {
        // Try to login with the user object given in the request body.
        // Authenticate user as admin and then create a token
        if (await iUserRepository.AuthenticateAdmin(user))
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("role", "admin")
            };
            var identity = new ClaimsIdentity(claims, AuthScheme);
            var userIdentity = new ClaimsPrincipal(identity);

            await ctx.SignInAsync(AuthScheme, userIdentity);
            return Results.Ok(dbUser);
        }
    }

    if (!dbUser.IsAdmin)
    {
        // Try to login with the user object given in the request body.
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
            return Results.Ok(dbUser);

        }
    }

    return Results.BadRequest("Invalid username or password");

}).AllowAnonymous();


app.MapPost("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(AuthScheme);
    return Results.Ok("Logged out succesfully");
}).RequireAuthorization(builder => builder.RequireAssertion(context =>
{
    return context.User.HasClaim("role", "user") || context.User.HasClaim("role", "admin");
}));


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
}).RequireAuthorization(builder => builder.RequireAssertion(context =>
{
    return context.User.HasClaim("role", "user") || context.User.HasClaim("role", "admin");
}));
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
                user.LikedBabyNames,
                user.CreatedDate
            });
    }
    else
    {
        return Results.Unauthorized();
    }

}).RequireAuthorization(builder => builder.RequireAssertion(context =>
{
    return context.User.HasClaim("role", "user") || context.User.HasClaim("role", "admin");
}));


app.MapPost("/add-partner", async (IUserRepository iUserRepository, HttpContext context, IMatchedBabyNamesRepository iMatchedBabyNamesRepository) =>
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
            var partner = await iUserRepository.GetUser(new User { Email = userRequest.Email });
            
            // Add partner to user and the other way around.
            await iUserRepository.AddPartner(user, partner.Email);

            // Create a MatchedBabyNames object for the users with an empty LikedBabyNames list
            await iMatchedBabyNamesRepository.CreateMatchedUsersList(user, partner);

            return Results.Ok("Partner added successfully");
        }
    }

    return Results.Unauthorized();
}).RequireAuthorization(builder => builder.RequireAssertion(context =>
{
    return context.User.HasClaim("role", "user") || context.User.HasClaim("role", "admin");
}));


#region BabyName endpoints

app.MapGet("/babynames", async ([FromQuery] int page, IBabyNameRepository iBabyNameRepository) =>
{
    var babyNamesList = await iBabyNameRepository.GetBabyNames(page);
    
    return Results.Ok(babyNamesList);
}).AllowAnonymous();

app.MapGet("/babynames/international", async ([FromQuery] int page, [FromQuery] bool isInternational, IBabyNameRepository iBabyNameRepository) =>
{
    var babyNamesList = await iBabyNameRepository.GetInternationalBabyNames(page, isInternational);

    return Results.Ok(babyNamesList);
}).AllowAnonymous();

app.MapGet("/babynames/sort/likes/asc", async ([FromQuery] int page, [FromQuery] bool isMale, [FromQuery] bool isFemale, [FromQuery] bool isInternational, IBabyNameRepository iBabyNameRepository) =>
{
    var babyNamesList = await iBabyNameRepository.GetBabyNamesSortedByLikesAsc(page, isMale, isFemale, isInternational);

    return Results.Ok(babyNamesList);
}).AllowAnonymous();

app.MapGet("/babynames/sort/likes/desc", async ([FromQuery] int page, [FromQuery] bool isMale, [FromQuery] bool isFemale, [FromQuery] bool isInternational, IBabyNameRepository iBabyNameRepository) =>
{
    var babyNamesList = await iBabyNameRepository.GetBabyNamesSortedByLikesDesc(page, isMale, isFemale, isInternational);

    return Results.Ok(babyNamesList);
}).AllowAnonymous();

app.MapGet("/babynames/sort/name/asc", async ([FromQuery] int page, [FromQuery] bool isMale, [FromQuery] bool isFemale, [FromQuery] bool isInternational, IBabyNameRepository iBabyNameRepository) =>
{
    var babyNamesList = await iBabyNameRepository.GetBabyNamesSortedByNameAsc(page, isMale, isFemale, isInternational);

    return Results.Ok(babyNamesList);
}).AllowAnonymous();

app.MapGet("/babynames/sort/name/desc", async ([FromQuery] int page, [FromQuery] bool isMale, [FromQuery] bool isFemale, [FromQuery] bool isInternational, IBabyNameRepository iBabyNameRepository) =>
{
    var babyNamesList = await iBabyNameRepository.GetBabyNamesSortedByNameDesc(page, isMale, isFemale, isInternational);

    return Results.Ok(babyNamesList);
}).AllowAnonymous();

app.MapGet("/babyname/random", async (IBabyNameRepository iBabyNameRepository, IUserRepository iUserRepository, HttpContext context) =>
{
    var babyName = await iBabyNameRepository.GetRandomBabyName();

    return Results.Ok(babyName);
}).AllowAnonymous();

app.MapGet("/babyname/random/sort", async([FromQuery] bool isMale, [FromQuery] bool isFemale, [FromQuery] bool isInternational, IBabyNameRepository iBabyNameRepository, IUserRepository iUserRepository, HttpContext context) =>
{
    var babyName = await iBabyNameRepository.GetRandomBabyNameSort(isMale, isFemale, isInternational);

    return Results.Ok(babyName);
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
}).RequireAuthorization(builder => builder.RequireAssertion(context =>
{
    return context.User.HasClaim("role", "user") || context.User.HasClaim("role", "admin");
}));


// Likes or dislikes a babyname.
// If the babyname is already in the users likedBabyNames list, it will be removed and the count will be decreased.
// If not , it will be added and the count will be increased.
// It also checks if the user has a partner and if the partner has liked the babyname too and then add or removes it from the matchedBabyNames list.
app.MapPut("/babynames/like", async (IUserRepository iUserRepository, IBabyNameRepository iBabyNameRepository, BabyName babyName, HttpContext context, IMatchedBabyNamesRepository iMatchedBabyRepository) =>
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

            // If the user has a partner, check if the partner has liked the babyname too and then remove it from the MatchedBabyNames list.
            if (user.Partner != null)
            {
                try
                {
                    // Gets the partner object from the database
                    var partner = await iUserRepository.GetUser(new User { Email = user.Partner.Email });

                    // Checks if the partner has liked the babyname too
                    if (partner.LikedBabyNames.Any(bn => bn.Id == babyName.Id))
                    {
                        // If the partner has liked the babyname too, retrieve the existing MatchedBabyNames object from the database
                        var matchedBabyNames = await iMatchedBabyRepository.GetMatchedBabyNames(user, partner);

                        // Remove the babyname from the MatchedBabyNames objects LikedBabyNames list
                        await iMatchedBabyRepository.RemoveMatchedBabyNames(user, partner, babyName, matchedBabyNames);
                    }

                }
                catch (Exception e)
                {
                    return Results.BadRequest(e.Message);
                }
            }

            return Results.Ok();
        }
        else // If not , it will be added and the count will be increased.
        {
            // Add like to the babyName
            var likeBabyName = await iBabyNameRepository.AddLike(babyName);

            if (likeBabyName == 0)
            {
                return Results.BadRequest("Babyname does not exist");
            }

            // Add the babyname to the user's likedBabyNames list
            await iUserRepository.LikeBabyname(user, babyName);

            // If the user has a partner, check if the partner has liked the babyname too and then add it to the MatchedBabyNames list.
            if (user.Partner != null)
            {
                try
                {
                    // Gets the partner object from the database
                    var partner = await iUserRepository.GetUser(new User { Email = user.Partner.Email });

                    // Checks if the partner has liked the babyname too
                    if (partner.LikedBabyNames.Any(bn => bn.Id == babyName.Id))
                    {
                        // Finds the existing MatchedBabyNames object from the database containing the users
                        var matchedBabyNames = await iMatchedBabyRepository.GetMatchedBabyNames(user, partner);

                        // Adds the babyname to the MatchedBabyNames objects LikedBabyNames list
                        await iMatchedBabyRepository.AddMatchedBabyNames(user, partner, babyName, matchedBabyNames);
                    }
                } catch (Exception e)
                {
                    return Results.BadRequest(e.Message);
                }
            }

            return Results.Ok();
        }
    }

    return Results.Unauthorized();
}).RequireAuthorization(builder => builder.RequireAssertion(context =>
{
    return context.User.HasClaim("role", "user") || context.User.HasClaim("role", "admin");
}));

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
}).RequireAuthorization(builder => builder.RequireAssertion(context =>
{
    return context.User.HasClaim("role", "user") || context.User.HasClaim("role", "admin");
}));


app.MapGet("/matches", async (IUserRepository iUserRepository, IMatchedBabyNamesRepository iMatchedBabyNamesRepository, HttpContext context) =>
{
    if (context.User.Identity.IsAuthenticated)
    {
        var userEmail = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        var user = await iUserRepository.GetUser(new User { Email = userEmail });

        if (user == null)
        {
            return Results.BadRequest("User does not exist");
        }

        if (user.Partner == null)
        {
            return Results.BadRequest("User does not have a partner");
        }

        var partner = await iUserRepository.GetUser(new User { Email = user.Partner.Email });

        if (partner == null)
        {
            return Results.BadRequest("Partner does not exist");
        }

        var matchedBabyNames = await iMatchedBabyNamesRepository.GetMatchedBabyNames(user, partner);

        return Results.Ok(matchedBabyNames);
    }

    return Results.Unauthorized();
}).RequireAuthorization(builder => builder.RequireAssertion(context =>
{
    return context.User.HasClaim("role", "user") || context.User.HasClaim("role", "admin");
}));

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

}).RequireAuthorization("admin");

#endregion

#region User Statistics Endpoint
app.MapGet("/users/count/daily", async (IUserRepository iUserRepository) =>
{
    return await iUserRepository.GetNewUsersDailyCountAsync();
}).RequireAuthorization("admin");

app.MapGet("/users/count/weekly", async (IUserRepository iUserRepository) =>
{
    return await iUserRepository.GetNewUsersWeeklyCountAsync();
}).RequireAuthorization("admin");

app.MapGet("/users/count/monthly", async (IUserRepository iUserRepository) =>
{
    return await iUserRepository.GetNewUsersMonthlyCountAsync();
}).RequireAuthorization("admin");

app.MapGet("/users/count/yearly", async (IUserRepository iUserRepository) =>
{
    return await iUserRepository.GetNewUsersYearlyCountAsync();
}).RequireAuthorization("admin");

#endregion

app.Run();