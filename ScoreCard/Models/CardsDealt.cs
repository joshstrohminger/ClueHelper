using System;
using System.Collections.Generic;
using System.Linq;
using ScoreCard.Interfaces;

namespace ScoreCard.Models
{
    public class CardsDealt : IGameChange
    {
        public DateTime DateTime { get; }
        public Player Player { get; }
        public Card[] Cards { get; }
        public PlayerPossibility Possibility { get; } = null;

        public CardsDealt(DateTime dateTime, Player player, IEnumerable<Card> cards)
        {
            DateTime = dateTime;
            Player = player;
            Cards = cards.ToArray();
        }

        public override string ToString()
        {
            return $"{DateTime:t} {Player.Name} was dealt cards {string.Join(",", Cards.Select(c => c.Name))}";
        }
    }
}
