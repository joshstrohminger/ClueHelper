using System;
using System.Linq;
using ClueHelper;
using ClueHelper.Models;

namespace ScoreCard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IMainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();

            var game = Config.BuildDefaultGame();
            var me = game.Players.Last();
            var solver = new Solver(game, me);

            _vm = new MainViewModel(solver);
            DataContext = _vm;

            var random = new Random();

            var shuffledCards = game.Categories
                .SelectMany(category => category.Cards)
                .OrderBy(card => random.Next())
                .Skip(Config.CardsPerSuggestion) // the answer
                .ToArray();

            var cardsPerHand = shuffledCards.Length / game.Players.Count;

            foreach (var card in shuffledCards.Take(cardsPerHand))
            {
                solver.PlayerHasCard(me, card);
            }

            solver.PlayerHasCard(game.Players.First(), shuffledCards.Skip(cardsPerHand).First());
            solver.PlayerMightHaveCards(game.Players.Skip(1).First(), shuffledCards.Skip(cardsPerHand + 1).Take(Config.CardsPerSuggestion));
            solver.PlayerDoesNotHaveCards(game.Players.Skip(2).First(), shuffledCards.Skip(cardsPerHand + Config.CardsPerSuggestion + 1).Take(Config.CardsPerSuggestion));
        }
    }
}