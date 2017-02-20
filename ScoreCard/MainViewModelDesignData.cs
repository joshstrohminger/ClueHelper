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

            var shuffledCards = game.Categories
                .SelectMany(category => category.Cards)
                .OrderBy(card => random.Next())
                .ToArray();

            foreach(var card in shuffledCards.Take(Config.CardsPerPlayer))
            {
                Solver.PlayerHasCard(me, card);
            }

            Solver = new Solver(game, me);
        }
    }
}
