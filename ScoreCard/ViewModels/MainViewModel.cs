using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ScoreCard.Interfaces;
using ScoreCard.Models;
using ScoreCard.MVVM;

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
        #region Fields
        
        private readonly IList<Card> _selectedCards = new List<Card>();
        private readonly SuggestionManager _suggestionManager;

        #endregion Fields

        #region Properties

        public State State { get; private set; }
        public Solver Solver { get; }
        public RelayCommand<Player> StartSuggestion { get; }
        public ICommand MakeSuggestion { get; }
        public RelayCommand<Card> SuggestCard { get; }
        public event EventHandler<ISuggestionResponseViewModel> PromptForSuggestionResult
        {
            add { _suggestionManager.PromptForSuggestionResult += value; }
            remove { _suggestionManager.PromptForSuggestionResult -= value; }
        }
        public event EventHandler<SimplePrompt> PromptForSimpleResponse
        {
            add { _suggestionManager.PromptForSimpleResponse += value; }
            remove { _suggestionManager.PromptForSimpleResponse -= value; }
        }

        #endregion Properties

        #region Public

        public MainViewModel(Solver solver)
        {
            Solver = solver;
            _suggestionManager = new SuggestionManager(solver);
            StartSuggestion = new RelayCommand<Player>(DoStartSuggestion, CanStartSuggestion);
            MakeSuggestion = new RelayCommand(DoMakeSuggestion, CanMakeSuggestion);
            SuggestCard = new RelayCommand<Card>(ToggleCardInSuggestion, CanToggleCardInSuggestion);
        }

        #endregion Public

        #region Private

        private void DoMakeSuggestion()
        {
            if (!CanMakeSuggestion())
            {
                return;
            }

            State = State.WaitingForResults;
            
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
            ClearSuggestion(_suggestionManager.PlayerTakingTurn);
        }

        private void ClearSuggestion(Player player)
        {
            foreach (var card in _selectedCards)
            {
                card.IsPartOfSuggestion = false;
            }
            _selectedCards.Clear();
            player.IsTakingTurn = false;
            State = State.None;
        }

        private bool CanMakeSuggestion()
        {
            return State.BuildingSuggestion == State &&
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
            return State.BuildingSuggestion == State &&
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
                State = State.BuildingSuggestion;
                player.IsTakingTurn = true;
            }
        }

        private bool CanStartSuggestion(Player player)
        {
            return State.None == State || player.IsTakingTurn;
        }

        #endregion Private
    }
}