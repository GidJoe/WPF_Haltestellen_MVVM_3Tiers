using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace WPF_Haltestellen_MVVM_3Tiers
{
    /// <summary>
    /// Interaktionslogik für InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        public InfoWindow()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            OpenLink(e.Uri.ToString());
            e.Handled = true;
        }

        private void OpenLink(string link)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = link,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open link: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}