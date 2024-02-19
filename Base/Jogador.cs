using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using CardGame.Network;

namespace CardGame.Base
{
    public class Jogador
    {
        public string Nome;
        public List<Carta> Mao;
        public ConexaoWebSocket? conexao { get; set; }

        public Jogador(string nome)
        {
            Nome = nome;
            Mao = new List<Carta>();
        }
    }
}
