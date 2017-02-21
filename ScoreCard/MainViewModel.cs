using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ClueHelper;
using ClueHelper.Models;

namespace ScoreCard
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

        public MainViewModel(Solver solver)
        {
            Solver = solver;
            StartSuggestion = new RelayCommand<Player>(DoStartSuggestion, CanStartSuggestion);
            MakeSuggestion = new RelayCommand(DoMakeSuggestion, CanMakeSuggestion);
            SuggestCard = new RelayCommand<Card>(ToggleCardInSuggestion, CanToggleCardInSuggestion);
        }

        private void DoMakeSuggestion()
        {
            if (!CanMakeSuggestion())
            {
                return;
            }

            _state = State.WaitingForResults;

            // todo, don't do this in this odd way of throwing a dialog from the viewmodel
            var currentTurnIndex = Solver.Game.Players.Select((player, i) => player.IsTakingTurn ? i : -1).Max();
            var playersToAsk = Solver.Game.Players.Skip(currentTurnIndex + 1)
                .Concat(Solver.Game.Players.Take(currentTurnIndex))
                .ToList();

            var stopped = false;

            foreach (var player in playersToAsk)
            {
                var keepGoing = true;
                var valid = false;
                while (!valid)
                {
                    try
                    {
                        keepGoing = GetSuggestionResponse(player, ref stopped);
                        valid = true;
                    }
                    catch (GameException e)
                    {
                        valid = MessageBoxResult.Yes == MessageBox.Show(e.Message, "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                        keepGoing = false;
                    }
                }

                if (!keepGoing)
                {
                    break;
                }
            }

            if (!stopped)
            {
                // todo, make inferences based on nobody having any of the cards
                // todo, either the suggestor has some of them or they're part of the solution
            }

            foreach (var card in _selectedCards)
            {
                card.IsPartOfSuggestion = false;
            }
            _selectedCards.Clear();
            Solver.Game.NextTurn();
            _state = State.None;
        }

        private bool GetSuggestionResponse(Player player, ref bool stopped)
        {
            if (ReferenceEquals(player, Solver.MyPlayer))
            {
                MessageBox.Show("Hit OK when done.", "Hit OK when done.", MessageBoxButton.OK);
                return !player.Hand.Intersect(_selectedCards).Any();
            }
            var vm = new DialogViewModel(player, Solver.MyPlayer.IsTakingTurn ? _selectedCards : new Card[0]);
            new SuggestionResponseDialog(vm).ShowDialog();

            switch (vm.Result)
            {
                case DialogResult.Cancel:
                    return false;
                case DialogResult.Skip:
                    return true;
                case DialogResult.Maybe:
                    Solver.PlayerMightHaveCards(player, _selectedCards);
                    stopped = true;
                    return false;
                case DialogResult.None:
                    Solver.PlayerDoesNotHaveCards(player, _selectedCards);
                    return true;
                case DialogResult.Card:
                    Solver.PlayerHasCard(player, vm.ResultCard);
                    stopped = true;
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool CanMakeSuggestion()
        {
            return State.BuildingSuggestion == _state &&
                _selectedCards.Count == Config.CardsPerSuggestion &&
                _selectedCards.Select(card => card.Category).Distinct().Count() == Config.CardsPerSuggestion;
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
            _state = State.BuildingSuggestion;
        }

        private bool CanStartSuggestion(Player player)
        {
            return State.None == _state && player.IsTakingTurn;
        }
    }
}