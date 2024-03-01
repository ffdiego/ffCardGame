using CardGame.Base;
using System.Text.Json;

namespace CardGame.Jogos
{
    public abstract class Jogo
    {
        private static int JogosCount = 0;
        public int Id { get; }

        public List<Jogador> Jogadores;
        public int MaximosJogadores { get; protected set; }

        public Jogo()
        {
            this.Jogadores = new List<Jogador>();
            this.Id = JogosCount++;
        }

        public bool AdicionaJogador(Jogador jogador)
        {
            if (Jogadores.Count > (MaximosJogadores + 1))
            {
                return false;
            }

            Jogadores.Add(jogador);
            return true;
        }

        public abstract bool IniciaJogo();

        public abstract EstadoTelaDAO ObtemInformacoes(Jogador jogador);
    }
}
