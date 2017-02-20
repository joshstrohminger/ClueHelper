using System;
using System.Linq;
using ClueHelper;
using ClueHelper.Models;

namespace ScoreCard
{
    public class MainViewModelDesignData : IMainViewModel
    {
        public Solver Solver { get; }

        public MainViewModelDesignData()
        {
            var game = Config.BuildDefaultGame();
            var me = game.Players.Last();

            var random = new Random();

            Solver = new Solver(game, me);

            var shuffledCards = game.Categories
                .SelectMany(category => category.Cards)
                .OrderBy(card => random.Next())
                .ToArray();

            foreach (var card in shuffledCards.Take(Config.CardsPerPlayer))
            {
                Solver.PlayerHasCard(me, card);
            }

            Solver.PlayerHasCard(game.Players.First(), shuffledCards.Skip(Config.CardsPerPlayer).First());
            Solver.PlayerMightHaveCards(game.Players.Skip(1).First(), shuffledCards.Skip(Config.CardsPerPlayer+1).Take(Config.CardsPerPlayer));
            Solver.PlayerDoesNotHaveCards(game.Players.Skip(2).First(), shuffledCards.Skip(Config.CardsPerPlayer*2+1).Take(Config.CardsPerPlayer));
        }
    }
}
