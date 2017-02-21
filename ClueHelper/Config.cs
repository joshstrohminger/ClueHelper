using ClueHelper.Models;

namespace ClueHelper
{
    public static class Config
    {
        public const int MinPlayers = 3;

        public static Category BuildDefaultPeople()
        {
            return new Category.Builder("People")
                .AddCard("Col. Mustard")
                .AddCard("Prof. Plum")
                .AddCard("Mr. Green")
                .AddCard("Ms. Scarlet")
                .AddCard("Mrs. Peacock")
                .AddCard("Mrs. White")
                .Build();
        }

        public static Category BuildDefaultWeapons()
        {
            return new Category.Builder("Weapons")
                .AddCard("Knife")
                .AddCard("Candlestick")
                .AddCard("Revolver")
                .AddCard("Rope")
                .AddCard("Lead Pipe")
                .AddCard("Wrench")
                .Build();
        }

        public static Category BuildDefaultRooms()
        {
            return new Category.Builder("Rooms")
                .AddCard("Hall")
                .AddCard("Lounge")
                .AddCard("Dining Room")
                .AddCard("Kitchen")
                .AddCard("Ballroom")
                .AddCard("Conservatory")
                .AddCard("Billiard Room")
                .AddCard("Library")
                .AddCard("Study")
                .Build();
        }

        public static Game BuildDefaultGame()
        {
            var people = BuildDefaultPeople();
            var rooms = BuildDefaultRooms();
            var weapons = BuildDefaultWeapons();

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
