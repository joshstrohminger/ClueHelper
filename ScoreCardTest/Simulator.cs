using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScoreCard;
using ScoreCard.Models;

namespace ScoreCardTest
{
    public class Simulator
    {
        private Solver _solver;
        public Card[] Solution { get; }
        public Dictionary<Player,Card[]> PlayerHands { get; }

        public Simulator()
        {
            var game = Config.BuildDefaultGame();
            var me = game.Players.Last();
            _solver = new Solver(game, me);

            var random = new Random();
            var shuffledCards = game.Categories
                .SelectMany(category => category.Cards)
                .OrderBy(card => random.Next())
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
        }
    }
}
