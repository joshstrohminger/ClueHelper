﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ScoreCard.Interfaces;
using ScoreCard.Models;

namespace ScoreCard.ViewModels
{
    public class SuggestionResponseViewModel : ISuggestionResponseViewModel
    {
        public Player Responder { get; }
        public Player Asker { get; }
        public IReadOnlyCollection<Card> Cards { get; }
        public DialogResult Result { get; set; }
        public Card ResultCard { get; set; }
        public bool CanChooseCard { get; }

        public SuggestionResponseViewModel(Player asker, Player responder, bool canChooseCard, IEnumerable<Card> cards)
        {
            CanChooseCard = canChooseCard;
            Asker = asker;
            Responder = responder;
            Cards = new ReadOnlyCollection<Card>(cards.ToList());
        }

        public void Reset()
        {
            ResultCard = null;
            Result = DialogResult.Cancel;
        }
    }
}
