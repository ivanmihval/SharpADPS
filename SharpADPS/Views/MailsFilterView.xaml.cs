using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFSharpADPS.ViewModels;

namespace WPFSharpADPS.Views
{
    /// <summary>
    /// Interaction logic for MailsFilterView.xaml
    /// </summary>
    public partial class MailsFilterView : Window
    {
        private readonly MailsFilterViewModel _viewModel;

        public MailsFilterView(MailsFilterViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            DataContext = _viewModel;
            _viewModel.ThisWindow = this;
        }

        public void FilterModeCombobox_OnLoaded(object o, RoutedEventArgs args)
        {
            var comboBox = o as ComboBox;
            if (comboBox != null)
            {
                comboBox.SelectedIndex = 0;
            }
        }
    }
}
