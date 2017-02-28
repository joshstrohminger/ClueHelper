using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ScoreCard;
using ScoreCard.Interfaces;
using ScoreCard.Models;
using ScoreCard.ViewModels;

namespace ScoreCardTest
{
    public class Simulator
    {
        private readonly Solver _solver;
        private readonly Random _random;
        private readonly MainViewModel _vm;
        private int _currentTurnIndex;
        public Card[] Solution { get; }
        public Dictionary<Player,Card[]> PlayerHands { get; }

        public Simulator()
        {
            var game = Config.BuildDefaultGame();
            var me = game.Players.Last();
            _solver = new Solver(game, me);

            _random = new Random();
            var shuffledCards = game.Categories
                .SelectMany(category => category.Cards)
                .OrderBy(card => _random.Next())
                .ToList();

            Solution = shuffledCards.Take(game.CardsPerSuggestion).ToArray();
            for (var i = 0; i < game.CardsPerSuggestion; i++)
            {
                shuffledCards.RemoveAt(0);
            }

            var numPlayers = game.Players.Count;
            PlayerHands = game.Players.ToDictionary(p => p, (player) =>
            {
                var index = game.Players.IndexOf(player);
                return shuffledCards.Where((c, i) => i % numPlayers == index).ToArray();
            });

            foreach (var card in PlayerHands[me])
            {
                _solver.PlayerHasCard(me, card, "Card was dealt.");
            }

            _vm = new MainViewModel(_solver);
            _vm.PromptForSuggestionResult += OnPromptForSuggestionResult;
            _vm.PromptForSimpleResponse += OnPromptForSimpleResponse;
        }

        public bool IsSolutionKnownByMe => Solution.All(c => c.IsPartOfAccusation);

        public int Run(int maxTurns = int.MaxValue)
        {
            for (var turn = 0; turn < maxTurns; turn++)
            {
                RunSingleTurn(turn);
                if (IsSolutionKnownByMe)
                {
                    return turn + 1;
                }       
            }
            return maxTurns;
        }

        private void RunSingleTurn(int turn)
        {
            if (_currentTurnIndex >= _solver.Game.Players.Count)
            {
                _currentTurnIndex = 0;
            }

            var player = _solver.Game.Players[_currentTurnIndex];
            TakeTurnFor(player, turn);

            _currentTurnIndex++;
        }

        private void TakeTurnFor(Player player, int turn)
        {
            MakeRandomSuggestion(player, turn);
        }

        private void OnPromptForSimpleResponse(object sender, SimplePrompt e)
        {
            switch (e.Button)
            {
                case MessageBoxButton.OK:
                case MessageBoxButton.OKCancel:
                    e.Result = MessageBoxResult.OK;
                    break;
                case MessageBoxButton.YesNoCancel:
                case MessageBoxButton.YesNo:
                    e.Result = MessageBoxResult.Yes;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnPromptForSuggestionResult(object sender, ISuggestionResponseViewModel suggestion)
        {
            var responder = suggestion.Responder;
            var responderCards = suggestion.Cards.Intersect(PlayerHands[responder]).ToArray();
            if(suggestion.Asker == _solver.MyPlayer)
            {
                // I am asking another player
                if (responderCards.Length > 0)
                {
                    suggestion.ResultCard = responderCards[0];
                    suggestion.Result = DialogResult.Card;
                }
                else
                {
                    suggestion.Result = DialogResult.None;
                }
            }
            else
            {
                // Another player is asking another player
                suggestion.Result = responderCards.Length > 0 ? DialogResult.Maybe : DialogResult.None;
            }
        }

        private void MakeRandomSuggestion(Player player, int turn)
        {
            var cards = _solver.Game.Categories
                .Select(cat => cat.Cards[_random.Next(cat.Cards.Length)])
                .ToArray();

            _vm.StartSuggestion.Execute(player);
            foreach (var card in cards)
            {
                _vm.SuggestCard.Execute(card);
            }
            
            _vm.MakeSuggestion.Execute(null);
            if (_vm.State != State.None)
            {
                throw new Exception($"VM is in state {_vm.State} after making suggestions in turn {turn}.");
            }
        }
    }
}
