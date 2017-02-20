namespace ClueHelper.Models
{
    internal class Player
    {
        public Card[] Hand { get; } = new Card[Config.CardsPerPlayer];
        public string Name { get; }
        public Card Representative { get; }

        public Player(string name, Card representative)
        {
            Name = name;
            Representative = representative;
        }

        public override string ToString()
        {
            return $"{Name} ({Representative.Name})";
        }
    }
}
