using System;
using System.Windows;
using System.Windows.Controls;
using AgroSintez.Services;

namespace AgroSintez.Views
{
    public partial class DeviationsView : Page
    {
        private readonly ApiService _api;

        public DeviationsView(ApiService api)
        {
            InitializeComponent();
            _api = api;

            // Отключаем автоматическую генерацию колонок
            dgDeviations.AutoGenerateColumns = false;

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var deviations = await _api.GetDeviationsAsync();
                dgDeviations.ItemsSource = deviations;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}