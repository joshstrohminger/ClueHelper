using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ClueHelper.Models
{
    public class Card : ObservableObject
    {
        private bool _isPartOfAccustion;
        private bool _isPartOfSuggestion;

        public Category Category { get; }
        public string Name { get; }
        public Player Holder { get; set; }
        public Player Representative { get; set; }
        public ObservableCollection<PlayerPossibility> Possibilities { get; } = new ObservableCollection<PlayerPossibility>();

        public bool IsPartOfAccusation
        {
            get { return _isPartOfAccustion; }
            set
            {
                _isPartOfAccustion = value;
                OnPropertyChanged();
            }
        }

        public bool IsPartOfSuggestion
        {
            get { return _isPartOfSuggestion; }
            set
            {
                _isPartOfSuggestion = value;
                OnPropertyChanged();
            }
        }

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
