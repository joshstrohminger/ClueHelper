using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClueHelper.Models;

namespace ScoreCard
{
    public enum DialogResult
    {
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
