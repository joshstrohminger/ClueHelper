﻿using System;
using System.Linq;
using System.Windows.Input;
using ClueHelper;
using ClueHelper.Models;
using ScoreCard.Interfaces;
using ScoreCard.MVVM;

namespace ScoreCard.DesignData
{
    public class MainViewModelDesignData : IMainViewModel
    {
        public Solver Solver { get; }
        public RelayCommand<Player> StartSuggestion { get; } = new RelayCommand<Player>(x => { });
        public ICommand MakeSuggestion { get; } = new RelayCommand(() => { });
        public RelayCommand<Card> SuggestCard { get; } = new RelayCommand<Card>(x => { });

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
                Solver.PlayerHasCard(me, card);
            }

            Solver.PlayerHasCard(game.Players.First(), shuffledCards.Skip(cardsPerHand).First());
            Solver.PlayerMightHaveCards(game.Players.Skip(1).First(), shuffledCards.Skip(cardsPerHand + 1).Take(game.CardsPerSuggestion));
            Solver.PlayerDoesNotHaveCards(game.Players.Skip(2).First(), shuffledCards.Skip(cardsPerHand + game.CardsPerSuggestion + 1).Take(game.CardsPerSuggestion));
            Solver.SuggestionLooped(game.Players.First(), shuffledCards.Skip(cardsPerHand + 2 * game.CardsPerSuggestion + 1).Take(game.CardsPerSuggestion));

            shuffledCards.Last().IsPartOfAccusation = true;
        }
    }
}