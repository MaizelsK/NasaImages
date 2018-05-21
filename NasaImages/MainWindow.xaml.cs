using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace NasaImages
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DatePicker.SelectedDate = DateTime.Today;
            Progress.IsIndeterminate = true;
            Progress.Visibility = Visibility.Hidden;
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (DatePicker.SelectedDate > DateTime.Today)
            {
                MessageBox.Show("Нету данных по выбранно дате!");
                return;
            }

            Progress.Visibility = Visibility.Visible;
            string selectedDate = DatePicker.SelectedDate.Value.ToString("yyyy-MM-dd");

            NasaImageResponse file = await LoadImage(selectedDate);

            if (file.MediaType != "video")
            {
                videoHost.Visibility = Visibility.Hidden;
                imageHost.Visibility = Visibility.Visible;

                ImageText.Text = file.Title;
                imageHost.Source = new BitmapImage(new Uri(file.Hdurl));
            }
            else
            {
                imageHost.Visibility = Visibility.Hidden;
                videoHost.Visibility = Visibility.Visible;

                videoHost.Address = file.Url;
            }

            Progress.Visibility = Visibility.Hidden;
        }

        private Task<NasaImageResponse> LoadImage(string selectedDate)
        {
            return Task.Run(() =>
            {
                WebClient client = new WebClient();

                string address = "https://api.nasa.gov/planetary/apod?" +
                "date=" + selectedDate +
                "&hd=true&api_key=7us9cTuYhzECs704EuxcMcNH0K1eTo40jRNiNjHN";

                using (Stream stream = client.OpenRead(address))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string data = reader.ReadToEnd();
                        NasaImageResponse response = JsonConvert.DeserializeObject<NasaImageResponse>(data);

                        return response;
                    }
                }
            });
        }
    }
}
