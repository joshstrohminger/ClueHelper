using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
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

            var result = MessageBox.Show("Do you want play the test game?", "Play Test Game?", MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            _vm = result == MessageBoxResult.Yes ? UseDefaultGame() : BuildGame();

            DataContext = _vm;
            _vm.PromptForSuggestionResult += PromptForSuggestionResult;
        }

        private void PromptForSuggestionResult(object sender, IDialogViewModel dialogViewModel)
        {
            new SuggestionResponseDialog(dialogViewModel) {Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner}.ShowDialog();
            _vm.ProvideSuggestionResult(dialogViewModel);
        }

        private static IMainViewModel BuildGame()
        {
            var builder = new GameBuilderViewModel();
            new GameBuilderDialog(builder).ShowDialog();
            var game = builder.GameResult;
            if (null == game)
            {
                return UseDefaultGame();
            }
            var myName = builder.Players.First(player => player.Me).Name;
            var me = game.Players.First(player => player.Name == myName);
            var solver = new Solver(game, me);

            // todo, need to figure out how to let the user select the cards they were dealt
            return new MainViewModel(solver);
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

        protected override void OnClosing(CancelEventArgs e)
        {
            _vm.PromptForSuggestionResult -= PromptForSuggestionResult;
            base.OnClosing(e);
        }
    }
}