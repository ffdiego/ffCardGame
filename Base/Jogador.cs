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

        public Jogador(ConexaoWebSocket webSocket, string nome)
        {
            this.conexao = webSocket;
            this.Nome = nome;
            this.Mao = new List<Carta>();
        }

        public async Task<string> EnviaPerguntaAsync(string pergunta, CancellationToken cToken)
        {
            return await conexao.EnviaPerguntaAsync(pergunta, cToken);
        }
    }
}
