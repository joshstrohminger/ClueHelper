﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ScoreCard.Interfaces;
using ScoreCard.Models;
using ScoreCard.MVVM;

namespace ScoreCard.DesignData
{
    public class GameBuilderViewModelDesignData : IGameBuilderViewModel
    {
        public ICommand AddPlayer { get; } = new RelayCommand(() => {});
        public RelayCommand<PotentialPlayer> RemovePlayer { get; } = new RelayCommand<PotentialPlayer>(o => {});
        public ObservableCollection<PotentialPlayer> Players { get; } = new ObservableCollection<PotentialPlayer>();
        public ReadOnlyCollection<Card> Pieces { get; }
        public ICommand Build { get; } = new RelayCommand(() => {});

        public GameBuilderViewModelDesignData()
        {
            var people = Config.BuildDefaultPeople();

            Pieces = new ReadOnlyCollection<Card>(people.Cards.ToList());

            Players.Add(new PotentialPlayer {Name = "Josh"});
            Players.Add(new PotentialPlayer { Name = "Frank", Piece = Pieces.First()});
            GameBuilt?.Invoke(this, new EventArgs());
        }

        public Game GameResult { get; } = null;
        public event EventHandler GameBuilt;
    }
}
