using CardGame.Base;

public enum EstadoTela
{
    Nada,
    TocaAnimacao,
}

namespace CardGame.DAO
{
    public class EstadoTelaDAO
    {
        public EstadoTela EstadoTela = EstadoTela.Nada;
        public string? Animacao;
        public Jogador? Jogador;
        public List<JogadorDAO>? OutrosJogadores;
        public List<Carta>? Monte;
    }
}
