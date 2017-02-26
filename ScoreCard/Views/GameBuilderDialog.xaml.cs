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
using ScoreCard.Interfaces;

namespace ScoreCard.Views
{
    /// <summary>
    /// Interaction logic for GameBuilderDialog.xaml
    /// </summary>
    public partial class GameBuilderDialog : Window
    {
        private readonly IGameBuilderViewModel _vm;

        public GameBuilderDialog(IGameBuilderViewModel vm)
        {
            InitializeComponent();

            _vm = vm;
            DataContext = _vm;
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
