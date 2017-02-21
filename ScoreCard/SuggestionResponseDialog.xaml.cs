using System;
using System.Collections.Generic;
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

namespace ScoreCard
{
    /// <summary>
    /// Interaction logic for SuggestionResponseDialog.xaml
    /// </summary>
    public partial class SuggestionResponseDialog : Window
    {
        private readonly IDialogViewModel _vm;

        public SuggestionResponseDialog(IDialogViewModel vm)
        {
            InitializeComponent();

            _vm = vm;
            DataContext = _vm;
        }
    }
}
