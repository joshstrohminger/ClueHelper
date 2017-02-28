using System;
using ScoreCard.Interfaces;

namespace ScoreCard.Models
{
    public class SuggestionResponse : IGameChange
    {
        public SuggestionResponse(DateTime dateTime, Player playerResponding, string cardName)
        {
            DateTime = dateTime;
            PlayerResponding = playerResponding;
            CardName = cardName;
        }

        public DateTime DateTime { get; }
        public Player PlayerResponding { get; }
        public string CardName { get; }
        public PlayerPossibility Possibility { get; } = null;

        public override string ToString()
        {
            var end = null == CardName ? "didn't have any of the cards." : $"had card '{CardName}'.";
            return $"{DateTime:t} {PlayerResponding.Name} {end}";
        }
    }
}
