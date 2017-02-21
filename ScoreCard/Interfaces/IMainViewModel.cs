using System.Windows.Input;
using ClueHelper;
using ClueHelper.Models;
using ScoreCard.MVVM;

namespace ScoreCard.Interfaces
{
    public interface IMainViewModel
    {
        Solver Solver { get; }

        RelayCommand<Player> StartSuggestion { get; }
        ICommand MakeSuggestion { get; }
        RelayCommand<Card> SuggestCard { get; }
    }
}
