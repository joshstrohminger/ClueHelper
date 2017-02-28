using System;
using System.ComponentModel;
using ScoreCard.Interfaces;

namespace ScoreCard.Views
{
    /// <summary>
    /// Interaction logic for SelectCardsDialog.xaml
    /// </summary>
    public partial class SelectCardsDialog
    {
        private readonly ISelectCardsViewModel _vm;
        public SelectCardsDialog(ISelectCardsViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            DataContext = _vm;
            _vm.DoneSelecting += DoneSelecting;
        }

        private void DoneSelecting(object sender, EventArgs eventArgs)
        {
            DialogResult = true;
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _vm.DoneSelecting -= DoneSelecting;
            base.OnClosing(e);
        }
    }
}
