using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ScoreCard.Interfaces;
using ScoreCard.Models;
using ScoreCard.MVVM;

namespace ScoreCard.DesignData
{
    public class SelectCardsViewModelDesignData : ISelectCardsViewModel
    {
        public IReadOnlyCollection<Category> Categories { get; }

        public SelectCardsViewModelDesignData()
        {
            Categories = new ReadOnlyCollection<Category>(new List<Category>
            {
                Config.BuildDefaultPeople(),
                Config.BuildDefaultWeapons(),
                Config.BuildDefaultRooms()
            });

            var i = 0;
            foreach (var category in Categories)
            {
                category.Cards[i++].IsPartOfSuggestion = true;
            }
            DoneSelecting?.Invoke(this,null);
        }

        public ICommand Done { get; } = new RelayCommand(() => { });
        public event EventHandler DoneSelecting;
    }
}
