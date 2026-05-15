using System.Windows;
using LabDesktop.Services;
using LabDesktop.Views;

namespace LabDesktop
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

            Title = $"АгроСинтез - Лаборатория ({_auth.CurrentUser?.FullName})";

            OpenDashboard_Click(null, null);
        }

        private void OpenDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardView(_api));
        }

        private void OpenBatches_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new BatchesListView(_api));
        }

        private void OpenRawMaterials_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RawMaterialBatchesView(_api));
        }

        private void OpenHistory_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TestHistoryView(_api));
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            _auth.Logout();
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }

        // Метод для обновления истории из других вкладок
        public void RefreshHistoryView()
        {
            if (MainFrame.Content is TestHistoryView historyView)
            {
                historyView.RefreshData();
            }
        }
    }
}