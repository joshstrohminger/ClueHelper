using System;
using System.Collections.Generic;
using System.Linq;
using ScoreCard.Interfaces;

namespace ScoreCard.Models
{
    public class SuggestionMade : IGameChange
    {
        public SuggestionMade(DateTime dateTime, Player playerAsking, IEnumerable<Card> cards)
        {
            DateTime = dateTime;
            PlayerAsking = playerAsking;
            Cards = cards.ToArray();
        }

        public DateTime DateTime { get; }
        public Player PlayerAsking { get; }
        public Card[] Cards { get; }
        public PlayerPossibility Possibility { get; } = null;

        public override string ToString()
        {
            return $"{DateTime:t} {PlayerAsking.Name} suggested {string.Join(",", Cards.Select(c => c.Name))}";
        }
    }
}
