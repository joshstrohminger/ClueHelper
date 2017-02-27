using System;
using System.Linq;
using System.Windows.Input;
using ScoreCard.Interfaces;
using ScoreCard.Models;
using ScoreCard.MVVM;

namespace ScoreCard.DesignData
{
    public class MainViewModelDesignData : IMainViewModel
    {
        public Solver Solver { get; }
        public RelayCommand<Player> StartSuggestion { get; } = new RelayCommand<Player>(x => { });
        public ICommand MakeSuggestion { get; } = new RelayCommand(() => { });
        public RelayCommand<Card> SuggestCard { get; } = new RelayCommand<Card>(x => { });

        public event EventHandler<ISuggestionResponseViewModel> PromptForSuggestionResult;

        public void ProvideSuggestionResult(ISuggestionResponseViewModel vm)
        {
            PromptForSuggestionResult?.Invoke(this, null);
        }

        public MainViewModelDesignData()
        {
            var game = Config.BuildDefaultGame();
            var me = game.Players.Last();

            var random = new Random();

            Solver = new Solver(game, me);

            var shuffledCards = game.Categories
                .SelectMany(category => category.Cards)
                .OrderBy(card => random.Next())
                .Skip(game.CardsPerSuggestion) // the answer
                .ToArray();

            var cardsPerHand = shuffledCards.Length / game.Players.Count;

            foreach (var card in shuffledCards.Take(cardsPerHand))
            {
                Solver.PlayerHasCard(me, card, $"{me.Name} was dealt card.");
            }

            Solver.PlayerHasCard(game.Players.First(), shuffledCards.Skip(cardsPerHand).First(), "Player has card");
            Solver.PlayerMightHaveCards(game.Players.Skip(1).First(), shuffledCards.Skip(cardsPerHand + 1).Take(game.CardsPerSuggestion), "Player has card");
            Solver.PlayerDoesNotHaveCards(game.Players.Skip(2).First(), shuffledCards.Skip(cardsPerHand + game.CardsPerSuggestion + 1).Take(game.CardsPerSuggestion), "Player has card");
            Solver.SuggestionLooped(game.Players.First(), shuffledCards.Skip(cardsPerHand + 2 * game.CardsPerSuggestion + 1).Take(game.CardsPerSuggestion));
            Solver.SuggestionLooped(game.Players.First(), shuffledCards.Skip(cardsPerHand + 3 * game.CardsPerSuggestion + 1).Take(game.CardsPerSuggestion));
            Solver.SuggestionLooped(game.Players.Skip(1).First(), shuffledCards.Skip(cardsPerHand + 4 * game.CardsPerSuggestion + 1).Take(game.CardsPerSuggestion));

            shuffledCards.Last().IsPartOfAccusation = true;

            //Solver.Changes.Add(new PossibilityChange(Solver.Possibilities.Values.First().Values.First(), Possibility.Maybe, Possibility.NotHolding, "Someone else had it.", Solver.Game.Players.First(), Solver.Game.Players.Skip(1).First(), DateTime.Now));
            //Solver.Changes.Add(new PossibilityChange(Solver.Possibilities.Values.First().Values.First(), Possibility.Unknown, Possibility.Holding, "They had it.", Solver.Game.Players.Skip(2).First(), Solver.Game.Players.Skip(1).First(), DateTime.Now.AddMinutes(1)));
            //Solver.Changes.Add(new PossibilityChange(Solver.Possibilities.Values.First().Values.First(), Possibility.Maybe, Possibility.Holding, "Someone else had the other one.", Solver.Game.Players.Last(), Solver.Game.Players.Skip(1).First(), DateTime.Now.AddMinutes(2)));
        }
    }
}
