using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Naipe
{
    Ouro,
    Espada,
    Copa,
    Paus
}

namespace CardGame.Base
{
    public class Carta
    {
        public Naipe Naipe;
        public int Numero;

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
}
