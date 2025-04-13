namespace CardGame.Base.Cartas;

public class Baralho
{
    public List<Carta> Cartas;
    public List<Carta> CartasRemovidas;
    public Baralho(int quantidadeBaralhos = 1, bool comCoringa = false)
    {
        int quantidadeNaipes = Enum.GetValues<Naipe>().Length;
        int quantidadeNumeros = 13;
        int quantidadeCoringas = comCoringa ? 2 : 0;

        int quantidadeCartas = quantidadeNaipes * quantidadeNumeros + quantidadeCoringas;
        Cartas = new List<Carta>(quantidadeCartas);
        CartasRemovidas = new List<Carta>(quantidadeCartas);

        foreach (var naipe in Enum.GetValues<Naipe>().Where(n => n != Naipe.Escondida))
        {
            for (int i = 1; i <= quantidadeNumeros; i++)
            {
                for (int b = 0; b < quantidadeBaralhos; b++)
                {
                    Cartas.Add(new Carta(naipe, i));
                }
            }
        }

        Embaralhar();
    }

    public List<Carta> PuxarCartas(int nCartas, bool remove = true)
    {
        List<Carta> cartasPuxadas = Cartas.TakeLast(nCartas).ToList();

        if (remove)
        {
            Cartas.RemoveRange(Cartas.Count - cartasPuxadas.Count, cartasPuxadas.Count);
            CartasRemovidas.AddRange(cartasPuxadas);
        }

        return cartasPuxadas;
    }

    public List<Carta> OlharCartasDoTopo(int nCartas) => PuxarCartas(nCartas, false);

    public void Embaralhar()
    {
        Random rand = new Random();
        int n = Cartas.Count;
        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);
            Carta carta = Cartas[k];
            Cartas[k] = Cartas[n];
            Cartas[n] = carta;
        }
    }
}
