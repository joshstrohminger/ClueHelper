using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ClueHelper.Models;
using ScoreCard.Models;
using ScoreCard.MVVM;

namespace ScoreCard.Interfaces
{
    public interface IGameBuilderViewModel
    {
        ICommand AddPlayer { get; }
        RelayCommand<PotentialPlayer> RemovePlayer { get; }
        ObservableCollection<PotentialPlayer> Players { get; }
        void Build();
        ReadOnlyCollection<string> Pieces { get; }
        Game GameResult { get; }
    }
}
