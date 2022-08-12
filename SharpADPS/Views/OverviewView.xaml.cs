using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WPFSharpADPS.ViewModels;

namespace WPFSharpADPS.Views
{
    /// <summary>
    /// View for main window
    /// https://metanit.com/sharp/wpf/22.2.php
    /// </summary>
    public partial class MainWindow
    {
        private bool _isRendered = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        // https://stackoverflow.com/a/13289118
        private void TextBox_KeyEnterUpdate(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tBox = (TextBox)sender;
                DependencyProperty prop = TextBox.TextProperty;

                BindingExpression binding = BindingOperations.GetBindingExpression(tBox, prop);
                if (binding != null) { binding.UpdateSource(); }

                UnfocusElement.Focus();
            }
        }

        private void DataGrid_OnKeyEnterDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                e.Handled = true;
            }
        }

        private void DataGrid_OnKeyEnterUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                var viewModel = (OverviewViewModel)DataContext;
                if (viewModel.OpenMailCommand.CanExecute(null))
                {
                    viewModel.OpenMailCommand.Execute(null);
                    e.Handled = true;
                }
            }
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (_isRendered && (args.HeightChanged || args.WidthChanged))
            {
                SettingsManager.SettingsManager.Settings.OverviewViewWidth = (int)args.NewSize.Width;
                SettingsManager.SettingsManager.Settings.OverviewViewHeight = (int)args.NewSize.Height;
            }
        }

        private void MainWindow_OnContentRendered(object sender, EventArgs args)
        {
            OverviewWindow.Width = SettingsManager.SettingsManager.Settings.OverviewViewWidth;
            OverviewWindow.Height = SettingsManager.SettingsManager.Settings.OverviewViewHeight;
            _isRendered = true;
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs args)
        {
            SettingsManager.SettingsManager.SaveSettings();
        }

        private void Exit_OnClick(object sender, RoutedEventArgs args)
        {
            Close();
        }

        private void About_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Amateur Digital Post Sevice (ADPS). (C) 2022 Ivan Mikhailov (ivanmihval@yandex.ru).", "About");
        }
    }
}
