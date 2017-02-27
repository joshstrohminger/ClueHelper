using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScoreCard;
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
        }

        public bool IsSolutionKnownByMe => Solution.All(c => c.IsPartOfAccusation);

        public void Run(int maxTurns = int.MaxValue)
        {
            for (var turn = 0; turn < maxTurns; turn++)
            {
                RunSingleTurn();
                if (IsSolutionKnownByMe)
                {
                    break;
                }       
            }
        }

        private void RunSingleTurn()
        {
            if (_currentTurnIndex > _solver.Game.Players.Count)
            {
                _currentTurnIndex = 0;
            }

            var player = _solver.Game.Players[_currentTurnIndex];
            TakeTurnFor(player);

            _currentTurnIndex++;
        }

        private void TakeTurnFor(Player player)
        {
            MakeRandomSuggestion(player);
            // todo need to figure out how to handle the events to get suggestion responses and message boxes
        }

        private void AnswerFor(Player player)
        {
            
        }

        private void AnswerForMe()
        {
            
        }

        private void MakeRandomSuggestion(Player player)
        {
            var cards = _solver.Game.Categories
                .Select(cat => cat.Cards[_random.Next(cat.Cards.Length)])
                .ToArray();

            _vm.StartSuggestion.Execute(player);
            foreach (var card in cards)
            {
                card.IsPartOfSuggestion = true;
            }
            _vm.MakeSuggestion.Execute(null);
        }
    }
}
