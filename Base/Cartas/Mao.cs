using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CardGame.Base.Cartas
{
    public class Mao : List<Carta>
    {
        public event Action? CollectionChanged;

        public IEnumerable<Carta> Figuras => this.Where(c => c.Numero >= 11);
        public IEnumerable<Carta> As => this.Where(c => c.Numero == 1);
        public IEnumerable<Carta> Comuns => this.Where(c => c.Numero < 11 && c.Numero > 1);

        public void Limpa() 
        {
            this.RemoveAll(c => true);
            this.CollectionChanged?.Invoke();
        }
        public void AdicionaCartas(List<Carta> cartas)
        {
            this.AddRange(cartas);
            this.CollectionChanged?.Invoke();
        }
    }
}
