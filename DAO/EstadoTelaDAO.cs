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
    public string Estado { get; set; }
    public int NumeroJogadores { get; set; }
    public JogadorDAO? Jogador { get; set; }
    public IEnumerable<JogadorDAO>? OutrosJogadores { get; set; }
    public IEnumerable<Carta> CartasMesa { get; set; }
    public int QuantidadeMonte { get; set; }
}
