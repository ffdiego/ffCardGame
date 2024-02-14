using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame
{
    public class Baralho
    {
        public List<Carta> Cartas;
        public List<Carta> CartasRemovidas;
        public Baralho(bool comCoringa = false) 
        {
            int quantidadeNaipes = Enum.GetValues<Naipe>().Length;
            int quantidadeNumeros = 13;
            int quantidadeCoringas = comCoringa ? 2 : 0;

            Cartas = new List<Carta>(quantidadeNaipes * quantidadeNumeros + quantidadeCoringas);
            CartasRemovidas = new List<Carta>(quantidadeNaipes * quantidadeNumeros + quantidadeCoringas);
            
            foreach (var naipe in Enum.GetValues<Naipe>()) 
            { 
                for(int i = 1; i <= quantidadeNumeros; i++)
                {
                    Cartas.Add(new Carta(naipe, i));
                }
            }

            Embaralhar();
        }

        public List<Carta> PuxarCartas(int nCartas)
        {
            List<Carta> cartasPuxadas = Cartas.TakeLast(nCartas).ToList();

            Cartas.RemoveRange(Cartas.Count - cartasPuxadas.Count, cartasPuxadas.Count);
            CartasRemovidas.AddRange(cartasPuxadas);

            return cartasPuxadas;
        }

        public List<Carta> OlharCartasDoTopo(int nCartas)
        {
            List<Carta> cartasPuxadas = Cartas.TakeLast(nCartas).ToList();

            return cartasPuxadas;
        }

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
}
