using System.Net.WebSockets;
using System.Text;

enum EstadoAtual
{
    PerguntaGuid,
    PerguntaPartida,
    Conectado
}

namespace CardGame.Network
{
    public class ConexaoWebSocket: IDisposable
    {
        private HttpContext context;
        private WebSocket? webSocket;
        private EstadoAtual estado;
        public Guid Guid;
        public int Partida;
        public ConexaoWebSocket(HttpContext context)
        {
            this.context = context;
            this.estado = EstadoAtual.PerguntaGuid;
        }

        public async Task EscutaAsync()
        {
            this.webSocket = await context.WebSockets.AcceptWebSocketAsync();

            while (webSocket.State == WebSocketState.Open)
            {
                await Roda();
            }

            Console.WriteLine("Fechou a conexão");
        }

        public async Task Roda()
        {
            switch (this.estado)
            {
                case EstadoAtual.PerguntaGuid:
                    string resposta1 = await EnviaPerguntaAsync("Qual seu guid?", CancellationToken.None);

                    if (!Guid.TryParse(resposta1, out this.Guid))
                    {
                        await EnviaMensagemAsync("Guid inválido!");
                        return;
                    }
                    this.estado++;
                    break;

                case EstadoAtual.PerguntaPartida:
                    string resposta2 = await EnviaPerguntaAsync("Qual sua partida", CancellationToken.None);

                    if(!int.TryParse(resposta2, out this.Partida))
                    {
                        await EnviaMensagemAsync("Partida Inválida!");
                        return;
                    }
                    this.estado++;
                    break;

                case EstadoAtual.Conectado:
                    string resposta3 = await EnviaPerguntaAsync($"Você está na partida {this.Partida}", CancellationToken.None);
                    break;
            }
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
