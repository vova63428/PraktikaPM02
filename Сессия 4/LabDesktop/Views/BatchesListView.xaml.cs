using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using LabDesktop.Services;
using LabDesktop.Models;

namespace LabDesktop.Views
{
    public partial class BatchesListView : Page
    {
        private readonly ApiService _api;
        private int _currentBatchId;
        private string _currentBatchNumber;
        private HashSet<int> _completedBatchIds = new HashSet<int>(); // ДОБАВЛЕНО

        public BatchesListView(ApiService api)
        {
            InitializeComponent();
            _api = api;

            dgPendingBatches.AutoGeneratingColumn += DgBatches_AutoGeneratingColumn;
            dgCompletedBatches.AutoGeneratingColumn += DgBatches_AutoGeneratingColumn;
            dgTests.AutoGeneratingColumn += DgTests_AutoGeneratingColumn;

            _ = LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                var history = await _api.GetTestHistoryAsync(null, null);

                var allBatchesFromHistory = new Dictionary<int, string>();
                var batchesWithDecision = new HashSet<int>();

                if (history != null)
                {
                    foreach (var test in history)
                    {
                        if (test.batch_id.HasValue)
                        {
                            if (!allBatchesFromHistory.ContainsKey(test.batch_id.Value))
                            {
                                allBatchesFromHistory.Add(test.batch_id.Value, test.batch_number ?? "Неизвестно");
                            }

                            if (test.decision == "approved" || test.decision == "rejected")
                            {
                                batchesWithDecision.Add(test.batch_id.Value);
                            }
                        }
                    }
                }

                var pendingBatches = await _api.GetBatchesForQCAsync();
                var pendingList = new List<dynamic>();
                var completedList = new List<dynamic>();

                if (pendingBatches != null)
                {
                    foreach (var batch in pendingBatches)
                    {
                        int batchId = Convert.ToInt32(batch.id);

                        // Пропускаем если уже завершена в этой сессии
                        if (_completedBatchIds.Contains(batchId))
                            continue;

                        if (allBatchesFromHistory.ContainsKey(batchId))
                        {
                            if (!batchesWithDecision.Contains(batchId))
                            {
                                completedList.Add(batch);
                            }
                        }
                        else
                        {
                            pendingList.Add(batch);
                        }
                    }
                }

                foreach (var batch in allBatchesFromHistory)
                {
                    int batchId = batch.Key;
                    string batchNumber = batch.Value;

                    if (_completedBatchIds.Contains(batchId))
                        continue;

                    bool existsInPending = pendingList.Any(p => Convert.ToInt32(p.id) == batchId);

                    if (!existsInPending)
                    {
                        if (!batchesWithDecision.Contains(batchId))
                        {
                            var fakeBatch = new { id = batchId, batch_number = batchNumber };
                            completedList.Add(fakeBatch);
                        }
                    }
                }

                dgPendingBatches.ItemsSource = pendingList;
                dgCompletedBatches.ItemsSource = completedList;

                if (pendingList.Count == 0 && completedList.Count == 0)
                {
                    txtNoSelection.Text = "Нет партий";
                    txtNoSelection.Visibility = Visibility.Visible;
                }
                else
                {
                    txtNoSelection.Text = $"👈 Ожидают контроля: {pendingList.Count} | С тестами: {completedList.Count}";
                    txtNoSelection.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void DgBatches_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string header = e.Column.Header.ToString();

            if (header == "id")
            {
                e.Column.Header = "ID";
                e.Column.Width = 50;
            }
            else if (header == "batch_number")
            {
                e.Column.Header = "Номер партии";
                e.Column.Width = 200;
            }
            else
            {
                e.Column.Visibility = Visibility.Collapsed;
            }
        }

        private void DgTests_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string header = e.Column.Header.ToString();

            if (header == "id" || header == "batch_id" || header == "raw_material_batch_id" ||
                header == "lab_technician_id" || header == "sample_type" || header == "lab_comment")
            {
                e.Column.Visibility = Visibility.Collapsed;
            }
            else if (header == "parameter_name")
            {
                e.Column.Header = "Параметр";
                e.Column.Width = 180;
            }
            else if (header == "measured_value")
            {
                e.Column.Header = "Значение";
                e.Column.Width = 100;
            }
            else if (header == "standard_value")
            {
                e.Column.Header = "Норма";
                e.Column.Width = 120;
            }
            else if (header == "unit_of_measure")
            {
                e.Column.Header = "Ед. изм.";
                e.Column.Width = 80;
            }
            else if (header == "result")
            {
                e.Column.Header = "Результат";
                e.Column.Width = 100;
            }
            else if (header == "analysis_date")
            {
                e.Column.Header = "Дата анализа";
                e.Column.Width = 130;
            }
            else if (header == "decision")
            {
                e.Column.Header = "Решение";
                e.Column.Width = 100;
            }
            else
            {
                e.Column.Visibility = Visibility.Collapsed;
            }
        }

