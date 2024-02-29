namespace CardGame.Jogos
{
    public class GerenciadorPartidas
    {
        public List<Jogo> listaPartidas;
        public GerenciadorPartidas() 
        { 
            listaPartidas = new List<Jogo>();
        }

        public Jogo CriaPartida()
        {
            Jogo jogo = new Bisca();
            listaPartidas.Add(jogo);
            return jogo;
        }

        public Jogo? GetPartida(int id) => listaPartidas.Where(p => p.Id == id).SingleOrDefault();
    }
}
