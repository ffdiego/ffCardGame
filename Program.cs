using CardGame.Jogos;
using CardGame.Network;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:8080");

GerenciadorPartidas gerenciador = new GerenciadorPartidas();

var app = builder.Build();
app.UseWebSockets();

app.Map("/", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
    else
    {
        using var ws = new ConexaoWebSocket(context, gerenciador);
        await ws.EscutaAsync();
    }
});

await app.RunAsync();