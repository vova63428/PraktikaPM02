using System;
using System.Windows;
using System.Windows.Controls;
using AgroSintez.Services;

namespace AgroSintez.Views
{
    public partial class MonitoringView : Page
    {
        private readonly ApiService _api;

        public MonitoringView(ApiService api)
        {
            InitializeComponent();
            _api = api;
            LoadBatches();
        }

        private async void LoadBatches()
        {
            try
            {
                var batches = await _api.GetActiveBatchesAsync();
                lvBatches.ItemsSource = batches;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private async void LvBatches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var batch = lvBatches.SelectedItem;
            if (batch != null)
            {
                var batchId = (int)batch.GetType().GetProperty("id").GetValue(batch, null);
                txtSelectedBatch.Text = $"Партия {(string)batch.GetType().GetProperty("batch_number").GetValue(batch, null)} - Шаги выполнения";
                var steps = await _api.GetBatchStepsAsync(batchId);
                dgSteps.ItemsSource = steps;
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadBatches();
        }
    }
}