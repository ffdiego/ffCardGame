using CardGame.Jogos;
using CardGame.Network;
using Microsoft.AspNetCore.Connections;

namespace CardGame.Base
{
    public class JogadorDAO
    {
        public string Nome { get; set; } = string.Empty;
        public List<Carta>? Mao { get; set; }

        public static implicit operator JogadorDAO(Jogador jogador)
        {
            return new JogadorDAO { Nome = jogador.Nome, Mao = jogador.Mao };
        }
    }
}
