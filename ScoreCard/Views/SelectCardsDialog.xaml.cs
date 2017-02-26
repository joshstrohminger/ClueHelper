using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ScoreCard.Interfaces;

namespace ScoreCard.Views
{
    /// <summary>
    /// Interaction logic for SelectCardsDialog.xaml
    /// </summary>
    public partial class SelectCardsDialog : Window
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
