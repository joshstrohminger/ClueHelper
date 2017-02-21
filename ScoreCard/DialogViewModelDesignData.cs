using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ClueHelper.Models;

namespace ScoreCard
{
    public class DialogViewModelDesignData : IDialogViewModel
    {
        public Player Responder { get; }
        public IReadOnlyCollection<Card> Cards { get; }
        public DialogResult Result { get; set; }
        public Card ResultCard { get; set; }

        public DialogViewModelDesignData()
        {
            var a = new Category.Builder("a").AddCard("a").Build();
            var b = new Category.Builder("b").AddCard("b").Build();
            var c = new Category.Builder("c").AddCard("c").Build();
            Responder = new Player("Mark", a.Cards.First());
            Cards = new ReadOnlyCollection<Card>(a.Cards.Concat(b.Cards).Concat(c.Cards).ToList());

        }
    }
}
