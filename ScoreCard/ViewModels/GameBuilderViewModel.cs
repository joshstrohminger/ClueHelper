using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ClueHelper;
using ClueHelper.Models;
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
            AddPlayer = new RelayCommand(() => Players.Add(new PotentialPlayer {Piece = _defaultCard}));
            RemovePlayer = new RelayCommand<PotentialPlayer>(player => Players.Remove(player));
            Build = new RelayCommand(BuildGame, CanBuildGame);
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
