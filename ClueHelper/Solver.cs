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
        public Player Player { get; }

        public IReadOnlyDictionary<Card, Dictionary<Player, Possibility>> Possibilities { get; }

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
            Player = playAs;
            Possibilities = new ReadOnlyDictionary<Card, Dictionary<Player, Possibility>>(
                Game.Categories
                    .SelectMany(category => category.Cards)
                    .ToDictionary(
                        card => card,
                        card => Game.Players.ToDictionary(
                            player => player,
                            player => Possibility.Unknown)));
        }

        public void PlayerHasCard(Player player, Card card)
        {
            ValidateInGame(player, card);
            if (Possibilities[card][Player] == Possibility.NotHolding)
            {
                throw new InvalidOperationException($"{player.Name} already marked {card.Name} as {Possibility.NotHolding}.");
            }
            player.PutCardInHand(card);
            Possibilities[card][Player] = Possibility.Holding;
        }

        public void PlayerDoesNotHaveCard(Player player, Card card)
        {
            ValidateInGame(player, card);
            if (player.Hand.Contains(card))
            {
                throw new InvalidOperationException($"{player.Name} already has {card.Name} in their hand.");
            }
            Possibilities[card][Player] = Possibility.NotHolding;
        }

        public void PlayerMightHaveCard(Player player, Card card)
        {
            ValidateInGame(player, card);
            if (Possibilities[card][Player] == Possibility.Unknown)
            {
                Possibilities[card][Player] = Possibility.Maybe;
            }
        }

        #region Helpers

        private void ValidateInGame(Player player, Card card)
        {
            if (null == player)
            {
                throw new ArgumentNullException(nameof(player));
            }
            if (null == card)
            {
                throw new ArgumentNullException(nameof(card));
            }
            if (!Game.Players.Contains(player))
            {
                throw new ArgumentException("Player must be from this game.", nameof(player));
            }
            if (!Game.Categories.SelectMany(category => category.Cards).Contains(card))
            {
                throw new ArgumentException("Card must be from this game.", nameof(card));
            }
        }

        #endregion Helpers
    }
}
