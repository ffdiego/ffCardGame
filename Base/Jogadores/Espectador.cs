using CardGame.DAO;
using CardGame.Jogos;
using CardGame.Network;
using System.Net.WebSockets;
using System.Text;

namespace CardGame.Base.Jogadores
{
    public class Espectador : IDisposable
    {
        private ConexaoWebSocket conexao;
        private readonly Jogo jogo;

        public Espectador(ConexaoWebSocket conexao, Jogo jogo)
        {
            this.conexao = conexao;
            this.jogo = jogo;
            jogo.OnAtualizacao += RecebeAtualizacao;
        }

        public async Task EscutaAsync() => await this.conexao.EscutaAsync();

        public void Dispose()
        {
            this.conexao.Dispose();
            this.jogo.OnAtualizacao -= RecebeAtualizacao;
        }

        public void RecebeAtualizacao(EstadoTelaDAO estadoTelaDAO)
        {

        }
    }
}
