using System;
using System.Threading;
using System.Windows;

namespace WPF_Haltestellen_MVVM_3Tiers;

/// <summary>
/// Interaktionslogik für Window1.xaml
/// </summary>
public partial class DownloadProgress : Window
{
    public Action OnCancelClicked { get; set; }
    public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

    public DownloadProgress()
    {
        InitializeComponent();
    }

    public void UpdateProgress(double value)
    {
        progressBar.Value = value;
        percentageText.Text = $"{value:0.0}%";
    }

    public void SetFileName(string fileName)
    {
        fileNameText.Text = $"Downloading: {fileName}";
    }

    private void OnCancelButtonClick(object sender, RoutedEventArgs e)
    {
        CancellationTokenSource.Cancel();
        OnCancelClicked?.Invoke();
    }
}