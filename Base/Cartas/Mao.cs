namespace CardGame.Base.Cartas
{
    public class Mao : List<Carta>
    {
        public void Limpa() => this.RemoveAll(c => true);
        public void AdicionaCartas(List<Carta> cartas)
        {
            this.AddRange(cartas);
        }
    }
}
