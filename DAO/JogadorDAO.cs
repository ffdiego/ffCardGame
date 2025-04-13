using CardGame.Base.Cartas;
using CardGame.Base.Jogadores;

namespace CardGame.DAO;

public class JogadorDAO
{
    public string Nome { get; set; } = string.Empty;
    public Mao Mao { get; set; }

    public static explicit operator JogadorDAO(Jogador jogador)
    {
        return new JogadorDAO { Nome = jogador.Nome, Mao = jogador.Mao };
    }
}
