using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ScoreCard.Interfaces;
using ScoreCard.Models;
using ScoreCard.MVVM;
using ObservableObject = ScoreCard.MVVM.ObservableObject;

namespace ScoreCard.ViewModels
{
    public class GameBuilderViewModel : ObservableObject, IGameBuilderViewModel
    {
        private readonly Category _people;
        private readonly Card _defaultCard;
        public ICommand AddPlayer { get; }
        public RelayCommand<PotentialPlayer> RemovePlayer { get; }
        public ObservableCollection<PotentialPlayer> Players { get; } = new ObservableCollection<PotentialPlayer>();
        public ICommand Build { get; }
        public ReadOnlyCollection<Card> Pieces { get; }
        public Game GameResult { get; private set; }

        public event EventHandler GameBuilt;

        public GameBuilderViewModel()
        {
            _people = Config.BuildDefaultPeople();

            _defaultCard = new Card(_people, "Choose a piece...");
            Pieces = new ReadOnlyCollection<Card>(new [] {_defaultCard}.Concat(_people.Cards).ToList());

            AddPlayer = new RelayCommand(DoAddPlayer, CanAddPlayer);
            RemovePlayer = new RelayCommand<PotentialPlayer>(player => Players.Remove(player));
            Build = new RelayCommand(BuildGame, CanBuildGame);
        }

        private void DoAddPlayer()
        {
            if (!CanAddPlayer())
            {
                return;
            }

            var player = new PotentialPlayer {Piece = _defaultCard};
            WeakEventManager<PotentialPlayer,PropertyChangedEventArgs>.AddHandler(player, nameof(PropertyChanged), PlayerPropertyChangedHandler);
            Players.Add(player);
        }

        private bool CanAddPlayer()
        {
            return Players.Count < Pieces.Count - 1;
        }

        private void PlayerPropertyChangedHandler(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(PotentialPlayer.Piece))
            {
                var changedPlayer = (PotentialPlayer)sender;
                if (changedPlayer.Piece != _defaultCard)
                {
                    foreach (var player in Players.Where(p => p != changedPlayer && p.Piece == changedPlayer.Piece))
                    {
                        player.Piece = _defaultCard;
                    }
                }
            }
        }

        private bool CanBuildGame()
        {
            return
                Players.Count >= Config.MinPlayers &&
                Players.Any(player => player.Me) &&
                Players.All(player => !ReferenceEquals(player.Piece, _defaultCard) && !string.IsNullOrWhiteSpace(player.Name)) &&
                Players.Select(player => player.Name).Distinct().Count() == Players.Count &&
                Players.Select(player => player.Piece.Name).Distinct().Count() == Players.Count;
        }

        private void BuildGame()
        {
            var builder = new Game.Builder()
                .AddCategory(_people)
                .AddCategory(Config.BuildDefaultRooms())
                .AddCategory(Config.BuildDefaultWeapons());

            foreach (var player in Players)
            {
                builder.AddPlayer(player.Name, player.Piece);
            }

            GameResult = builder.Build();
            GameBuilt?.Invoke(this, new EventArgs());
        }
    }
}
