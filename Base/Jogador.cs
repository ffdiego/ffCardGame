using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace CardGame.Base
{
    public class Jogador
    {
        public string Nome;
        public List<Carta> Mao;
        private WebSocket ws;

        public Jogador(string nome)
        {
            Nome = nome;
            Mao = new List<Carta>();
        }

        public async Task EnviaMensagem(string mensagem)
        {
            var buffer = new byte[1024 * 4];
            await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task<string> RecebeResposta()
        {
            var buffer = new byte[1024 * 4];
            await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            return "oitutobom";
        }
    }
}
