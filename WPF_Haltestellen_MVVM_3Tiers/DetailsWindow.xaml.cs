using System;
using System.Windows;



namespace WPF_Haltestellen_MVVM_3Tiers
{
    /// <summary>
    /// Interaktionslogik für DetailsWindow.xaml
    /// </summary>
    public partial class DetailsWindow : Window
    {
        public DetailsWindow(Haltestellen haltestelle)
        {
            InitializeComponent();
            ZeigeKarte(haltestelle);
            UpdateLbl(haltestelle);
        }

        private async void ZeigeKarte(Haltestellen haltestelle)
        {
            await WebView.EnsureCoreWebView2Async();

            var laenge = haltestelle.Laenge.Replace(',', '.');
            var breite = haltestelle.Breite.Replace(',', '.');

            //Eingebettete Karten erzeugen. Siehe:
            //https://www.maps.ie/create-google-map/
            //https://www.mapsdirections.info/de/erstellen-sie-eine-google-map/

            //Eingebettete Karte vorbereiten
            string HtmlCode = @$"
               <!DOCTYPE html>
               <html lang=""de"" xmlns=""http://www.w3.org/1999/xhtml"">
                 <head>
                   <meta charset = ""utf-8"" />
                   <title></title>
                 </head>
                 <body>
                   <div style=""width: 100%""><iframe width=""100%"" height=""600"" frameborder=""0"" scrolling=""no"" marginheight=""0"" marginwidth=""0"" src=""https://maps.google.com/maps?width=100%25&amp;height=600&amp;hl=de&amp;q={breite},{laenge}&amp;t=k&amp;z=17&amp;ie=UTF8&amp;iwloc=B&amp;output=embed""></iframe></div>
                 </body>
               </html>";

            //und im WebView2-Control anzeigen
            WebView.CoreWebView2.NavigateToString(HtmlCode);
            
        }

        public void UpdateLbl(Haltestellen haltestelle)
        {
            if (haltestelle == null)
            {
                return;
            } else
            {
                lbl_Nr.Content = haltestelle.EVA_NR;
                lbl_NameVonHaltestelle.Content = haltestelle.NAME;
                lbl_Laengengrad.Content = haltestelle.Laenge;
                lbl_Breitengrad.Content = haltestelle.Breite;
                lbl_Betriebstelle.Content = haltestelle.Betreiber_Name;
                lbl_Verkehr.Content = haltestelle.Verkehr;
            }
        }
    }
}
