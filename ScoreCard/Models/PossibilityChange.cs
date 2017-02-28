using System;
using ScoreCard.Interfaces;

namespace ScoreCard.Models
{
    public class PossibilityChange : IGameChange
    {
        public PlayerPossibility Possibility { get; }
        public Possibility Before { get; }
        public Possibility After { get; }
        public string Reason { get; }
        public Player PlayerAsking { get; }
        public Player PlayerAnswering { get; }
        public DateTime DateTime { get; }

        public PossibilityChange(PlayerPossibility possibility, Possibility before, Possibility after, string reason, Player playerAsking, Player playerAnswering, DateTime dateTime)
        {
            Possibility = possibility;
            Before = before;
            After = after;
            Reason = reason;
            PlayerAsking = playerAsking;
            PlayerAnswering = playerAnswering;
            DateTime = dateTime;
        }

        public override string ToString()
        {
            return $"{DateTime:t} {Possibility.Player.Name}/{Possibility.Card.Name}: {Before} => {After}, {Reason}";
        }
    }
}
