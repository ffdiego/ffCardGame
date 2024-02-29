using CardGame.Base;
using System.Text.Json;

namespace CardGame.Jogos
{
    public class Bisca: Jogo
    {
        // Definicoes fixas
        readonly int CartasNaMaoDeCadaJogador = 7;
        readonly TimeSpan tempoPorRodada = TimeSpan.FromSeconds(15);

        public Baralho Baralho;
        public List<Carta> Monte;

        public bool Iniciou;
        private Jogador? JogadorAtual;

        public Bisca()
        {
            Baralho = new Baralho();
            Monte = new List<Carta>();
            this.MaximosJogadores = 6;

            Iniciou = false;
        }

        public override bool IniciaJogo()
        {
            if (Jogadores.Count == 0)
            {
                return false;
            }

            foreach (var jogador in Jogadores)
            {
                jogador.Mao.AddRange(Baralho.PuxarCartas(CartasNaMaoDeCadaJogador));
            }

            Jogador jogadorAtual = Jogadores.FirstOrDefault()!;

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
                Baixo = baixo,
                Direita = direita,
                Cima = cima,
                Esquerda = esquerda,
                Monte = this.Monte
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
