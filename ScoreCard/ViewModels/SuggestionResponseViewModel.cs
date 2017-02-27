﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ClueHelper.Models;
using ScoreCard.Interfaces;

namespace ScoreCard.ViewModels
{
    public class SuggestionResponseViewModel : ISuggestionResponseViewModel
    {
        public Player Responder { get; }
        public IReadOnlyCollection<Card> Cards { get; }
        public DialogResult Result { get; set; }
        public Card ResultCard { get; set; }

        public SuggestionResponseViewModel(Player responder, IEnumerable<Card> cards)
        {
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