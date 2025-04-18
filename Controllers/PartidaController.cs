using CardGame.Base.Jogadores;
using CardGame.DAO;
using CardGame.Jogos;
using CardGame.Network;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CardGame.Controllers;

[ApiController]
public class PartidaController : ControllerBase
{
    private readonly GerenciadorPartidas gerenciador;

    public PartidaController(GerenciadorPartidas gerenciador)
    {
        this.gerenciador = gerenciador;
    }

    [HttpGet("/health")]
    public IResult Health()
    {
        return Results.Ok();
    }

    [HttpGet("/lista")]
    public ActionResult<List<Jogo>> Lista()
    {
        return this.gerenciador.listaPartidas;
    }

    [HttpGet("/jogar/{idPartida}")]
    public async Task<ActionResult> Jogar(int idPartida, [FromQuery] string nome)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            return BadRequest();
        }

        if (string.IsNullOrEmpty(nome))
        {
            return BadRequest("Nome inválido ou ausente");
        }

        Console.WriteLine($"{nome} tentando conectar ao jogo {idPartida}");

        var partida = gerenciador.GetPartida(idPartida);
        if (partida is null)
        {
            return NotFound();
        }

        var jogador = new JogadorWebsocket(new ConexaoWebSocket(HttpContext), nome);

        try
        {
            partida.AdicionaJogador(jogador);
        }
        catch (Exception e)
        {
            jogador.Dispose();
            return Problem(e.Message);
        }

        await jogador.EscutaAsync();

        return NoContent();
    }

    [HttpGet("/assistir/{idPartida}")]
    public async Task<ActionResult> Assistir(int idPartida)
    {
        if(!HttpContext.WebSockets.IsWebSocketRequest)
        {
            return BadRequest();
        }

        Console.WriteLine($"Novo espectador conectando ao jogo {idPartida}");

        var partida = gerenciador.GetPartida(idPartida);
        if (partida is null)
        {
            return NotFound();
        }

        var espectador = new Espectador(new ConexaoWebSocket(HttpContext), partida);

        await espectador.EscutaAsync();

        return NoContent();
    }

    [HttpGet("/admin/{numero:int}")]
    public ActionResult<EstadoTelaDAO> Admin(int numero, [FromQuery] string acao)
    {
        var partida = gerenciador.GetPartida(numero);
        if (partida == null)
        {
            return NotFound();
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

        return partida.ObtemInformacoes();
    }
}
