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
        private readonly Solver _solver;
        private ICollection<Card> _selectedCards;

        public SuggestionManager(Solver solver)
        {
            _solver = solver;
        }

        public event EventHandler<ISuggestionResponseViewModel> PromptForSuggestionResult;
        public event EventHandler<SimplePrompt> PromptForSimpleResponse;


        public Player PlayerTakingTurn { get; private set; }

        public void AskPlayers(Player playerTakingTurn, ICollection<Player> playersToAsk, ICollection<Card> selectedCards)
        {
            _selectedCards = selectedCards;
            PlayerTakingTurn = playerTakingTurn;

            _solver.Changes.Add(new SuggestionMade(DateTime.Now, playerTakingTurn, selectedCards));

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
                        var prompt = new SimplePrompt(e.Message, "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                        PromptForSimpleResponse?.Invoke(this, prompt);
                        valid = MessageBoxResult.Yes == prompt.Result;
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
                _solver.SuggestionLooped(playerTakingTurn, _selectedCards);
            }
        }

        private bool GetSuggestionResponse(Player player, ref bool stopped)
        {
            if (ReferenceEquals(player, _solver.MyPlayer))
            {
                var prompt = new SimplePrompt("Hit OK when done.", "Hit OK when done.", MessageBoxButton.OK, MessageBoxImage.None);
                PromptForSimpleResponse?.Invoke(this, prompt);
                if (player.Hand.Intersect(_selectedCards).Any())
                {
                    _solver.Changes.Add(new SuggestionResponse(DateTime.Now, player, "?"));
                    stopped = true;
                    return false;
                }
                _solver.Changes.Add(new SuggestionResponse(DateTime.Now, player, null));
                return true;
            }

            _currentAsk = new SuggestionResponseViewModel(PlayerTakingTurn, player, _solver.MyPlayer.IsTakingTurn, _selectedCards);
            PromptForSuggestionResult?.Invoke(this, _currentAsk);
            var addedChange = false;
            try
            {

                switch (_currentAsk.Result)
                {
                    case DialogResult.Cancel:
                        return false;
                    case DialogResult.Skip:
                        return true;
                    case DialogResult.Maybe:
                        _solver.Changes.Add(new SuggestionResponse(DateTime.Now, player, "?"));
                        addedChange = true;
                        _solver.PlayerMightHaveCards(player, _selectedCards,
                            $"{player.Name} showed a card to {PlayerTakingTurn.Name}.");
                        stopped = true;
                        return false;
                    case DialogResult.None:
                        _solver.Changes.Add(new SuggestionResponse(DateTime.Now, player, null));
                        addedChange = true;
                        _solver.PlayerDoesNotHaveCards(player, _selectedCards,
                            $"{player.Name} said they didn't have this card when asked.");
                        return true;
                    case DialogResult.Card:
                        _solver.Changes.Add(new SuggestionResponse(DateTime.Now, player, _currentAsk.ResultCard.Name));
                        addedChange = true;
                        _solver.PlayerHasCard(player, _currentAsk.ResultCard, $"{player.Name} showed me {_currentAsk.ResultCard}.");
                        stopped = true;
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (GameException)
            {
                if (addedChange)
                {
                    _solver.Changes.RemoveAt(_solver.Changes.Count-1);
                }
                throw;
            }
        }
    }
}
