using System;
using System.Linq;
using ClueHelper;
using ScoreCard.Interfaces;
using ScoreCard.ViewModels;

namespace ScoreCard.Views
{
    public partial class MainWindow
    {
        private readonly IMainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = UseDefaultGame();
            DataContext = _vm;
        }

        private static IMainViewModel UseDefaultGame()
        {
            var game = Config.BuildDefaultGame();
            var me = game.Players.Last();
            var solver = new Solver(game, me);

            var random = new Random();

            var shuffledCards = game.Categories
                .SelectMany(category => category.Cards)
                .OrderBy(card => random.Next())
                .Skip(game.CardsPerSuggestion) // the answer
                .ToArray();

            var cardsPerHand = shuffledCards.Length / game.Players.Count;

            foreach (var card in shuffledCards.Take(cardsPerHand))
            {
                solver.PlayerHasCard(me, card);
            }

            return new MainViewModel(solver);
        }
    }
}