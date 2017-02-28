using System;
using System.Windows.Input;
using ScoreCard.Models;
using ScoreCard.MVVM;
using ScoreCard.ViewModels;

namespace ScoreCard.Interfaces
{
    public interface IMainViewModel
    {
        Solver Solver { get; }
        RelayCommand<Player> StartSuggestion { get; }
        ICommand MakeSuggestion { get; }
        RelayCommand<Card> SuggestCard { get; }
        event EventHandler<ISuggestionResponseViewModel> PromptForSuggestionResult;
        event EventHandler<SimplePrompt> PromptForSimpleResponse;
        State State { get; }
    }
}
