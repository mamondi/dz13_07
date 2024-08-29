using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;

namespace dz13_07
{
    public partial class MainWindow : Window
    {
        //Початкові параметри пошуку
        private string _selectedSearchEngine = "Google";
        private string _selectedContentType = "Text"; 

        private ObservableCollection<SearchResult> _searchResults = new ObservableCollection<SearchResult>();

        public MainWindow()
        {
            InitializeComponent();
            ResultsItemsControl.ItemsSource = _searchResults;
        }

        private void GoogleChoose_Click(object sender, RoutedEventArgs e)
        {
            _selectedSearchEngine = "Google";
        }

        private void BingChoose_Click(object sender, RoutedEventArgs e)
        {
            _selectedSearchEngine = "Bing";
        }

        private void TextChoose_Click(object sender, RoutedEventArgs e)
        {
            _selectedContentType = "Text";
        }

        private void PictureChoose_Click(object sender, RoutedEventArgs e)
        {
            _selectedContentType = "Picture";
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var query = SearchTextBox.Text;
            if (string.IsNullOrEmpty(query)) return;

            _searchResults.Clear();
            var results = await PerformSearch(query);
            foreach (var result in results)
            {
                _searchResults.Add(result);
            }
        }

        private async Task<ObservableCollection<SearchResult>> PerformSearch(string query)
        {
            var results = new ObservableCollection<SearchResult>();

            string url = _selectedSearchEngine == "Google"
                ? $"https://www.googleapis.com/customsearch/v1?q={query}&key=Гугл_Апі_Токен&cx=СХ_Айді"
                : $"https://api.bing.microsoft.com/v7.0/search?q={query}&Бінг_Апі_Токен";

            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                var json = JObject.Parse(response);

                foreach (var item in json["items"])
                {
                    var title = item["title"]?.ToString();
                    var imageUrl = _selectedContentType == "Picture"
                        ? item["pagemap"]?["cse_image"]?[0]?["src"]?.ToString()
                        : null;

                    results.Add(new SearchResult
                    {
                        Title = title,
                        ImageUrl = imageUrl
                    });
                }
            }

            return results;
        }
    }

    public class SearchResult
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
    }
}
