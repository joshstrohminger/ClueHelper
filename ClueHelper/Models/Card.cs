using System.Collections.Generic;

namespace ClueHelper.Models
{
    internal class Card
    {
        public Category Category { get; }
        public string Name { get; }
        public Player Holder { get; set; }
        public Player Representative { get; set; }

        public Card(Category category, string name)
        {
            Category = category;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
