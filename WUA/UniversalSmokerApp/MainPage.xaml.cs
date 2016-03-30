using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UniversalSmokerApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            
        }

        private async void Get_Button_Click(object sender, RoutedEventArgs e)
        {
            string status = "";
            try
            {
                status = await HttpUtilities.HttpGet(IPInput.Text) + "\r\n";
            }
            catch
            {
                var dialog = new Windows.UI.Popups.MessageDialog("Invalid Smoker IP Address");
                await dialog.ShowAsync();
                return;
            }

            string[] items =   status.Split(new string[] { "</td></tr>" },StringSplitOptions.None);
            string smokerTemp = items[0].Substring(items[0].LastIndexOf("<td>")+4);
            Dictionary<string, string> probes = new Dictionary<string, string>();
            for (int i=1; i < items.Count(); i++)
            {
                if(items[i].Contains("Probe"))
                {
                    string probeInfo = items[i].Substring(items[i].IndexOf("P"));
                    string[] probeData = probeInfo.Split(new string[]
                        { "</td><td>" }, StringSplitOptions.None);
                    probes.Add(probeData[0], probeData[1]);
                }
                if (items[i].Contains("Target Temperature"))
                {
                    string targetTemp = items[i].Substring(items[i].IndexOf("value") + 7);
                    targetTemp = targetTemp.Remove(targetTemp.IndexOf("'"));
                    tempInput.PlaceholderText = targetTemp;
                    float target = 0;

                    float.TryParse(targetTemp, out target);
                    smokerTempData.Content = smokerTemp;

                    smokerTemp = smokerTemp.Remove(smokerTemp.IndexOf(' '));
                    float tempActual = 0;
                    float.TryParse(smokerTemp, out tempActual);

                    if (target > tempActual + 5)
                    {
                        smokerTempData.Background = new SolidColorBrush(Windows.UI.Colors.DodgerBlue);
                        smokerTempLbl.Background = new SolidColorBrush(Windows.UI.Colors.DodgerBlue);
                    }
                    if (target >= tempActual - 5 && target <= tempActual + 5)
                    {
                        smokerTempData.Background = new SolidColorBrush(Windows.UI.Colors.Lime);
                        smokerTempLbl.Background = new SolidColorBrush(Windows.UI.Colors.Lime);
                    }
                    if (target < tempActual - 5)
                    {
                        smokerTempData.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                        smokerTempLbl.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                    }
                }
            }

            string data;
            float temp = 0;

            probes.TryGetValue("Probe 1", out data);
            if (data == null)
            { data = "disconnected"; temp = 0; probe1TempData.Content = data;
            }
            else
            {
                probe1TempData.Content = data;
                data = data.Remove(data.IndexOf(' '));
                float.TryParse(data, out temp);
            }
            if (temp < 190)
            {
                probe1TempData.Background = new SolidColorBrush(Windows.UI.Colors.DodgerBlue);
                probe1TempLbl.Background = new SolidColorBrush(Windows.UI.Colors.DodgerBlue);
            }
            if (temp >= 190 && temp <= 205)
            {
                probe1TempData.Background = new SolidColorBrush(Windows.UI.Colors.Lime);
                probe1TempLbl.Background = new SolidColorBrush(Windows.UI.Colors.Lime);
            }
            if (temp > 205)
            {
                probe1TempData.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                probe1TempLbl.Background = new SolidColorBrush(Windows.UI.Colors.Red);
            }

            probes.TryGetValue("Probe 2", out data);
            if (data == null)
            { data = "disconnected"; temp = 0; probe2TempData.Content = data;
            }
            else
            {
                probe2TempData.Content = data;
                data = data.Remove(data.IndexOf(' '));
                float.TryParse(data, out temp);
            }
            if (temp < 190)
            {
                probe2TempData.Background = new SolidColorBrush(Windows.UI.Colors.DodgerBlue);
                probe2TempLbl.Background = new SolidColorBrush(Windows.UI.Colors.DodgerBlue);
            }
            if (temp >= 190 && temp <= 205)
            {
                probe2TempData.Background = new SolidColorBrush(Windows.UI.Colors.Lime);
                probe2TempLbl.Background = new SolidColorBrush(Windows.UI.Colors.Lime);
            }
            if (temp > 205)
            {
                probe2TempData.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                probe2TempLbl.Background = new SolidColorBrush(Windows.UI.Colors.Red);
            }

            probes.TryGetValue("Probe 3", out data);
            if (data == null)
            { data = "disconnected"; temp = 0; probe3TempData.Content = data;
            }
            else
            {
                probe3TempData.Content = data;
                data = data.Remove(data.IndexOf(' '));
                float.TryParse(data, out temp);
            }
            if (temp < 190)
            {
                probe3TempData.Background = new SolidColorBrush(Windows.UI.Colors.DodgerBlue);
                probe3TempLbl.Background = new SolidColorBrush(Windows.UI.Colors.DodgerBlue);
            }
            if (temp >= 190 && temp <= 205)
            {
                probe3TempData.Background = new SolidColorBrush(Windows.UI.Colors.Lime);
                probe3TempLbl.Background = new SolidColorBrush(Windows.UI.Colors.Lime);
            }
            if (temp > 205)
            {
                probe3TempData.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                probe3TempLbl.Background = new SolidColorBrush(Windows.UI.Colors.Red);
            }

            probes.TryGetValue("Probe 4", out data);
            if (data == null)
            { data = "disconnected"; temp = 0; probe4TempData.Content = data;
            }
            else
            {
                probe4TempData.Content = data;
                data = data.Remove(data.IndexOf(' '));
                float.TryParse(data, out temp);
            }
            if (temp < 190)
            {
                probe4TempData.Background = new SolidColorBrush(Windows.UI.Colors.DodgerBlue);
                probe4TempLbl.Background = new SolidColorBrush(Windows.UI.Colors.DodgerBlue);
            }
            if (temp >= 190 && temp <= 205)
            {
                probe4TempData.Background = new SolidColorBrush(Windows.UI.Colors.Lime);
                probe4TempLbl.Background = new SolidColorBrush(Windows.UI.Colors.Lime);
            }
            if (temp > 205)
            {
                probe4TempData.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                probe4TempLbl.Background = new SolidColorBrush(Windows.UI.Colors.Red);
            }

            probes.TryGetValue("Probe 5", out data);
            if (data == null)
            { data = "disconnected"; temp = 0; probe5TempData.Content = data;
            }
            else
            {
                probe5TempData.Content = data;
                data = data.Remove(data.IndexOf(' '));
                float.TryParse(data, out temp);
            }
            if (temp < 190)
            {
                probe5TempData.Background = new SolidColorBrush(Windows.UI.Colors.DodgerBlue);
                probe5TempLbl.Background = new SolidColorBrush(Windows.UI.Colors.DodgerBlue);
            }
            if (temp >= 190 && temp <= 205)
            {
                probe5TempData.Background = new SolidColorBrush(Windows.UI.Colors.Lime);
                probe5TempLbl.Background = new SolidColorBrush(Windows.UI.Colors.Lime);
            }
            if (temp > 205)
            {
                probe5TempData.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                probe5TempLbl.Background = new SolidColorBrush(Windows.UI.Colors.Red);
            }

            probes.TryGetValue("Probe 6", out data);
            if (data == null)
            { data = "disconnected"; temp = 0; probe6TempData.Content = data;
            }
            else
            {
                probe6TempData.Content = data;
                data = data.Remove(data.IndexOf(' '));
                float.TryParse(data, out temp);
            }
            if (temp < 190)
            {
                probe6TempData.Background = new SolidColorBrush(Windows.UI.Colors.DodgerBlue);
                probe6TempLbl.Background = new SolidColorBrush(Windows.UI.Colors.DodgerBlue);
            }
            if (temp >= 190 && temp <= 205)
            {
                probe6TempData.Background = new SolidColorBrush(Windows.UI.Colors.Lime);
                probe6TempLbl.Background = new SolidColorBrush(Windows.UI.Colors.Lime);
            }
            if (temp > 205)
            {
                probe6TempLbl.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                probe6TempData.Background = new SolidColorBrush(Windows.UI.Colors.Red);
            }
        }

        private async void Post_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string status = await HttpUtilities.HttpGet(string.Format("{0}?t={1}",
                    IPInput.Text, tempInput.Text));
            }
            catch
            {
                var dialog = new Windows.UI.Popups.MessageDialog("Invalid Smoker IP Address");
                await dialog.ShowAsync();
                return;
            }
            Get_Button_Click(sender, e);
        }
    }
}
