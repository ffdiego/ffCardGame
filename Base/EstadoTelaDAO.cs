namespace CardGame.Base
{
    public class EstadoTelaDAO
    {
        public JogadorDAO? Esquerda;
        public JogadorDAO? Cima;
        public JogadorDAO? Direita;
        public JogadorDAO? Baixo;
        public List<Carta> Monte;
    }
}
