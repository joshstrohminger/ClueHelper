using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ScoreCard.Interfaces;
using ScoreCard.Models;
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
            _vm.PromptForSimpleResponse += PromptForSimpleResponse;
        }

        private void PromptForSimpleResponse(object sender, SimplePrompt e)
        {
            e.Result = MessageBox.Show(this, e.Message, e.Title, e.Button, e.Image);
        }

        private void PromptForSuggestionResult(object sender, ISuggestionResponseViewModel suggestionResponseViewModel)
        {
            new SuggestionResponseDialog(suggestionResponseViewModel)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            }.ShowDialog();
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

            var cardsPerPlayer = (game.Categories.SelectMany(c => c.Cards).Count() - game.CardsPerSuggestion) / (double)game.Players.Count;
            var selectCardsViewModel = new SelectCardsViewModel(game.Categories, (int)Math.Floor(cardsPerPlayer), (int)Math.Ceiling(cardsPerPlayer));
            if (false == new SelectCardsDialog(selectCardsViewModel).ShowDialog())
            {
                return UseDefaultGame();
            }
            var cardsDealt = game.Categories.SelectMany(c => c.Cards).Where(c => c.IsPartOfSuggestion).ToArray();
            solver.PlayerWasDealtCards(me, cardsDealt);
            foreach (var card in cardsDealt)
            {
                card.IsPartOfSuggestion = false;
            }
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

            solver.PlayerWasDealtCards(me, shuffledCards.Take(cardsPerHand));

            return new MainViewModel(solver);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _vm.PromptForSuggestionResult -= PromptForSuggestionResult;
            base.OnClosing(e);
        }

        private void HightlightItem(object sender, bool highlight)
        {
            var element = sender as FrameworkElement;
            var possibility = element?.DataContext as PlayerPossibility ??
                              (element?.DataContext as PossibilityChange)?.Possibility;
            if (possibility == null || possibility.Possibility == Possibility.Unknown)
            {
                return;
            }

            possibility.IsHighlighted = highlight;
        }

        private void HighlightedItem_OnMouseEnter(object sender, MouseEventArgs e)
        {
            HightlightItem(sender, true);
        }

        private void HighlightedItem_OnMouseLeave(object sender, MouseEventArgs e)
        {
            HightlightItem(sender, false);
        }
    }
}