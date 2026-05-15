using System.Windows;
using AgroSintez.Services;
using AgroSintez.Views;

namespace AgroSintez
{
    public partial class MainWindow : Window
    {
        private readonly ApiService _api;
        private readonly AuthService _auth;

        public MainWindow(ApiService api, AuthService auth)
        {
            InitializeComponent();
            _api = api;
            _auth = auth;

            Title = $"АгроСинтез - Модуль технолога ({_auth.CurrentUser?.FullName})";

            OpenProducts_Click(null, null);
        }

        private void OpenProducts_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductsView());
        }

        private void OpenRecipes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RecipesView(_api));
        }

        // НОВЫЙ МЕТОД
        private void OpenTechCards_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TechCardsView(_api));
        }

        private void OpenOrders_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductionOrdersView(_api));
        }

        private void OpenBatches_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductionBatchesView(_api));
        }

        private void OpenMonitoring_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MonitoringView(_api));
        }

        private void OpenExtruder_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ExtruderSettingsView());
        }

        private void OpenDeviations_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DeviationsView(_api));
        }

        private void OpenReports_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ReportsView(_api));
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            _auth.Logout();
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
    }
}