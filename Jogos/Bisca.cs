using CardGame.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame.Jogos
{
    public class Bisca
    {
        public Baralho Baralho;
        public List<Jogador> Jogadores;

        public Bisca()
        {
            Baralho = new Baralho();
            Jogadores = new List<Jogador>();
        }
    }
}
