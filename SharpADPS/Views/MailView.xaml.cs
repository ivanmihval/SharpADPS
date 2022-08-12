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
    /// Interaction logic for MailView.xaml
    /// </summary>
    public partial class MailView : Window
    {
        private readonly MailViewModel _viewModel;

        public MailView(MailViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            DataContext = _viewModel;
            _viewModel.ThisWindow = this;
        }

        private void AttachmentsListBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.IsAttachmentsListBoxSelected = false;
        }

        private void AttachmentsListBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.IsAttachmentsListBoxSelected = true;
        }

        private void CoordinatesListBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.IsCoordinatesListBoxSelected = false;
        }

        private void CoordinatesListBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.IsCoordinatesListBoxSelected = true;
        }
    }
}
