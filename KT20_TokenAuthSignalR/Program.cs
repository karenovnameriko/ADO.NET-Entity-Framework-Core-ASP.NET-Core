using KT20_TokenAuthSignalR.Data;
using KT20_TokenAuthSignalR.Hubs;
using KT20_TokenAuthSignalR.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=users.db"));

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key is not configured.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT Issuer is not configured.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT Audience is not configured.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),

            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };

        // SignalR передаёт JWT через access_token в query string.
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/chatHub"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddSignalR();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapPost("/api/auth/register", async (
    RegisterRequest request,
    AppDbContext db,
    IPasswordHasher<User> passwordHasher) =>
{
    var userName = request.UserName.Trim();

    if (string.IsNullOrWhiteSpace(userName) || userName.Length < 3)
        return Results.BadRequest(new { message = "Имя пользователя должно содержать минимум 3 символа." });

    if (request.Password.Length < 6)
        return Results.BadRequest(new { message = "Пароль должен содержать минимум 6 символов." });

    if (request.Password != request.ConfirmPassword)
        return Results.BadRequest(new { message = "Пароли не совпадают." });

    if (await db.Users.AnyAsync(user => user.UserName == userName))
        return Results.Conflict(new { message = "Такой пользователь уже существует." });

    var user = new User
    {
        UserName = userName
    };

    user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Ok(new { message = "Регистрация прошла успешно." });
});

app.MapPost("/api/auth/login", async (
    LoginRequest request,
    AppDbContext db,
    IPasswordHasher<User> passwordHasher) =>
{
    var userName = request.UserName.Trim();

    var user = await db.Users
        .FirstOrDefaultAsync(user => user.UserName == userName);

    if (user == null)
        return Results.Unauthorized();

    var verificationResult = passwordHasher.VerifyHashedPassword(
        user,
        user.PasswordHash,
        request.Password);

    if (verificationResult == PasswordVerificationResult.Failed)
        return Results.Unauthorized();

    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.UserName)
    };

    var signingKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(jwtKey));

    var credentials = new SigningCredentials(
        signingKey,
        SecurityAlgorithms.HmacSha256);

    var expiresAt = DateTime.UtcNow.AddHours(1);

    var token = new JwtSecurityToken(
        issuer: jwtIssuer,
        audience: jwtAudience,
        claims: claims,
        expires: expiresAt,
        signingCredentials: credentials);

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(new
    {
        token = tokenString,
        userName = user.UserName,
        expiresAt
    });
});

app.MapRazorPages();
app.MapHub<ChatHub>("/chatHub");

app.Run();

