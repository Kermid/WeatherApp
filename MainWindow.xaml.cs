using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Windows.Threading;
using Newtonsoft.Json;

namespace WeatherApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //weather api key
        private readonly string apiKey = "";
        private string cityName;

        private DateTime _now;

        public DateTime Now
        { get { return _now; }
          set 
            { 
                _now = value; 
                OnPropertyChanged(nameof(Now));
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            DataContext= this;

            

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (sender, args) =>
            {
                Now = DateTime.Now;
            };
            timer.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
                this.DragMove();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           this.Close();
        }

        private async void btnGetWeather_Click(object sender, RoutedEventArgs e)
        {
            cityName = txtCityName.Text.Trim();
            if(string.IsNullOrEmpty(cityName))
            {
                MessageBox.Show("Enter a city name.");
                    return;
            }

            string apiURL = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={cityName}";

            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(apiURL);
                request.Method = "GET";

                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream)) 
                        {
                            string jsonResponse = reader.ReadToEnd();  
                            WeatherData weatherData = JsonConvert.DeserializeObject<WeatherData>(jsonResponse);
                            DisplayWeatherData(weatherData);
                        }
                    }
                }
            }
            catch(WebException ex)
            {
                MessageBox.Show("An error occured while fetching data: " + ex.Message);
            }

            
        }

        private void DisplayWeatherData(WeatherData? weatherData)
        {
            if (weatherData == null)
            {
                MessageBox.Show("Weather data is not available.");
                return;
            }
            lblCityName.Content = weatherData.Location.Name;
            lblTemperature.Content = weatherData.Current.TempC + "C";
            lblCondition.Content= weatherData.Current.Condition.Text;
            lblHumidity.Content = weatherData.Current.Humidity + "%";

            BitmapImage weatherIcon= new BitmapImage();
            weatherIcon.BeginInit();
            weatherIcon.UriSource = new Uri("http:" + weatherData.Current.Condition.Icon);
            weatherIcon.EndInit();
            imageWeatherIcon.Source = weatherIcon;

            lblWindSpeed.Content = weatherData.Current.WindKph + "km/h";
        }

      
    }
}
