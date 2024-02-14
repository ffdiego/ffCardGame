using CardGame.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private Jogador JogadorAtual;

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

        public async void ExecutaRodada()
        {
            await JogadorAtual.EnviaMensagem("suavez");
            
            Task tempoEsgotado = Task.Delay(tempoPorRodada);
            Task<string> respostaJogador = JogadorAtual.RecebeResposta();

            var timeoutRodada = new CancellationTokenSource(tempoPorRodada);

            Task terminouPrimeiro = await Task.WhenAny(tempoEsgotado, Task.Delay(tempoPorRodada));

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
