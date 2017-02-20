using System.Linq;
using ClueHelper;
using ClueHelper.Models;

namespace ScoreCard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IMainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();

            var game = Config.BuildDefaultGame();

            _vm = new MainViewModel(new Solver(game, game.Players.Last()));
            DataContext = _vm;
        }
    }
}