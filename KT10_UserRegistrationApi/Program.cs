using KT10_UserRegistrationApi.Data;
using KT10_UserRegistrationApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("UsersDatabase"));

builder.Services.AddScoped<IPasswordHasher<AppUser>,
    PasswordHasher<AppUser>>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();

var users = app.MapGroup("/api/users");

// GET: получить всех пользователей
users.MapGet("/", async (AppDbContext db) =>
{
    var result = await db.Users
        .AsNoTracking()
        .Select(u => new UserResponse
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            CreatedAt = u.CreatedAt
        })
        .ToListAsync();

    return Results.Ok(result);
});

// GET: получить пользователя по ID
users.MapGet("/{id:int}", async (int id, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(id);

    if (user == null)
        return Results.NotFound(new { message = "Пользователь не найден" });

    return Results.Ok(new UserResponse
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        CreatedAt = user.CreatedAt
    });
});

// POST: добавить пользователя
users.MapPost("/", async (UserRequest request, AppDbContext db) =>
{
    var errors = ValidationHelper.Validate(request);

    if (errors.Count > 0)
        return Results.ValidationProblem(errors);

    var email = request.Email.Trim().ToLowerInvariant();

    if (await db.Users.AnyAsync(u => u.Email == email))
    {
        return Results.Conflict(new
        {
            message = "Пользователь с такой почтой уже существует"
        });
    }

    var user = new AppUser
    {
        Username = request.Username.Trim(),
        Email = email,
        CreatedAt = DateTime.UtcNow
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Created($"/api/users/{user.Id}", new UserResponse
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        CreatedAt = user.CreatedAt
    });
});

// PUT: обновить пользователя
users.MapPut("/{id:int}", async (
    int id,
    UserRequest request,
    AppDbContext db) =>
{
    var errors = ValidationHelper.Validate(request);

    if (errors.Count > 0)
        return Results.ValidationProblem(errors);

    var user = await db.Users.FindAsync(id);

    if (user == null)
        return Results.NotFound(new { message = "Пользователь не найден" });

    var email = request.Email.Trim().ToLowerInvariant();

    if (await db.Users.AnyAsync(u => u.Email == email && u.Id != id))
    {
        return Results.Conflict(new
        {
            message = "Эта электронная почта уже используется"
        });
    }

    user.Username = request.Username.Trim();
    user.Email = email;

    await db.SaveChangesAsync();

    return Results.Ok(new UserResponse
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        CreatedAt = user.CreatedAt
    });
});

// DELETE: удалить пользователя
users.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(id);

    if (user == null)
        return Results.NotFound(new { message = "Пользователь не найден" });

    db.Users.Remove(user);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();