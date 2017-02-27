using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ScoreCard.Interfaces;
using ScoreCard.Models;
using ScoreCard.MVVM;
using ObservableObject = ScoreCard.MVVM.ObservableObject;

namespace ScoreCard.ViewModels
{
    public enum State
    {
        None,
        BuildingSuggestion,
        WaitingForResults
    }

    public class MainViewModel : ObservableObject, IMainViewModel
    {

        private State _state;
        private readonly IList<Card> _selectedCards = new List<Card>();

        #region Properties

        public Solver Solver { get; }
        public RelayCommand<Player> StartSuggestion { get; }
        public ICommand MakeSuggestion { get; }
        public RelayCommand<Card> SuggestCard { get; }

        #endregion Properties

        private readonly SuggestionManager _suggestionManager;

        public event EventHandler<ISuggestionResponseViewModel> PromptForSuggestionResult
        {
            add { _suggestionManager.PromptForSuggestionResult += value; }
            remove { _suggestionManager.PromptForSuggestionResult -= value; }
        }

        public MainViewModel(Solver solver)
        {
            Solver = solver;
            _suggestionManager = new SuggestionManager(solver);
            StartSuggestion = new RelayCommand<Player>(DoStartSuggestion, CanStartSuggestion);
            MakeSuggestion = new RelayCommand(DoMakeSuggestion, CanMakeSuggestion);
            SuggestCard = new RelayCommand<Card>(ToggleCardInSuggestion, CanToggleCardInSuggestion);
        }

        public void ProvideSuggestionResult(ISuggestionResponseViewModel vm)
        {
            if (_state != State.WaitingForResults)
            {
                throw new GameException($"Tried to provide suggestion result in state {_state}");
            }

            _suggestionManager.ProvideSuggestionResult(vm);
            if (_suggestionManager.IsDoneAsking)
            {
                ClearSuggestion(_suggestionManager.PlayerTakingTurn);
            }
        }

        private void DoMakeSuggestion()
        {
            if (!CanMakeSuggestion())
            {
                return;
            }

            _state = State.WaitingForResults;

            // todo, don't do this in this odd way of throwing a dialog from the viewmodel
            var playerTakingTurn = Solver.Game.Players.First(player => player.IsTakingTurn);
            var currentTurnIndex = Solver.Game.Players.IndexOf(playerTakingTurn);
            if (currentTurnIndex < 0)
            {
                throw new GameException("No player is taking a turn.");
            }
            var playersToAsk = Solver.Game.Players.Skip(currentTurnIndex + 1)
                .Concat(Solver.Game.Players.Take(currentTurnIndex))
                .ToList();

            _suggestionManager.AskPlayers(playerTakingTurn, playersToAsk, _selectedCards);
            if (_suggestionManager.IsDoneAsking)
            {
                ClearSuggestion(_suggestionManager.PlayerTakingTurn);
            }
        }

        private void ClearSuggestion(Player player)
        {
            foreach (var card in _selectedCards)
            {
                card.IsPartOfSuggestion = false;
            }
            _selectedCards.Clear();
            player.IsTakingTurn = false;
            _state = State.None;
        }

        private bool CanMakeSuggestion()
        {
            return State.BuildingSuggestion == _state &&
                _selectedCards.Count == Solver.Game.CardsPerSuggestion &&
                _selectedCards.Select(card => card.Category).Distinct().Count() == Solver.Game.CardsPerSuggestion;
        }

        private void ToggleCardInSuggestion(Card card)
        {
            if (!CanToggleCardInSuggestion(card))
            {
                return;
            }

            if (card.IsPartOfSuggestion)
            {
                _selectedCards.Remove(card);
            }
            else
            {
                _selectedCards.Add(card);
            }
            card.IsPartOfSuggestion ^= true;
        }

        private bool CanToggleCardInSuggestion(Card card)
        {
            return State.BuildingSuggestion == _state &&
                (card.IsPartOfSuggestion || 
                _selectedCards.All(selected => selected.Category != card.Category));
        }

        private void DoStartSuggestion(Player player)
        {
            if (!CanStartSuggestion(player))
            {
                return;
            }
            if (player.IsTakingTurn)
            {
                ClearSuggestion(player);
            }
            else
            {
                _state = State.BuildingSuggestion;
                player.IsTakingTurn = true;
            }
        }

        private bool CanStartSuggestion(Player player)
        {
            return State.None == _state || player.IsTakingTurn;
        }
    }
}