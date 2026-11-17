using KT14_ManyToMany.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=kt14.db"));

var app = builder.Build();

// Создание базы данных при первом запуске.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();

app.Run();