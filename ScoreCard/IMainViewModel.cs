using System.Windows.Input;
using ClueHelper;
using ClueHelper.Models;

namespace ScoreCard
{
    public interface IMainViewModel
    {
        Solver Solver { get; }

        RelayCommand<Player> StartSuggestion { get; }
        ICommand MakeSuggestion { get; }
        RelayCommand<Card> SuggestCard { get; }
    }
}
