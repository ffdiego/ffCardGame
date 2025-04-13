using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CardGame.Base.Cartas;

public enum Naipe
{
    Escondida,
    Ouro,
    Espada,
    Copa,
    Paus
}

public class Carta
{
    public Naipe Naipe { get; }
    public int Numero { get; }

    public Carta()
    {
        Naipe = Naipe.Ouro;
        Numero = 0;
    }

    public Carta(Naipe naipe, int numero)
    {
        if (numero < 1 || numero > 13)
        {
            throw new ArgumentException("Número de carta inválido");
        }

        Naipe = naipe;
        Numero = numero;
    }

    public static Carta GerarCartaAleatoria()
    {
        Random rand = new Random();
        Naipe naipe = Enum.GetValues<Naipe>()[rand.Next(4)];
        int numero = rand.Next(13 + 1);

        return new Carta(naipe, numero);
    }
}

