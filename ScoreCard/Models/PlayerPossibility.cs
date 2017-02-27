using System;
using ScoreCard.MVVM;

namespace ScoreCard.Models
{
    public class PlayerPossibility : ObservableObject
    {
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
            set
            {
                if (value != _possibility)
                {
                    _possibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public Player Player { get; }

        public PlayerPossibility(Player player)
        {
            if (null == player)
            {
                throw new ArgumentNullException(nameof(player));
            }
            Player = player;
        }
    }
}
