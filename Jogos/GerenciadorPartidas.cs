using CardGame.DAO;
using System.Text.Json;

namespace CardGame.Jogos
{
    public class GerenciadorPartidas
    {
        public List<Jogo> listaPartidas;
        public GerenciadorPartidas() 
        { 
            listaPartidas = new List<Jogo>();
            var blackjack = new Blackjack();
            listaPartidas.Add(blackjack);

            blackjack.OnAtualizacao += Blackjack_OnAtualizaEspectadores;
        }

        private void Blackjack_OnAtualizaEspectadores(EstadoTelaDAO obj)
        {
            var prettyJson = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
            Console.Write(prettyJson);
        }

        public Jogo CriaPartida()
        {
            Jogo jogo = new Blackjack();
            listaPartidas.Add(jogo);
            return jogo;
        }

        public Jogo? GetPartida(int id) => listaPartidas.Where(p => p.Id == id).SingleOrDefault();
    }
}
