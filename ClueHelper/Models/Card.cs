using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ClueHelper.Models
{
    public class Card
    {
        public Category Category { get; }
        public string Name { get; }
        public Player Holder { get; set; }
        public Player Representative { get; set; }
        public bool IsPartOfAccusation { get; set; }
        public ObservableCollection<PlayerPossibility> Possibilities { get; } = new ObservableCollection<PlayerPossibility>();

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
