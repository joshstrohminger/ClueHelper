using System.Collections.Generic;
using ScoreCard.Models;

namespace ScoreCard.Interfaces
{
    public enum DialogResult
    {
        Cancel,
        Skip,
        Maybe,
        None,
        Card
    }

    public interface ISuggestionResponseViewModel
    {
        Player Responder { get; }
        IReadOnlyCollection<Card> Cards { get; }
        DialogResult Result { get; set; }
        Card ResultCard { get; set; }
    }
}
