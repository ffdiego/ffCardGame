using CardGame.Base;

namespace CardGame.Jogos
{
    public class Bisca
    {
        // Definicoes fixas
        readonly int MaximosJogadores = 6;
        readonly int CartasNaMaoDeCadaJogador = 7;
        readonly TimeSpan tempoPorRodada = TimeSpan.FromSeconds(15);

        public Baralho Baralho;
        public List<Jogador> Jogadores;
        public bool Iniciou;
        private Jogador? JogadorAtual;

        public Bisca()
        {
            Baralho = new Baralho();
            Jogadores = new List<Jogador>();
            Iniciou = false;
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

        public bool IniciaJogo()
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

        private bool JogadorEstaNaPartida(Jogador jogador) 
        {
            if (!Jogadores.Contains(jogador))
            {
                throw new Exception($"Jogador {jogador.Nome} não está na partida!");
            }

            return true;
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
