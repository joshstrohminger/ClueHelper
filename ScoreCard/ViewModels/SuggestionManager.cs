using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ScoreCard.Interfaces;
using ScoreCard.Models;

namespace ScoreCard.ViewModels
{
    public class SuggestionManager
    {
        private ISuggestionResponseViewModel _currentAsk;
        private bool _stopped;
        private Queue<Player> _playersToAsk;
        private readonly Solver _solver;
        private ICollection<Card> _selectedCards;

        public SuggestionManager(Solver solver)
        {
            _solver = solver;
        }

        public event EventHandler<ISuggestionResponseViewModel> PromptForSuggestionResult;

        public bool IsDoneAsking => (_playersToAsk?.Count ?? 0) == 0;
        public Player PlayerTakingTurn { get; private set; }

        public void ProvideSuggestionResult(ISuggestionResponseViewModel vm)
        {
            if (!ReferenceEquals(vm, _currentAsk))
            {
                throw new GameException("Response was provided for a different suggestion");
            }

            var keepGoing = false;
            var valid = false;
            try
            {
                keepGoing = ProcessResult();
                valid = true;
            }
            catch (GameException e)
            {
                // todo, this needs to be rethought since we need to run headless sometimes
                valid = MessageBoxResult.Yes == MessageBox.Show(e.Message, "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                keepGoing = false;
            }

            CheckIfLoopShouldContinue(valid, keepGoing);
        }

        private void CheckIfLoopShouldContinue(bool valid, bool keepGoing)
        {
            if (!valid)
            {
                AskCurrentPlayer();
                return;
            }

            if (keepGoing)
            {
                _playersToAsk.Dequeue();
                if (!IsDoneAsking)
                {
                    AskCurrentPlayer();
                }
            }
            else
            {
                _playersToAsk.Clear();
            }

            if (IsDoneAsking && !_stopped)
            {
                _solver.SuggestionLooped(PlayerTakingTurn, _selectedCards);
            }
        }

        public void AskPlayers(Player playerTakingTurn, ICollection<Player> playersToAsk, ICollection<Card> selectedCards)
        {
            if (!IsDoneAsking)
            {
                throw new GameException("Already asking players.");
            }
            
            _playersToAsk = new Queue<Player>(playersToAsk);
            _selectedCards = selectedCards;
            PlayerTakingTurn = playerTakingTurn;
            _stopped = false;
            AskCurrentPlayer();
        }

        private void AskCurrentPlayer()
        {
            var player = _playersToAsk.Peek();
            if (ReferenceEquals(player, _solver.MyPlayer))
            {
                var keepGoing = true;
                // todo, this needs to be rethought since we need to run headless sometimes
                MessageBox.Show("Hit OK when done.", "Hit OK when done.", MessageBoxButton.OK);
                if (player.Hand.Intersect(_selectedCards).Any())
                {
                    _stopped = true;
                    keepGoing = false;
                }
                CheckIfLoopShouldContinue(true, keepGoing);
                return;
            }
            _currentAsk = new SuggestionResponseViewModel(player, _solver.MyPlayer.IsTakingTurn ? _selectedCards : new Card[0]);
            PromptForSuggestionResult?.Invoke(this, _currentAsk);
        }

        private bool ProcessResult()
        {
            switch (_currentAsk.Result)
            {
                case DialogResult.Cancel:
                    return false;
                case DialogResult.Skip:
                    return true;
                case DialogResult.Maybe:
                    _solver.PlayerMightHaveCards(_playersToAsk.Peek(), _selectedCards, $"{_playersToAsk.Peek().Name} showed a card to {PlayerTakingTurn.Name}.");
                    _stopped = true;
                    return false;
                case DialogResult.None:
                    _solver.PlayerDoesNotHaveCards(_playersToAsk.Peek(), _selectedCards, $"{_playersToAsk.Peek().Name} said they didn't have this card when asked.");
                    return true;
                case DialogResult.Card:
                    _solver.PlayerHasCard(_playersToAsk.Peek(), _currentAsk.ResultCard, $"{_playersToAsk.Peek().Name} showed me {_currentAsk.ResultCard}.");
                    _stopped = true;
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
