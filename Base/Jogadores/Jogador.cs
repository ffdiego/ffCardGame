using CardGame.Base.Cartas;

namespace CardGame.Base.Jogadores;

public class Jogador
{
    public string Nome;

    public Mao Mao;

    public int Dinheiro;

    public Jogador(string nome)
    {
        Nome = nome;
        Mao = new Mao();
        Dinheiro = 0;
    }
}
