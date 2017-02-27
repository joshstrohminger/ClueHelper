using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ScoreCard.Models;
using ScoreCard.MVVM;

namespace ScoreCard.Interfaces
{
    public interface IGameBuilderViewModel
    {
        ICommand AddPlayer { get; }
        RelayCommand<PotentialPlayer> RemovePlayer { get; }
        ObservableCollection<PotentialPlayer> Players { get; }
        ICommand Build { get; }
        ReadOnlyCollection<Card> Pieces { get; }
        Game GameResult { get; }
        event EventHandler GameBuilt;
    }
}
