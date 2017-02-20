using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ClueHelper.Models
{
    public class Player : ObservableObject
    {
        private bool _isTakingTurn;

        public IReadOnlyCollection<Card> Hand { get; private set; } = new ReadOnlyCollection<Card>(new List<Card>());
        public string Name { get; }
        public Card Representative { get; }

        public bool IsTakingTurn
        {
            get { return _isTakingTurn; }
            set
            {
                _isTakingTurn = value;
                OnPropertyChanged();
            }
        }

        public Player(string name, Card representative)
        {
            Name = name;
            Representative = representative;
        }

        internal void PutCardInHand(Card card)
        {
            if (!Hand.Contains(card))
            {
                Hand = new ReadOnlyCollection<Card>(Hand.Concat(new[] {card}).ToList());
            }
        }

        public override string ToString()
        {
            return $"{Name} ({Representative.Name})";
        }
    }
}
