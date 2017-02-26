using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ClueHelper.Models;
using ScoreCard.Interfaces;
using ScoreCard.MVVM;

namespace ScoreCard.ViewModels
{
    public class SelectCardsViewModel : ISelectCardsViewModel
    {
        private readonly int _min;
        private readonly int _max;
        public IReadOnlyCollection<Category> Categories { get; }
        public ICommand Done { get; }
        public event EventHandler DoneSelecting;

        public SelectCardsViewModel(IEnumerable<Category> categories, int min, int max)
        {
            _min = min;
            _max = max;
            Categories = new ReadOnlyCollection<Category>(categories.ToList());
            Done = new RelayCommand(NotifyDone, CanNotifyDone);
        }

        private bool CanNotifyDone()
        {
            var selected = Categories.SelectMany(c => c.Cards).Count(c => c.IsPartOfSuggestion);
            return selected >= _min && selected <= _max;
        }

        private void NotifyDone()
        {
            if (CanNotifyDone())
            {
                DoneSelecting?.Invoke(this, new EventArgs());
            }
        }
    }
}
