using CardGame.Base.Cartas;
using CardGame.Base.Jogadores;

namespace CardGame.DAO;

public enum EstadoTela
{
    Nada,
    TocaAnimacao,
}

public class EstadoTelaDAO
{
    public JogadorDAO? Jogador;
    public IEnumerable<JogadorDAO>? OutrosJogadores;
    public IEnumerable<Carta> CartasMesa;
    public int QuantidadeMonte;
}
