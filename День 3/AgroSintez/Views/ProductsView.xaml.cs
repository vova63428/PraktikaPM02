using System;
using System.Windows;
using System.Windows.Controls;
using AgroSintez.Services;

namespace AgroSintez.Views
{
    public partial class ProductsView : Page
    {
        private readonly ApiService _api;

        public ProductsView()
        {
            InitializeComponent();
            _api = new ApiService();
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var products = await _api.GetProductsAsync();
                dgProducts.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}