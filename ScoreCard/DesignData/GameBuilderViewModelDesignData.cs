using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ClueHelper.Models;
using ScoreCard.Interfaces;
using ScoreCard.Models;
using ScoreCard.MVVM;

namespace ScoreCard.DesignData
{
    public class GameBuilderViewModelDesignData : IGameBuilderViewModel
    {
        public ICommand AddPlayer { get; } = new RelayCommand(() => {});
        public RelayCommand<PotentialPlayer> RemovePlayer { get; } = new RelayCommand<PotentialPlayer>(o => {});
        public ObservableCollection<PotentialPlayer> Players { get; } = new ObservableCollection<PotentialPlayer>();
        public ReadOnlyCollection<string> Pieces { get; }
        public Game GameResult { get; }
        public void Build()
        {
            throw new System.NotImplementedException();
        }

        public GameBuilderViewModelDesignData()
        {
            Pieces = new ReadOnlyCollection<string>(new List<string> {"That One", "This One"});

            Players.Add(new PotentialPlayer {Name = "Josh"});
            Players.Add(new PotentialPlayer { Name = "Frank", Piece = Pieces.First()});
        }
    }
}
