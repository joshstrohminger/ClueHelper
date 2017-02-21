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
                .Skip(game.CardsPerSuggestion) // the answer
                .ToArray();

            var cardsPerHand = shuffledCards.Length / game.Players.Count;

            foreach (var card in shuffledCards.Take(cardsPerHand))
            {
                solver.PlayerHasCard(me, card);
            }
        }
    }
}