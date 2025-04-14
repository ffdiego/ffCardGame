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
        return;
    }

    using var ws = new ConexaoWebSocket(context, gerenciador);
    await ws.EscutaAsync();
});

app.Map("/{numero:int}", (int? numero, string? acao) =>
{
    if (numero == null) 
    {
        return Results.BadRequest();
    }

    var partida = gerenciador.GetPartida(numero.Value);

    if (partida == null)
    {
        return Results.NotFound();
    }

    if (!string.IsNullOrEmpty(acao))
    {
        switch (acao?.ToLower())
        {
            case "avanca":
                partida.Avanca();
                break;
            default:
                break;
        }
    }

    return Results.Json(partida.ObtemInformacoes());
});

await app.RunAsync();