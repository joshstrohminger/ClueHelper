using ClueHelper.Models;
using ObservableObject = ScoreCard.MVVM.ObservableObject;

namespace ScoreCard.Models
{
    public class PotentialPlayer : ObservableObject
    {
        private string _name;
        private Card _piece;
        private bool _me;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public Card Piece
        {
            get { return _piece; }
            set
            {
                _piece = value;
                OnPropertyChanged();
            }
        }

        public bool Me
        {
            get { return _me; }
            set
            {
                _me = value;
                OnPropertyChanged();
            }
        }
    }
}
