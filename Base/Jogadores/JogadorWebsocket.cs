using CardGame.Base.Cartas;
using CardGame.Network;

namespace CardGame.Base.Jogadores
{
    public class JogadorWebsocket : Jogador
    {
        private ConexaoWebSocket conexao;

        public JogadorWebsocket(ConexaoWebSocket webSocket, string nome) : base(nome)
        {
            conexao = webSocket;
        }

        public async Task<string> EnviaPerguntaAsync(string pergunta, CancellationToken cToken)
        {
            return await conexao.EnviaPerguntaAsync(pergunta, cToken);
        }
    }
}
