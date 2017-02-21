using System;
using System.Collections.Generic;
using System.Linq;
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
                .Where(player => !ReferenceEquals(player, Solver.MyPlayer))
                .ToList();

            foreach (var player in playersToAsk)
            {
                var vm = new DialogViewModel(player, _selectedCards);
                new SuggestionResponseDialog(vm).ShowDialog();

                if (vm.Result == DialogResult.Card)
                {
                    Solver.PlayerHasCard(player, vm.ResultCard);
                    break;
                }

                switch (vm.Result)
                {
                    case DialogResult.Skip:
                        break;
                    case DialogResult.Maybe:
                        Solver.PlayerMightHaveCards(player, vm.Cards);
                        break;
                    case DialogResult.None:
                        Solver.PlayerDoesNotHaveCards(player, vm.Cards);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            Solver.Game.NextTurn();
            _state = State.None;
        }

        private bool CanMakeSuggestion()
        {
            return State.BuildingSuggestion == _state && _selectedCards.Count == Config.CardsPerSuggestion && _selectedCards.Select(card => card.Category).Distinct().Count() == Config.CardsPerSuggestion;
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
            return State.BuildingSuggestion == _state;
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
            return State.None == _state;
        }
    }
}