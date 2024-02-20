using CardGame.Jogos;
using CardGame.Network;
using Microsoft.AspNetCore.Connections;

namespace CardGame.Base
{
    public class Jogador
    {
        public string Nome;
        public List<Carta> Mao;
        public Bisca? jogoAtual;
        public ConexaoWebSocket? conexao { get; set; }

        public Jogador(string nome)
        {
            Nome = nome;
            Mao = new List<Carta>();
        }

        public async Task<string> EnviaPerguntaAsync(string pergunta, CancellationToken cToken)
        {
            if (conexao == null)
            {
                throw new ConnectionAbortedException();
            }

            return await conexao.EnviaPerguntaAsync(pergunta, cToken);
        }
    }
}
