using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ClueHelper.Models
{
    public class Game : ObservableObject
    {
        public IReadOnlyCollection<Category> Categories { get; }
        public ReadOnlyCollection<Player> Players { get; }

        private Game(IEnumerable<Category> categories, IEnumerable<Player> players)
        {
            Categories = new ReadOnlyCollection<Category>(categories.ToList());
            Players = new ReadOnlyCollection<Player>(players.ToList());
            Players.First().IsTakingTurn = true;
        }

        public void NextTurn()
        {
            var found = false;

            foreach (var player in Players)
            {
                if (found)
                {
                    player.IsTakingTurn = true;
                    return;
                }

                found = player.IsTakingTurn;
                player.IsTakingTurn = false;
            }

            Players.First().IsTakingTurn = true;
        }

        public class Builder
        {
            private readonly IList<Category> _categories = new List<Category>();
            private readonly IList<Player> _players = new List<Player>();

            public Builder AddCategory(Category category)
            {
                if (null == category)
                {
                    throw new ArgumentNullException(nameof(category));
                }
                if (_categories.Any(c => c.Name == category.Name))
                {
                    throw new ArgumentException($"Category already exists with name {category.Name}.", nameof(category));
                }

                _categories.Add(category);

                return this;
            }

            public Builder AddPlayer(string name, Card representative)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentNullException(nameof(name));
                }
                if (_players.Any(p => p.Name == name))
                {
                    throw new ArgumentException($"Player already exists with name {name}.", nameof(name));
                }
                if (null == representative)
                {
                    throw new ArgumentNullException(nameof(representative));
                }
                if (_categories.SelectMany(category => category.Cards).All(card => !ReferenceEquals(card, representative)))
                {
                    throw new ArgumentException("Card must be from an existing category.", nameof(representative));
                }
                if(_players.Any(p => ReferenceEquals(p.Representative, representative)))
                {
                    throw new ArgumentException("Card is already representing another player.", nameof(representative));
                }

                var player = new Player(name, representative);
                representative.Representative = player;
                _players.Add(player);

                return this;
            }

            public Game Build()
            {
                if (_players.Count < Config.MinPlayers)
                {
                    throw new GameException($"Can't build game without at least {Config.MinPlayers} players.");
                }

                return new Game(_categories, _players);
            }
        }
    }
}
