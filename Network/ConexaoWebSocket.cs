using System.Net.WebSockets;
using System.Text;

namespace CardGame.Network
{
    public class ConexaoWebSocket: IDisposable
    {
        public bool ConexaoAtiva => webSocket?.State == WebSocketState.Open;
        private HttpContext context;
        private WebSocket? webSocket;

        public Guid GuidConexao;
        public event Action<string>? MensagemRecebida;

        public ConexaoWebSocket(HttpContext context)
        {
            this.context = context;
        }

        public async Task EscutaAsync()
        {
            this.webSocket = await context.WebSockets.AcceptWebSocketAsync();

            while (webSocket.State == WebSocketState.Open)
            {
                byte[] buffer = new byte[1024];
                await this.webSocket!.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string mensagem = Encoding.UTF8.GetString(buffer).TrimEnd((char)0);

                MensagemRecebida?.Invoke(mensagem);
            }

            Console.WriteLine("Fechou a conexão");

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
