using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClueHelper.Models;

namespace ClueHelper
{
    internal class Solver
    {
        private readonly Game _game;
        private readonly Player _player;

        public IReadOnlyDictionary<Card, ReadOnlyDictionary<Player, Possibility>> Possibilities { get; }

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
            if (game.Players.All(player => !object.ReferenceEquals(player, playAs)))
            {
                throw new ArgumentException("You must play as a player from the game.", nameof(playAs));
            }

            _game = game;
            _player = playAs;
            Possibilities = new ReadOnlyDictionary<Card, ReadOnlyDictionary<Player, Possibility>>(
                _game.Categories
                    .SelectMany(category => category.Cards)
                    .ToDictionary(
                        card => card,
                        card => new ReadOnlyDictionary<Player, Possibility>(_game.Players.ToDictionary(
                            player => player,
                            player => Possibility.Unknown))));
        }
    }
}
