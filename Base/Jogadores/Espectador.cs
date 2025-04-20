using CardGame.DAO;
using CardGame.Jogos;
using CardGame.Network;
using System.Text.Json;

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
            NotificaClient(jogo.ObtemInformacoes());
            jogo.OnAtualizacao += NotificaClient;
        }

        public async Task EscutaAsync() => await this.conexao.EscutaAsync();

        public void Dispose()
        {
            this.conexao.Dispose();
            this.jogo.OnAtualizacao -= NotificaClient;
        }

        public void NotificaClient(EstadoTelaDAO estadoTelaDAO)
        {
            var mensagem = new
            {
                tipo = "EstadoTela",
                conteudo = estadoTelaDAO
            };

            var mensagemJSON = JsonSerializer.Serialize(mensagem);
            this.conexao.EnviaMensagemAsync(mensagemJSON).RunSynchronously();
        }
    }
}
