using CardGame.Base.Cartas;
using CardGame.Network;
using System.Net.WebSockets;
using System.Threading;

namespace CardGame.Base.Jogadores
{
    public class JogadorWebsocket : Jogador, IDisposable
    {
        public bool Conectado => conexao.ConexaoAtiva;
        private ConexaoWebSocket conexao;

        public JogadorWebsocket(ConexaoWebSocket webSocket, string nome) : base(nome)
        {
            conexao = webSocket;
        }

        public async Task EscutaAsync() => await this.conexao.EscutaAsync();

        public async Task EnviaPergunta(string pergunta, TimeSpan timeout, Func<string, bool> handler)
        {
            await this.conexao.EnviaMensagemAsync(pergunta);

            var tcs = new TaskCompletionSource<bool>();
             
            async void HandlerInterno(string msg)
            {
                try
                {
                    while (!handler(msg));
                }
                finally
                {
                    tcs.TrySetResult(true);
                }
            }

            this.conexao.MensagemRecebida += HandlerInterno;

            var delay = Task.Delay(timeout);
            var completed = await Task.WhenAny(tcs.Task, delay);

            this.conexao.MensagemRecebida -= HandlerInterno;

            if (completed == delay)
            {
                await this.conexao.EnviaMensagemAsync("Você não respondeu a tempo, pulando sua vez");
            }
        }

        public void Dispose()
        {
            conexao.Dispose();
        }
    }
}
