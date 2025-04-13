using CardGame.Base.Cartas;
using CardGame.Base.Jogadores;
using CardGame.DAO;

namespace CardGame.Jogos;

public class Blackjack : Jogo
{
    private enum EstadoJogo
    {
        EsperandoJogadoresConectarem,
        DistribuiCartas,
        JogadoresCompram,
        CasaCompra,
        VerificaResultados
    }

    private enum EstadoMao
    {
        AbaixoDe17,
        DezesseteOuMais,
        VinteUm,
        Estorou
    }

    private enum Resultado
    {
        Venceu,
        Perdeu,
        Empate
    }

    private void DistribuiCartas()
    {
        this.estadoJogo = EstadoJogo.DistribuiCartas;

        foreach (var jogador in Jogadores.Concat(new[] { casa }))
        {
            jogador.Mao.Limpa();
            jogador.Mao.AdicionaCartas(this.baralho.PuxarCartas(2));
        }
    }

    private async void JogadoresCompram()
    {
        this.estadoJogo = EstadoJogo.JogadoresCompram;

        this.resultadosRodada = new();

        foreach (var jogador in Jogadores)
        {
            var somaCartas = SomaCartas(jogador.Mao);

            var resposta = string.Empty;

            while (resposta != "stand")
            {
                resposta = await jogador.EnviaPerguntaAsync("Vai ou racha?", new CancellationToken());

                if (resposta == "hit")
                {
                    jogador.Mao.AdicionaCartas(this.baralho.PuxarCartas(1));
                    somaCartas = SomaCartas(jogador.Mao);

                    if (somaCartas.estado == EstadoMao.Estorou)
                    {
                        break;
                    }
                }
            }

            resultadosRodada.Add(jogador, somaCartas);
        }
    }

    private void CasaCompra()
    {
        this.estadoJogo = EstadoJogo.CasaCompra;

        var somaCartas = SomaCartas(casa.Mao);

        while (somaCartas.soma < 17)
        {
            casa.Mao.AdicionaCartas(this.baralho.PuxarCartas(1));
            Thread.Sleep(TimeSpan.FromSeconds(2));
            somaCartas = SomaCartas(casa.Mao);
        }

        resultadosRodada.Add(casa, somaCartas);
    }

    private void VerificaResultados()
    {
        this.estadoJogo = EstadoJogo.VerificaResultados;

        var resultadoCasa = this.resultadosRodada[casa];

        foreach (var jogador in Jogadores)
        {
            var resultadoJogador = this.resultadosRodada[jogador];

            Resultado resultado = (resultadoJogador.estadoMao, resultadoCasa.estadoMao, resultadoJogador.soma, resultadoCasa.soma) switch
            {
                (EstadoMao.Estorou, _, _, _) => Resultado.Perdeu,
                (_, EstadoMao.Estorou, _, _) => Resultado.Venceu,
                (_, _, var somaJ, var somaC) when somaJ == somaC => Resultado.Empate,
                (_, _, var somaJ, var somaC) when somaJ > somaC => Resultado.Venceu,
                _ => Resultado.Perdeu
            };

            switch (resultado)
            {
                case Resultado.Venceu:
                    jogador.Dinheiro += valorRodada;
                    break;
                case Resultado.Perdeu:
                    casa.Dinheiro += valorRodada;
                    break;
                default:
                    break;
            }
        }
    }



    private static (int soma, EstadoMao estado) SomaCartas(Mao mao)
    {
        int somaSemAs = mao.Where(c => c.Numero != 1).Sum(c => c.Numero);
        int quantidadeAs = mao.Where(c => c.Numero == 1).Count();

        int somaTotal(int asDesconsiderados)
        {
            return somaSemAs + (quantidadeAs * 11) - (asDesconsiderados * 10);
        }

        int soma = somaTotal(0);
        int asDesconsiderados = 0;
        while (soma > 21 && asDesconsiderados < quantidadeAs)
        {
            soma = somaTotal(asDesconsiderados++);
        }

        EstadoMao estado =
            (soma < 17) ? EstadoMao.AbaixoDe17 :
            soma > 17 && soma < 21 ? EstadoMao.DezesseteOuMais :
            soma == 21 ? EstadoMao.VinteUm :
            EstadoMao.Estorou;

        return (soma, estado);
    }

    private readonly Dictionary<EstadoJogo, Action> acoesEstado;
    private Baralho baralho;
    private EstadoJogo estadoJogo;

    private Jogador casa;

    // Por rodada
    private Dictionary<Jogador, (int soma, EstadoMao estadoMao)>? resultadosRodada;
    private int valorRodada;

    public Blackjack() : base()
    {
        baralho = new Baralho(quantidadeBaralhos: 5);
        estadoJogo = EstadoJogo.EsperandoJogadoresConectarem;
        casa = new Jogador("Casa");
        valorRodada = 50;

        this.acoesEstado = new Dictionary<EstadoJogo, Action>()
        {
            { EstadoJogo.EsperandoJogadoresConectarem, () => { } },
            { EstadoJogo.DistribuiCartas, DistribuiCartas },
            { EstadoJogo.JogadoresCompram, JogadoresCompram },
            { EstadoJogo.CasaCompra, CasaCompra },
            { EstadoJogo.VerificaResultados, VerificaResultados }
        };
    }

    public override bool IniciaJogo()
    {
        DistribuiCartas();
        return true;
    }

    public override bool Avanca()
    {
        if (this.estadoJogo == EstadoJogo.VerificaResultados)
        {
            this.estadoJogo = EstadoJogo.DistribuiCartas;
        }

        this.acoesEstado[this.estadoJogo]();

        return true;
    }

    public override EstadoTelaDAO ObtemInformacoes(Jogador jogador)
    {
        var cartasMesa = new List<Carta> { casa.Mao.First() };
        cartasMesa.AddRange(
            estadoJogo < EstadoJogo.CasaCompra
                ? new[] { new Carta() }
                : casa.Mao.Skip(1)
        );

        return new EstadoTelaDAO()
        {
            Jogador = null,
            OutrosJogadores = Jogadores.Select(j => (JogadorDAO)j),
            CartasMesa = cartasMesa,
            QuantidadeMonte = this.baralho.Cartas.Count()
        };
    }
}
