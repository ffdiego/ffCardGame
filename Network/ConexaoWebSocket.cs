using CardGame.Base.Jogadores;
using CardGame.Jogos;
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
        private GerenciadorPartidas gerenciadorPartidas;

        public Guid GuidConexao;
        public JogadorWebsocket? Jogador;
        public event Action<string>? MensagemRecebida;

        public ConexaoWebSocket(HttpContext context, GerenciadorPartidas gerenciadorPartidas)
        {
            this.context = context;
            this.gerenciadorPartidas = gerenciadorPartidas;
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
                        string novaGuid = Guid.NewGuid().ToString();
                        await EnviaMensagemAsync(novaGuid);
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

                    if (!Guid.TryParse(guid, out this.GuidConexao))
                    {
                        await EnviaMensagemAsync("GUID invalido");
                        return;
                    }

                    if (nome == string.Empty)
                    {
                        await EnviaMensagemAsync("Nome vazio");
                        return;
                    }

                    this.Jogador = new JogadorWebsocket(this, nome);

                    try {
                        var partida = gerenciadorPartidas.GetPartida(int.Parse(jogo));

                        if (partida == null)
                        {
                            await EnviaMensagemAsync($"Partida {jogo} não encontrada!");
                        }
                        else
                        {
                            partida.AdicionaJogador(this.Jogador);
                            await EnviaMensagemAsync($"Jogador {nome} entrou no jogo {jogo}");
                        }
                    } catch (Exception ex) { 
                        await EnviaMensagemAsync(ex.Message);
                        return;
                    }

                    estado++;
                    break;
                case EstadoAtual.Identificado:
                    MensagemRecebida?.Invoke(mensagem);
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
