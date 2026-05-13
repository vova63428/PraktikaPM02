using System;
using System.Windows;
using System.Windows.Controls;
using AgroSintez.Services;

namespace AgroSintez.Views
{
    public partial class ProductionBatchesView : Page
    {
        private readonly ApiService _api;

        public ProductionBatchesView(ApiService api)
        {
            InitializeComponent();
            _api = api;
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var batches = await _api.GetProductionBatchesAsync();
                dgBatches.ItemsSource = batches;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки: {ex.Message}");
            }
        }

        private async void AddBatch_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddBatchDialog(_api);
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var result = await _api.CreateProductionBatchAsync(new
                    {
                        batchNumber = dialog.BatchNumber,
                        orderId = dialog.OrderId,
                        status = dialog.Status
                    });

                    if (result != null)
                    {
                        MessageBox.Show($"Партия \"{dialog.BatchNumber}\" успешно создана!",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при создании партии", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}