using CardGame.Base.Jogadores;
using CardGame.DAO;
using System.Text.Json;

namespace CardGame.Jogos
{
    public abstract class Jogo
    {
        public int Id { get; }
        public int MaximosJogadores { get; protected set; }
        public string Tipo { get => this.GetType().Name; }
        public int NumeroJogadores { get => this.Jogadores.Count; }
        
        private static int JogosCount = 0;
        public bool Iniciou;
        public List<JogadorWebsocket> Jogadores;
        public event Action<EstadoTelaDAO>? OnAtualizacao;


        public Jogo()
        {
            this.Jogadores = new();
            this.Id = JogosCount++;
            this.Iniciou = false;
        }

        public bool AdicionaJogador(JogadorWebsocket jogador)
        {
            if (Jogadores.Count >= MaximosJogadores)
            {
                throw new Exception($"Número máximo de jogadores já atingido: {Jogadores.Count}/{MaximosJogadores}");
            }

            Jogadores.Add(jogador);
            return true;
        }

        protected void EnviaAtualizacoes()
        {
            this.OnAtualizacao?.Invoke(this.ObtemInformacoes());
        }

        public abstract bool IniciaJogo();

        public abstract bool Avanca();

        public abstract EstadoTelaDAO ObtemInformacoes(Jogador? jogador = null);
    }
}
