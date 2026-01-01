using MyWebAppDI.Interfaces;
using MyWebAppDI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IWelcomeService, WelcomeService>();

var app = builder.Build();

app.MapGet("/", (IWelcomeService welcomeService) => welcomeService.GetWelcomeMessage());

app.Run();