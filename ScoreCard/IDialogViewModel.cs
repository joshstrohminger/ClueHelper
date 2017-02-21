using System.Collections.Generic;
using ClueHelper.Models;

namespace ScoreCard
{
    public enum DialogResult
    {
        Cancel,
        Skip,
        Maybe,
        None,
        Card
    }

    public interface IDialogViewModel
    {
        Player Responder { get; }
        IReadOnlyCollection<Card> Cards { get; }
        DialogResult Result { get; set; }
        Card ResultCard { get; set; }
    }
}
