using System;
using System.Collections.ObjectModel;
using ScoreCard.Interfaces;
using ScoreCard.MVVM;

namespace ScoreCard.Models
{
    public class PlayerPossibility : ObservableObject
    {
        private readonly ObservableCollection<IGameChange> _changes;
        private Possibility _possibility;
        private bool _isHighlighted;

        public bool IsHighlighted
        {
            get { return _isHighlighted; }
            set
            {
                _isHighlighted = value;
                OnPropertyChanged();
            }
        }

        public Possibility Possibility
        {
            get { return _possibility;}
            private set
            {
                _possibility = value;
                OnPropertyChanged();
            }
        }

        public void UpdatePossibility(Possibility possibility, string reason, Player asking, Player answering)
        {
            if (Possibility == possibility)
            {
                return;
            }

            _changes.Add(new PossibilityChange(this, Possibility, possibility, reason, asking, answering, DateTime.Now));
            Possibility = possibility;
        }

        public Player Player { get; }
        public Card Card { get; }

        public PlayerPossibility(Player player, Card card, ObservableCollection<IGameChange> changes)
        {
            if (null == player)
            {
                throw new ArgumentNullException(nameof(player));
            }
            _changes = changes;
            Player = player;
            Card = card;
        }
    }
}
