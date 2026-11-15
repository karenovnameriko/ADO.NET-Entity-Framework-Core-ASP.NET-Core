using KT6_UserUpdate;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<Database>();

var app = builder.Build();

var database = app.Services.GetRequiredService<Database>();
database.Initialize();

app.MapControllers();

app.Run();