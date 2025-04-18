using CardGame.Base.Jogadores;
using CardGame.Controllers;
using CardGame.Jogos;
using CardGame.Network;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<GerenciadorPartidas>();
builder.Services.AddControllers();
builder.WebHost.UseUrls("http://0.0.0.0:8080");

var app = builder.Build();
app.UseWebSockets();
app.MapControllers();

await app.RunAsync();