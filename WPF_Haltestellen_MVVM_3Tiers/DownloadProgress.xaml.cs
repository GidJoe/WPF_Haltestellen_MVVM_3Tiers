using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System;
using Microsoft.Win32;

namespace WPF_Haltestellen_MVVM_3Tiers;

/// <summary>
/// Interaktionslogik für Window1.xaml
/// </summary>
public partial class DownloadProgress : Window
{
    public Action OnCancelClicked { get; set; }
    

    public DownloadProgress()
    {
        InitializeComponent();
    }

    public void UpdateProgress(double value)
    {
        downloadProgress.Value = value;
        percentageText.Text = $"{value:0.0}%";
    }

    public void SetFileName(string fileName)
    {
        fileNameText.Text = $"Downloading: {fileName}";
    }

    private void OnCancelButtonClick(object sender, RoutedEventArgs e)
    {
        OnCancelClicked?.Invoke();
    }

    


}