using System;
using System.Windows;
using System.Windows.Controls;
using LabDesktop.Services;

namespace LabDesktop.Views
{
    public partial class DashboardView : Page
    {
        private readonly ApiService _api;

        public DashboardView(ApiService api)
        {
            InitializeComponent();
            _api = api;

            dgRecentTests.AutoGeneratingColumn += DgRecentTests_AutoGeneratingColumn;

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var stats = await _api.GetStatisticsAsync();
                if (stats != null)
                {
                    txtPendingCount.Text = (stats.pendingBatchesCount + stats.pendingRawMaterialsCount).ToString();
                    txtCompletedToday.Text = stats.completedTodayCount.ToString();
                    txtRejectedCount.Text = stats.rejectedThisWeekCount.ToString();
                }

                var history = await _api.GetTestHistoryAsync(DateTime.Now.AddDays(-30), null);
                if (history != null && history.Count > 0)
                {
                    dgRecentTests.ItemsSource = history;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private void DgRecentTests_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.Column.Header.ToString())
            {
                case "id":
                    e.Column.Visibility = Visibility.Collapsed;
                    break;
                case "batch_id":
                    e.Column.Header = "ID партии";
                    e.Column.Width = 80;
                    break;
                case "batch_number":
                    e.Column.Header = "Номер партии";
                    e.Column.Width = 120;
                    break;
                case "parameter_name":
                    e.Column.Header = "Параметр";
                    e.Column.Width = 180;
                    break;
                case "measured_value":
                    e.Column.Header = "Значение";
                    e.Column.Width = 80;
                    break;
                case "standard_value":
                    e.Column.Header = "Норма";
                    e.Column.Width = 100;
                    break;
                case "unit_of_measure":
                    e.Column.Header = "Ед. изм.";
                    e.Column.Width = 70;
                    break;
                case "result":
                    e.Column.Header = "Результат";
                    e.Column.Width = 80;
                    break;
                case "analysis_date":
                    e.Column.Header = "Дата анализа";
                    e.Column.Width = 130;
                    break;
                case "raw_material_batch_id":
                    e.Column.Visibility = Visibility.Collapsed;
                    break;
                case "lab_technician_id":
                    e.Column.Visibility = Visibility.Collapsed;
                    break;
                case "sample_type":
                    e.Column.Visibility = Visibility.Collapsed;
                    break;
                case "decision":
                    e.Column.Visibility = Visibility.Collapsed;
                    break;
                case "lab_comment":
                    e.Column.Visibility = Visibility.Collapsed;
                    break;
                default:
                    e.Column.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}