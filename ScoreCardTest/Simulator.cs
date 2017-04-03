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

            Solution = game.Categories.Select(cat => shuffledCards.First(c => c.Category == cat)).ToArray();
            foreach (var card in Solution)
            {
                shuffledCards.Remove(card);
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
            MakeCalculatedSuggestion(player, turn);
            //MakeRandomSuggestion(player, turn);
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

        private void MakeCalculatedSuggestion(Player player, int turn)
        {
            var selectedCards = _solver.Game.Categories
                .Select(c => c.Cards)
                .Select(cards =>
                    cards.FirstOrDefault(c => c.IsPartOfAccusation).ToReplaceableCard(false)
                    ?? cards.FirstOrDefault(c => c.Possibilities.All(p => p.Possibility == Possibility.Unknown)).ToReplaceableCard(false)
                    ?? cards.FirstOrDefault(c => c.Possibilities.Any(p => p.Possibility == Possibility.Unknown) && c.Possibilities.All(p => p.Possibility != Possibility.Maybe)).ToReplaceableCard(false)
                    ?? cards.FirstOrDefault(c => c.Possibilities.Any(p => p.Possibility <= Possibility.Maybe)).ToReplaceableCard(true)
                    ?? cards.First().ToReplaceableCard(true)
                ).ToArray();

            // Replace all potentially unhelpful cards in the suggestion with cards the player already has.
            // This will make the remaining "unreplaced" cards more likely to provide helpful results.
            var maxReplaceable = Math.Min(selectedCards.Count(c => c.Replaceable) - 1, _solver.Game.CardsPerSuggestion - 1);
            foreach(var card in selectedCards)
            {
                if (selectedCards.Count(c => c.IsReplaced) >= maxReplaceable)
                {
                    break;
                }
                card.Replace(PlayerHands[player].FirstOrDefault(h => h.Category == card.Original.Category));
            }

            MakeSuggestion(player, turn, selectedCards.Select(c => c.Replacement).ToArray());
        }

        //private void MakeRandomSuggestion(Player player, int turn)
        //{
        //    var cards = _solver.Game.Categories
        //        .Select(cat => cat.Cards[_random.Next(cat.Cards.Length)])
        //        .ToArray();
        //    MakeSuggestion(player, turn, cards);
        //}

        private void MakeSuggestion(Player player, int turn, Card[] cards)
        {
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

    public class ReplaceableCard
    {
        private Card _replacement;

        public Card Original { get; }
        public bool Replaceable { get; }

        public Card Replacement => _replacement ?? Original;
        public bool IsReplaced => _replacement != null;

        public ReplaceableCard(Card card, bool replaceable)
        {
            Original = card;
            Replaceable = replaceable;
        }

        public void Replace(Card card)
        {
            if (Replaceable)
            {
                _replacement = card;
            }
        }

        public override string ToString() => $"{Replaceable}, {Original}{(IsReplaced ? $" => {Replacement}" : "")}";
    }

    public static class SimExtensions
    {
        public static ReplaceableCard ToReplaceableCard(this Card card, bool replaceable)
        {
            if (card is null)
            {
                return null;
            }
            return new ReplaceableCard(card, replaceable);
        }
    }
}
