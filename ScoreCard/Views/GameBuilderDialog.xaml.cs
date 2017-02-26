using System;
using System.ComponentModel;
using System.Windows;
using ScoreCard.Interfaces;

namespace ScoreCard.Views
{
    /// <summary>
    /// Interaction logic for GameBuilderDialog.xaml
    /// </summary>
    public partial class GameBuilderDialog
    {
        private readonly IGameBuilderViewModel _vm;

        public GameBuilderDialog(IGameBuilderViewModel vm)
        {
            InitializeComponent();

            _vm = vm;
            DataContext = _vm;
            _vm.GameBuilt += GameBuilt;
        }

        private void GameBuilt(object sender, EventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _vm.GameBuilt -= GameBuilt;
            base.OnClosing(e);
        }
    }
}
