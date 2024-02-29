using CardGame.Base;
using System.Net.WebSockets;
using System.Text;

enum EstadoAtual
{
    Anonimo,
    Identificado
}

namespace CardGame.Network
{
    public class ConexaoWebSocket: IDisposable
    {
        private HttpContext context;
        private WebSocket? webSocket;
        private EstadoAtual estado;

        public Guid Guid;
        public string Nome;
        public Jogador? Jogador;

        public ConexaoWebSocket(HttpContext context)
        {
            this.context = context;
            this.estado = EstadoAtual.Anonimo;
        }

        public async Task EscutaAsync()
        {
            this.webSocket = await context.WebSockets.AcceptWebSocketAsync();

            while (webSocket.State == WebSocketState.Open)
            {
                await EsperaMensagem();
            }

            Console.WriteLine("Fechou a conexão");
        }

        private async Task EsperaMensagem()
        {
            byte[] buffer = new byte[1024];

            await this.webSocket!.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            string mensagem = Encoding.UTF8.GetString(buffer).TrimEnd((char)0);

            await Responde(mensagem);
        }

        public async Task Responde(string mensagem)
        {
            switch (this.estado)
            {
                case EstadoAtual.Anonimo:
                    if (mensagem == "quero_guid")
                    {
                        await EnviaMensagemAsync(Guid.NewGuid().ToString());
                        break;
                    }

                    string[] partes = mensagem.Split(',');

                    if (partes.Length != 3)
                    {
                        await EnviaMensagemAsync("Envie Nome, GUID, Jogo");
                        return;
                    }

                    string nome = mensagem.Split(',')[0];
                    string guid = mensagem.Split(',')[1];
                    string jogo = mensagem.Split(',')[2];

                    if (!Guid.TryParse(guid, out this.Guid))
                    {
                        await EnviaMensagemAsync("GUID invalido");
                        return;
                    }

                    this.estado++;

                    this.Jogador.Nome = nome;

                    break;

                case EstadoAtual.Identificado:
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

            await this.webSocket.ReceiveAsync(new ArraySegment<byte>(bufferResposta), CancellationToken.None);

            return Encoding.UTF8.GetString(bufferResposta).TrimEnd((char)0);
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
