using System.Threading;
using System.Windows;
using System;
using System.IO;
using Microsoft.Win32;
using System.Net.Http;
using System.Threading.Tasks;

namespace WPF_Haltestellen_MVVM_3Tiers
{
    internal class DownloadHelper
    {
        public static async Task DownloadAsync(string url, string outputPath, IProgress<double> progress, CancellationToken cancellationToken)
        {
            const int bufferSize = 8192; 

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();

                    using (var remoteStream = await response.Content.ReadAsStreamAsync(cancellationToken))
                    using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        var totalBytes = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : -1L;
                        var bytesRead = 0L;
                        var buffer = new byte[bufferSize];
                        var isMoreToRead = true;

                        do
                        {
                            var read = await remoteStream.ReadAsync(buffer, cancellationToken);
                            if (read == 0)
                            {
                                isMoreToRead = false;
                                continue;
                            }

                            await fileStream.WriteAsync(buffer, 0, read, cancellationToken);

                            bytesRead += read;
                            if (totalBytes != -1)
                            {
                                var percent = (double)bytesRead / totalBytes * 100;
                                progress.Report(percent);
                            }
                        }
                        while (isMoreToRead);
                    }
                }
            }
        }

        public string OpenSaveDialog(string filter, string title)
        {
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = filter;
            saveFileDialog.Title = title;

            bool? result = saveFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                return saveFileDialog.FileName;
            }

            return string.Empty;
        }
    }
}
