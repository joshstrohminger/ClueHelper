using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ClueHelper.Models;

namespace ClueHelper
{
    public class Solver
    {
        public Game Game { get; }
        public Player MyPlayer { get; }

        public IReadOnlyDictionary<Card, Dictionary<Player, PlayerPossibility>> Possibilities { get; }
        public IReadOnlyDictionary<Player, ObservableCollection<List<Card>>> PlayerMaybeHistory { get; }

        public Solver(Game game, Player playAs)
        {
            if (null == game)
            {
                throw new ArgumentNullException(nameof(game));
            }
            if (null == playAs)
            {
                throw new ArgumentNullException(nameof(playAs));
            }
            if (game.Players.All(player => !ReferenceEquals(player, playAs)))
            {
                throw new ArgumentException("You must play as a player from the game.", nameof(playAs));
            }

            Game = game;
            MyPlayer = playAs;

            foreach (var card in Game.Categories.SelectMany(category => category.Cards))
            {
                foreach (var player in Game.Players)
                {
                    card.Possibilities.Add(new PlayerPossibility(player));
                }
            }

            Possibilities = new ReadOnlyDictionary<Card, Dictionary<Player, PlayerPossibility>>(
                Game.Categories
                    .SelectMany(category => category.Cards)
                    .ToDictionary(
                        card => card,
                        card => card.Possibilities.ToDictionary(
                            possibility => possibility.Player,
                            possibility => possibility)));

            PlayerMaybeHistory = new ReadOnlyDictionary<Player, ObservableCollection<List<Card>>>(
                Game.Players.ToDictionary(player => player, player => new ObservableCollection<List<Card>>()));
        }

        public void PlayerHasCard(Player player, Card card)
        {
            ValidateInGame(player);
            ValidateInGame(card);
            if (Possibilities[card][player].Possibility == Possibility.NotHolding)
            {
                throw new GameException($"{player.Name} already marked {card.Name} as {Possibility.NotHolding}.");
            }

            // mark player
            player.PutCardInHand(card);
            Possibilities[card][player].Possibility = Possibility.Holding;

            // mark off other players
            foreach (var playerPossibility in Possibilities[card].Values.Where(p => !ReferenceEquals(p.Player, player)))
            {
                playerPossibility.Possibility = Possibility.NotHolding;
            }

            MakeInferences();
        }

        public void PlayerDoesNotHaveCards(Player player, IEnumerable<Card> cardsTheyDontHave)
        {
            ValidateInGame(player);
            var cards = ValidateCardCount(cardsTheyDontHave);
            foreach (var card in cards)
            {
                ValidateInGame(card);
                if (player.Hand.Contains(card))
                {
                    throw new GameException($"{player.Name} already has {card.Name} in their hand.");
                }
            }
            foreach (var card in cards)
            {
                Possibilities[card][player].Possibility = Possibility.NotHolding;
            }
            MakeInferences();
        }

        public void PlayerMightHaveCards(Player player, IEnumerable<Card> cardsTheyMightHave)
        {
            ValidateInGame(player);
            var cards = ValidateCardCount(cardsTheyMightHave);
            foreach (var card in cards)
            {
                ValidateInGame(card);
            }

            var maybes = cards
                .Where(card => Possibilities[card][player].Possibility <= Possibility.Maybe)
                .OrderBy(card => card.Name)
                .ToList();
            if (maybes.Count == 0)
            {
                return;
            }
            foreach (var card in maybes)
            {
                Possibilities[card][player].Possibility = Possibility.Maybe;
            }

            // can't add to maybe history if one of them is already in their hand
            // the info would be meaningless to us in the future so no need to save it
            var notMaybes = cards.Except(maybes).ToList();
            if (notMaybes.All(card => Possibilities[card][player].Possibility == Possibility.NotHolding))
            {
                if (maybes.Count == 1)
                {
                    // no need to add it to the list, just mark it as Holding
                    PlayerHasCard(player, maybes.First());
                    return;
                }

                var history = PlayerMaybeHistory[player];
                // only add unique lists
                if(history.All(list => !list.SequenceEqual(maybes)))
                {
                    history.Add(maybes);   
                }
            }
        }

        private void MakeInferences()
        {
            // todo, check each category to see if we can mark something as known
            // todo, check each player to see if we can mark one of the categories as known
            // todo, go back through each player maybe history to see if we have new info that would allow us to mark something as known
        }

        #region Helpers

        private void ValidateInGame(Player player)
        {
            if (null == player)
            {
                throw new ArgumentNullException(nameof(player));
            }
            if (!Game.Players.Contains(player))
            {
                throw new ArgumentException("Player must be from this game.", nameof(player));
            }
        }

        private void ValidateInGame(Card card)
        {
            if (null == card)
            {
                throw new ArgumentNullException(nameof(card));
            }
            if (!Game.Categories.SelectMany(category => category.Cards).Contains(card))
            {
                throw new ArgumentException("Card must be from this game.", nameof(card));
            }
        }

        private Card[] ValidateCardCount(IEnumerable<Card> cards)
        {
            if (null == cards)
            {
                throw new ArgumentNullException(nameof(cards));
            }

            var cardsToCheck = cards.ToArray();

            if (cardsToCheck.Length != Config.CardsPerSuggestion)
            {
                throw new ArgumentException($"Hands must contain {Config.CardsPerSuggestion} cards.");
            }

            foreach (var card in cardsToCheck)
            {
                ValidateInGame(card);
            }

            return cardsToCheck;
        }

        #endregion Helpers
    }
}
