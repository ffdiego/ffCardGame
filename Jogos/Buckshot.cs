using CardGame.Base;
using CardGame.DAO;
using System.Text.Json;

namespace CardGame.Jogos
{
    public class Buckshot: Jogo
    {
        // Definicoes fixas

        readonly TimeSpan tempoPorRodada = TimeSpan.FromSeconds(15);

        private Jogador? JogadorAtual;

        public Buckshot()
        {
            this.MaximosJogadores = 2;
        }

        public override bool IniciaJogo()
        {
            if (Jogadores.Count == 0)
            {
                return false;
            }

            Jogador jogadorAtual = Jogadores.First();

            return true;
        }

        public async void RespondeStatus(Jogador jogador)
        {
            if (!JogadorEstaNaPartida(jogador))
            {
                return;
            }

            await jogador.EnviaPerguntaAsync("mão: 11, k3, 21", CancellationToken.None);
        }

        public override EstadoTelaDAO ObtemInformacoes(Jogador jogador)
        {
            if (!JogadorEstaNaPartida(jogador))
            {
                throw new Exception($"Jogador {jogador.Nome} requisitou tela e não está na partida!");
            }

            Jogador baixo = jogador;
            Jogador direita = ObtemProximo(jogador);
            Jogador cima = ObtemProximo(direita);
            Jogador esquerda = ObtemProximo(cima);

            return new EstadoTelaDAO()
            {
                
            };
        }

        private bool JogadorEstaNaPartida(Jogador jogador) => Jogadores.Contains(jogador);

        private Jogador ObtemProximo(Jogador jogador) 
        {
            int indexJogador = Jogadores.IndexOf(jogador);

            if (indexJogador < Jogadores.Count - 1)
            {
                return Jogadores[indexJogador + 1];
            }

            return Jogadores[0];
        } 

        public async void ExecutaRodada()
        {
            var cancelToken = new CancellationTokenSource(tempoPorRodada);

            Task<string> respostaJogador = JogadorAtual!.EnviaPerguntaAsync("suavez", cancelToken.Token);

            Task terminouPrimeiro = await Task.WhenAny(respostaJogador, Task.Delay(tempoPorRodada));
            
            if (terminouPrimeiro == respostaJogador)
            {
                Console.WriteLine($"Resposta do jogador: {respostaJogador.Result}");
            } 
            else
            {
                Console.WriteLine("O jogador demorou muito pra responder!");
            }

        }
    }
}
