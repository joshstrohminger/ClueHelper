using System;
using System.Linq;
using System.Windows;
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
                .ToArray();

            foreach (var card in shuffledCards.Take(Config.CardsPerPlayer))
            {
                solver.PlayerHasCard(me, card);
            }

            solver.PlayerHasCard(game.Players.First(), shuffledCards.Skip(Config.CardsPerPlayer).First());
            solver.PlayerMightHaveCards(game.Players.Skip(1).First(), shuffledCards.Skip(Config.CardsPerPlayer + 1).Take(Config.CardsPerPlayer));
            solver.PlayerDoesNotHaveCards(game.Players.Skip(2).First(), shuffledCards.Skip(Config.CardsPerPlayer * 2 + 1).Take(Config.CardsPerPlayer));
        }
    }
}