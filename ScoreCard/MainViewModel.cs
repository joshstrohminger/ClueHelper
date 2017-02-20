using ClueHelper;

namespace ScoreCard
{
    public class MainViewModel : ObservableObject, IMainViewModel
    {
        #region Properties

        public Solver Solver { get; }

        #endregion Properties
        public MainViewModel(Solver solver)
        {
            Solver = solver;
        }
    }
}