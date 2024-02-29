using CardGame.Jogos;
using CardGame.Network;
using Microsoft.AspNetCore.Connections;

namespace CardGame.Base
{
    public class Jogador
    {
        private ConexaoWebSocket conexao;

        public string Nome;
        public List<Carta> Mao;
        public Bisca? jogoAtual;

        public Jogador(ConexaoWebSocket webSocket)
        {
            this.conexao = webSocket;

            this.Nome = "sem_nome";
            this.Mao = new List<Carta>();
        }

        public async Task<string> EnviaPerguntaAsync(string pergunta, CancellationToken cToken)
        {
            return await conexao.EnviaPerguntaAsync(pergunta, cToken);
        }
    }
}
