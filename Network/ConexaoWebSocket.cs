using System;
using System.Net.WebSockets;
using System.Text;

namespace CardGame.Network
{
    public class ConexaoWebSocket: IDisposable
    {
        private HttpContext context;
        private WebSocket? webSocket;
        public Guid guid;
        public ConexaoWebSocket(HttpContext context)
        {
            this.context = context;
        }

        public async Task EscutaAsync()
        {
            this.webSocket = await context.WebSockets.AcceptWebSocketAsync();

            while (webSocket.State == WebSocketState.Open)
            {
                bool guidValido = false;

                while (guidValido)
                {
                    string resposta = await EnviaPerguntaAsync("Qual seu guid?", CancellationToken.None);
                    guidValido = Guid.TryParse(resposta, out this.guid);
                }
            }

            Console.WriteLine("Fechou a conexão");
        }

        public async Task<string> EnviaPerguntaAsync(string pergunta, CancellationToken cancellationToken)
        {
            if (webSocket == null)
            {
                throw new Exception("WebSocket indefinido");
            }

            byte[] bufferPergunta = Encoding.UTF8.GetBytes(pergunta);
            byte[] bufferResposta = new byte[1024 * 4];

            await this.webSocket.SendAsync(new ArraySegment<byte>(bufferPergunta), WebSocketMessageType.Text, true, CancellationToken.None);

            await webSocket.ReceiveAsync(new ArraySegment<byte>(bufferResposta), CancellationToken.None);
            
            return UTF8Encoding.UTF8.GetString(bufferResposta);
        }

        public async Task EnviaMensagemAsync(string mensagem)
        {
            if (webSocket == null)
            {
                throw new Exception("WebSocket indefinido");
            }
            byte[] buffer = Encoding.UTF8.GetBytes(mensagem);
            await this.webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            webSocket?.Dispose();
        }
    }
}
