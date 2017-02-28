using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ScoreCard.Interfaces;
using ScoreCard.Models;

namespace ScoreCard.DesignData
{
    public class SuggestionResponseViewModelDesignData : ISuggestionResponseViewModel
    {
        public Player Responder { get; }
        public Player Asker { get; }
        public IReadOnlyCollection<Card> Cards { get; }
        public DialogResult Result { get; set; }
        public Card ResultCard { get; set; }
        public bool CanChooseCard { get; } = false;

        public SuggestionResponseViewModelDesignData()
        {
            var a = new Category.Builder("a").AddCard("First Card Here").Build();
            var b = new Category.Builder("b").AddCard("Second Card Here").Build();
            var c = new Category.Builder("c").AddCard("Third Card Here").Build();
            Responder = new Player("Mark", a.Cards.First());
            Asker = new Player("Joe", a.Cards.Last());
            Cards = new ReadOnlyCollection<Card>(a.Cards.Concat(b.Cards).Concat(c.Cards).ToList());
        }
    }
}
