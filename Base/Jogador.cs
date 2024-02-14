using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame.Base
{
    public class Jogador
    {
        public string Nome;
        public List<Carta> Mao;

        public Jogador(string nome)
        {
            Nome = nome;
            Mao = new List<Carta>();
        }
    }
}
