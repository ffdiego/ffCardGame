using CardGame.Base.Jogadores;
using CardGame.Jogos;
using CardGame.Network;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:8080");

GerenciadorPartidas gerenciador = new GerenciadorPartidas();

var app = builder.Build();
app.UseWebSockets();

app.Map("/health", () =>
{
    return Results.Ok();
});

app.Map("/jogar/{numero:int}", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest || int.TryParse(context.Request.RouteValues["numero"]?.ToString(), out int numero))
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return;
    }

    var nome = context.Request.Query["nome"];
    if (string.IsNullOrEmpty(nome))
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Nome inválido ou ausente");
        return;
    }

    Console.WriteLine($"{nome} tentando conectar ao jogo {numero}");

    var partida = gerenciador.GetPartida(numero);
    if (partida is null)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        return;
    }

    var jogador = new JogadorWebsocket(new ConexaoWebSocket(context), nome);

    try
    {
        partida.AdicionaJogador(jogador);
    }
    catch (Exception e)
    {
        await context.Response.WriteAsync(e.Message);
        context.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
        jogador.Dispose();
        return;
    }

    await jogador.EscutaAsync();
});

app.Map("/assistir/{numero:int}", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest || int.TryParse(context.Request.RouteValues["numero"]?.ToString(), out int numero))
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return;
    }

    Console.WriteLine($"Novo espectador conectando ao jogo {numero}");

    var partida = gerenciador.GetPartida(numero);
    if (partida is null)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        return;
    }

    var espectador = new Espectador(new ConexaoWebSocket(context), partida);

    await espectador.EscutaAsync();
});

app.Map("/admin/{numero:int}", (int? numero, string? acao) =>
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