using System;
using ScoreCard.MVVM;

namespace ScoreCard.Models
{
    public class PlayerPossibility : ObservableObject
    {
        private Possibility _possibility;

        public Possibility Possibility
        {
            get { return _possibility;}
            set
            {
                _possibility = value;
                OnPropertyChanged();
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
