using ClueHelper.Models;
using ScoreCard.Interfaces;
using ScoreCard.MVVM;

namespace ScoreCard.Views
{
    /// <summary>
    /// Interaction logic for SuggestionResponseDialog.xaml
    /// </summary>
    public partial class SuggestionResponseDialog
    {
        private readonly IDialogViewModel _vm;

        public RelayCommand<DialogResult> CloseWithResult { get; }
        public RelayCommand<Card> CloseWithCard { get; }

        public SuggestionResponseDialog(IDialogViewModel vm)
        {
            InitializeComponent();

            _vm = vm;
            DataContext = _vm;
            CloseWithResult = new RelayCommand<DialogResult>(DoCloseWithResult);
            CloseWithCard = new RelayCommand<Card>(DoCloseWithCard);
        }

        private void DoCloseWithCard(Card card)
        {
            _vm.Result = Interfaces.DialogResult.Card;
            _vm.ResultCard = card;
            Close();
        }

        private void DoCloseWithResult(DialogResult dialogResult)
        {
            _vm.Result = dialogResult;
            Close();
        }
    }
}
