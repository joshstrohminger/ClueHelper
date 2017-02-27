using System;
using System.Collections.Generic;
using System.Windows.Input;
using ScoreCard.Models;

namespace ScoreCard.Interfaces
{
    public interface ISelectCardsViewModel
    {
        IReadOnlyCollection<Category> Categories { get; }
        ICommand Done { get; }
        event EventHandler DoneSelecting;
    }
}
