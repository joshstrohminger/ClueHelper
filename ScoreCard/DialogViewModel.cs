using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ClueHelper.Models;

namespace ScoreCard
{
    public class DialogViewModel : IDialogViewModel
    {
        public Player Responder { get; }
        public IReadOnlyCollection<Card> Cards { get; }
        public DialogResult Result { get; set; }
        public Card ResultCard { get; set; }

        public DialogViewModel(Player responder, IEnumerable<Card> cards)
        {
            Responder = responder;
            Cards = new ReadOnlyCollection<Card>(cards.ToList());
        }
    }
}
