using CardGame.Base.Cartas;

namespace CardGame.Base.Jogadores;

public class Jogador
{
    public string Nome { get; set; }

    public Mao Mao { get; set; }

    public decimal Dinheiro
    {
        get => this.dinheiro;
        set
        {
            this.dinheiro = Math.Round(value, 2);
        }
    }
    private decimal dinheiro;

    public Jogador(string nome)
    {
        Nome = nome;
        Mao = new Mao();
        Dinheiro = 0;
    }
}
