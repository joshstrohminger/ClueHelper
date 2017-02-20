namespace ClueHelper.Models
{
    internal static class Config
    {
        public const int CardsPerPlayer = 3;
        public const int MinPlayers = 3;

        public static Game GetDefaultGame()
        {
            var people = new Category.Builder("People")
                .AddCard("Ms. Scarlet")
                .AddCard("Col Mustard")
                .AddCard("Mrs. White")
                .AddCard("Mr. Green")
                .AddCard("Mrs. Peacock")
                .AddCard("Prof. Plum")
                .Build();

            var weapons = new Category.Builder("Weapons")
                .AddCard("Candlestick")
                .AddCard("Poison")
                .AddCard("Rope")
                .AddCard("Gloves")
                .AddCard("Horseshoe")
                .AddCard("Knife")
                .AddCard("Lead Pipe")
                .AddCard("Revolver")
                .AddCard("Wrench")
                .Build();

            var rooms = new Category.Builder("Rooms")
                .AddCard("Kitchen")
                .AddCard("Ballroom")
                .AddCard("Conservatory")
                .AddCard("Billiard Room")
                .AddCard("Library")
                .AddCard("Study")
                .AddCard("Hall")
                .AddCard("Lounge")
                .AddCard("Dining Room")
                .AddCard("Cellar")
                .Build();

            return new Game.Builder()
                .AddCategory(people)
                .AddCategory(rooms)
                .AddCategory(weapons)
                .AddPlayer("Jenny", people.Cards[1])
                .AddPlayer("Nancy", people.Cards[2])
                .AddPlayer("Daniel", people.Cards[5])
                .AddPlayer("Josh", people.Cards[4])
                .Build();
        }
    }
}
