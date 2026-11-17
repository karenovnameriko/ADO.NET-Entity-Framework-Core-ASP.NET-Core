using KT11_CryptoLibraries.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Регистрируем сервис шифрования
builder.Services.AddSingleton<CryptoService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();

app.Run();