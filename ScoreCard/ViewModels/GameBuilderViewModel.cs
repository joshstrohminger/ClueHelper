using System.Collections.ObjectModel;
using System.Windows.Input;
using ClueHelper.Models;
using ScoreCard.Interfaces;
using ScoreCard.Models;
using ScoreCard.MVVM;
using ObservableObject = ScoreCard.MVVM.ObservableObject;

namespace ScoreCard.ViewModels
{
    public class GameBuilderViewModel : ObservableObject, IGameBuilderViewModel
    {
        public ICommand AddPlayer { get; }
        public RelayCommand<PotentialPlayer> RemovePlayer { get; }
        public ObservableCollection<PotentialPlayer> Players { get; }
        public void Build()
        {
            throw new System.NotImplementedException();
        }
        public Game GameResult { get; private set; }

        public ReadOnlyCollection<string> Pieces { get; }
    }
}