        private async void DgPendingBatches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgPendingBatches.SelectedItem == null) return;
            dgCompletedBatches.SelectedItem = null;
            await LoadBatchTests(dgPendingBatches.SelectedItem);
        }

        private async void DgCompletedBatches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCompletedBatches.SelectedItem == null) return;
            dgPendingBatches.SelectedItem = null;
            await LoadBatchTests(dgCompletedBatches.SelectedItem);
        }

        private async Task LoadBatchTests(dynamic selectedBatch)
        {
            try
            {
                _currentBatchId = Convert.ToInt32(selectedBatch.id);
                _currentBatchNumber = selectedBatch.batch_number.ToString();

                txtBatchTitle.Text = $"Партия {_currentBatchNumber} - Результаты испытаний";

                var tests = await _api.GetBatchTestsAsync(_currentBatchId);

                if (tests != null && tests.Count > 0)
                {
                    foreach (var test in tests)
                    {
                        if (test.result == "passed" || test.result == "pass")
                            test.result = "Соответствует";
                        else if (test.result == "fail")
                            test.result = "Не соответствует";
                        else if (test.result == "pending")
                            test.result = "В ожидании";

                        if (test.decision == "approved")
                            test.decision = "Допущена";
                        else if (test.decision == "rejected")
                            test.decision = "Забракована";
                        else
                            test.decision = "";
                    }
                    dgTests.ItemsSource = tests;
                    txtBatchTitle.Text = $"Партия {_currentBatchNumber} - Результаты испытаний ({tests.Count} тестов)";
                }
                else
                {
                    dgTests.ItemsSource = null;
                    txtBatchTitle.Text = $"Партия {_currentBatchNumber} - Результаты испытаний (нет данных)";
                }

                panelDetails.Visibility = Visibility.Visible;
                txtNoSelection.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async void AddTest_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBatchId == 0)
            {
                MessageBox.Show("Сначала выберите партию");
                return;
            }

            try
            {
                var testWindow = new BatchTestView(_api);
                testWindow.Owner = Window.GetWindow(this);

                if (testWindow.ShowDialog() == true)
                {
                    await LoadData();
                    var tests = await _api.GetBatchTestsAsync(_currentBatchId);
                    if (tests != null && tests.Count > 0)
                    {
                        dgTests.ItemsSource = tests;
                    }
                    MessageBox.Show("Тест успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async void CompleteQC_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBatchId == 0)
            {
                MessageBox.Show("Выберите партию");
                return;
            }

            try
            {
                var tests = await _api.GetBatchTestsAsync(_currentBatchId);
                if (tests == null || tests.Count == 0)
                {
                    MessageBox.Show("Сначала добавьте результаты испытаний", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                bool hasPending = false;
                foreach (var test in tests)
                {
                    string result = test.result ?? "";
                    if (result == "pending" || result == "В ожидании")
                    {
                        hasPending = true;
                        break;
                    }
                }

                if (hasPending)
                {
                    MessageBox.Show("Не все испытания завершены. Дождитесь результатов.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var dialog = new CompleteQCDialog("Выберите решение по партии:");
                dialog.Owner = Window.GetWindow(this);

                if (dialog.ShowDialog() == true)
                {
                    string comment = dialog.Comment;
                    if (string.IsNullOrWhiteSpace(comment))
                    {
                        comment = dialog.Decision == "approved"
                            ? "Партия соответствует требованиям качества"
                            : "Партия забракована";
                    }

                    bool result = await _api.CompleteBatchQCAsync(_currentBatchId, dialog.Decision, comment);

                    if (result)
                    {
                        string resultMsg = dialog.Decision == "approved" ? "ДОПУЩЕНА" : "ЗАБРАКОВАНА";
                        MessageBox.Show($"Партия {_currentBatchNumber} {resultMsg}!\nКомментарий: {comment}", "Контроль завершён",
                            MessageBoxButton.OK, dialog.Decision == "approved" ? MessageBoxImage.Information : MessageBoxImage.Warning);

                        // Добавляем в список завершённых
                        _completedBatchIds.Add(_currentBatchId);

                        // Обновляем данные
                        await LoadData();

                        // Обновляем историю
                        var mainWindow = Application.Current.MainWindow as MainWindow;
                        mainWindow?.RefreshHistoryView();

                        panelDetails.Visibility = Visibility.Collapsed;
                        _currentBatchId = 0;
                        txtNoSelection.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при завершении контроля", "Ошибка");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            // При ручном обновлении очищаем список завершённых
            _completedBatchIds.Clear();
            await LoadData();
            panelDetails.Visibility = Visibility.Collapsed;
            _currentBatchId = 0;
            txtNoSelection.Visibility = Visibility.Visible;
        }
    }
}