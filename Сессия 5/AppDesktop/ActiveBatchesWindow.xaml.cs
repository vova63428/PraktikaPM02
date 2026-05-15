using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using AppDesktop.Models;
using AppDesktop.Services;
using System.Threading.Tasks;
namespace AppDesktop
{
    public partial class ActiveBatchesWindow : Window
    {
        private readonly OperatorApiService _api;

        public ActiveBatchesWindow(OperatorApiService api)
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

                if (batches == null || batches.Count == 0)
                {
                    
                    batches = GetMockBatches();
                }

                dgBatches.ItemsSource = batches;

                if (batches.Count == 0)
                {
                    MessageBox.Show("Нет активных партий для выполнения", "Информация");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
               
                dgBatches.ItemsSource = GetMockBatches();
            }
        }

        private List<ProductionBatch> GetMockBatches()
        {
            return new List<ProductionBatch>
            {
                new ProductionBatch { id = 1, batch_number = "B-2025-001", product_name = "Гербицид Торнадо", line_number = 1, status = "in_progress", current_step_number = 1, total_steps = 4 },
                new ProductionBatch { id = 2, batch_number = "B-2025-002", product_name = "Инсектицид Бэтрайдер", line_number = 2, status = "planned", current_step_number = 0, total_steps = 3 },
                new ProductionBatch { id = 3, batch_number = "B-2025-003", product_name = "Фунгицид АгроФорт", line_number = 1, status = "in_progress", current_step_number = 2, total_steps = 5 }
            };
        }

        private async void DgBatches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgBatches.SelectedItem == null) return;

            try
            {
                dynamic selected = dgBatches.SelectedItem;
                string status = selected.status;
                int batchId = selected.id;
                string batchNumber = selected.batch_number;

                if (status == "completed")
                {
                    MessageBox.Show($"Партия {batchNumber} уже завершена. Выполнение невозможно.", "Доступ запрещён",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    dgBatches.SelectedItem = null;
                    return;
                }

              
                var execWindow = new BatchExecutionWindow(_api, batchId, batchNumber);
                execWindow.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadBatches();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}