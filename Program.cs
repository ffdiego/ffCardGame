using CardGame.Network;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:8080");

var app = builder.Build();
app.UseWebSockets();

app.Map("/", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    else
    {
        using var ws = new ConexaoWebSocket(context);
        await ws.EscutaAsync();
    }
});

await app.RunAsync();