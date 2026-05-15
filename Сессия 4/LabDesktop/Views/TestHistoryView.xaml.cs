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
    public partial class TestHistoryView : Page
    {
        private readonly ApiService _api;
        private int _selectedBatchId;
        private string _selectedBatchNumber;

        public TestHistoryView(ApiService api)
        {
            InitializeComponent();
            _api = api;

            dgHistory.AutoGeneratingColumn += DgHistory_AutoGeneratingColumn;
            dgBatchTests.AutoGeneratingColumn += DgBatchTests_AutoGeneratingColumn;

            LoadData();
        }

        public async void RefreshData()
        {
            try
            {
                var history = await _api.GetTestHistoryAsync(null, null);

                if (history != null)
                {
                    foreach (var test in history)
                    {
                        if (test.result == "passed" || test.result == "pass")
                            test.result = "Соответствует";
                        else if (test.result == "fail")
                            test.result = "Не соответствует";

                        if (test.decision == "approved")
                            test.decision = "Допущена";
                        else if (test.decision == "rejected")
                            test.decision = "Забракована";
                        else
                            test.decision = "";
                    }
                }

                dgHistory.ItemsSource = history;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления истории: {ex.Message}");
            }
        }

        private async void LoadData()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var history = await _api.GetTestHistoryAsync(null, null);

                if (history != null)
                {
                    foreach (var test in history)
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
                        else if (string.IsNullOrEmpty(test.decision))
                            test.decision = "";
                    }
                }

                dgHistory.ItemsSource = history;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка: {ex.Message}");
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void DgHistory_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.Column.Header.ToString())
            {
                case "id":
                    e.Column.Header = "ID";
                    e.Column.Width = 50;
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
                    e.Column.Width = 100;
                    break;
                case "standard_value":
                    e.Column.Header = "Норма";
                    e.Column.Width = 120;
                    break;
                case "unit_of_measure":
                    e.Column.Header = "Ед. изм.";
                    e.Column.Width = 80;
                    break;
                case "result":
                    e.Column.Header = "Результат";
                    e.Column.Width = 100;
                    break;
                case "decision":
                    e.Column.Header = "Решение";
                    e.Column.Width = 100;
                    break;
                case "analysis_date":
                    e.Column.Header = "Дата анализа";
                    e.Column.Width = 130;
                    break;
                default:
                    e.Column.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void DgBatchTests_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.Column.Header.ToString())
            {
                case "id":
                case "batch_id":
                case "raw_material_batch_id":
                case "lab_technician_id":
                case "sample_type":
                case "lab_comment":
                    e.Column.Visibility = Visibility.Collapsed;
                    break;
                case "parameter_name":
                    e.Column.Header = "Параметр";
                    e.Column.Width = 180;
                    break;
                case "measured_value":
                    e.Column.Header = "Значение";
                    e.Column.Width = 100;
                    break;
                case "standard_value":
                    e.Column.Header = "Норма";
                    e.Column.Width = 120;
                    break;
                case "unit_of_measure":
                    e.Column.Header = "Ед. изм.";
                    e.Column.Width = 80;
                    break;
                case "result":
                    e.Column.Header = "Результат";
                    e.Column.Width = 100;
                    break;
                case "analysis_date":
                    e.Column.Header = "Дата анализа";
                    e.Column.Width = 130;
                    break;
                case "decision":
                    e.Column.Header = "Решение";
                    e.Column.Width = 100;
                    break;
                default:
                    e.Column.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private async void DgHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dgHistory.SelectedItem == null)
                {
                    panelDetails.Visibility = Visibility.Collapsed;
                    return;
                }

                dynamic selectedTest = dgHistory.SelectedItem;
                var type = selectedTest.GetType();

                _selectedBatchId = Convert.ToInt32(type.GetProperty("batch_id")?.GetValue(selectedTest, null) ?? 0);
                _selectedBatchNumber = type.GetProperty("batch_number")?.GetValue(selectedTest, null)?.ToString() ?? "Неизвестно";

                txtBatchTitle.Text = $"Партия {_selectedBatchNumber} - Все испытания";

                var tests = await _api.GetBatchTestsAsync(_selectedBatchId);

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
                    dgBatchTests.ItemsSource = tests;
                    txtBatchTitle.Text = $"Партия {_selectedBatchNumber} - Результаты испытаний ({tests.Count} тестов)";
                }
                else
                {
                    dgBatchTests.ItemsSource = null;
                    txtBatchTitle.Text = $"Партия {_selectedBatchNumber} - Результаты испытаний (нет данных)";
                }

                panelDetails.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var history = await _api.GetTestHistoryAsync(dpFrom.SelectedDate, dpTo.SelectedDate);

                if (history != null)
                {
                    foreach (var test in history)
                    {
                        if (test.result == "passed" || test.result == "pass")
                            test.result = "Соответствует";
                        else if (test.result == "fail")
                            test.result = "Не соответствует";

                        if (test.decision == "approved")
                            test.decision = "Допущена";
                        else if (test.decision == "rejected")
                            test.decision = "Забракована";
                    }
                }

                dgHistory.ItemsSource = history;
                panelDetails.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            panelDetails.Visibility = Visibility.Collapsed;
        }

        // ==================== ФОРМИРОВАНИЕ ПРОТОКОЛА ====================
        private async void CreateProtocol_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgHistory.SelectedItem == null)
                {
                    MessageBox.Show("Выберите тест из истории для формирования протокола", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                dynamic selected = dgHistory.SelectedItem;
                int batchId = selected.batch_id;
                string batchNumber = selected.batch_number;

                var tests = await _api.GetBatchTestsAsync(batchId);

                if (tests == null || tests.Count == 0)
                {
                    MessageBox.Show("Нет данных для формирования протокола", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string protocol = GenerateProtocol(batchNumber, batchId, tests);
                ShowProtocolWindow(protocol, batchNumber);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании протокола: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GenerateProtocol(string batchNumber, int batchId, List<LabTest> tests)
        {
            string protocol = "";

            protocol += "╔══════════════════════════════════════════════════════════════════╗\n";
            protocol += "║                  ПРОТОКОЛ ЛАБОРАТОРНЫХ ИСПЫТАНИЙ                 ║\n";
            protocol += "╚══════════════════════════════════════════════════════════════════╝\n\n";

            protocol += $"Номер партии: {batchNumber}\n";
            protocol += $"ID партии: {batchId}\n";
            protocol += $"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm:ss}\n";

            // Берем ФИО лаборанта из первого теста (реальный лаборант, который создавал тесты)
            if (tests != null && tests.Count > 0 && !string.IsNullOrEmpty(tests.First().lab_technician_name))
            {
                protocol += $"Лаборант: {tests.First().lab_technician_name}\n";
            }
            else
            {
                protocol += $"Лаборант: {App.CurrentUser?.FullName ?? "Неизвестно"}\n";
            }

            protocol += "\n" + new string('─', 60) + "\n\n";

            protocol += "РЕЗУЛЬТАТЫ ИСПЫТАНИЙ:\n\n";
            protocol += string.Format("{0,-30} {1,-12} {2,-12} {3,-10} {4,-12}\n",
                "Параметр", "Значение", "Норма", "Ед.изм.", "Результат");
            protocol += new string('─', 80) + "\n";

            foreach (var test in tests)
            {
                string resultText = "";
                if (test.result == "passed" || test.result == "Соответствует")
                    resultText = "✓ Соответствует";
                else if (test.result == "fail" || test.result == "Не соответствует")
                    resultText = "✗ НЕ СООТВЕТСТВУЕТ";
                else
                    resultText = "● В ожидании";

                string paramName = test.parameter_name ?? "";
                if (paramName.Length > 28) paramName = paramName.Substring(0, 25) + "...";

                protocol += string.Format("{0,-30} {1,-12} {2,-12} {3,-10} {4,-12}\n",
                    paramName,
                    test.measured_value?.ToString() ?? "—",
                    test.standard_value ?? "—",
                    test.unit_of_measure ?? "—",
                    resultText);
            }

            protocol += "\n" + new string('─', 60) + "\n\n";

            bool hasDecision = tests.Any(t => t.decision == "approved" || t.decision == "rejected");
            if (hasDecision)
            {
                var firstDecision = tests.FirstOrDefault(t => t.decision == "approved" || t.decision == "rejected");
                if (firstDecision != null)
                {
                    string decisionText = firstDecision.decision == "approved" ? "ДОПУЩЕНА" : "ЗАБРАКОВАНА";
                    protocol += $"ИТОГОВОЕ РЕШЕНИЕ: {decisionText}\n";
                }
            }
            else
            {
                protocol += "ИТОГОВОЕ РЕШЕНИЕ: НЕ ПРИНЯТО (контроль не завершён)\n";
            }

            var comments = tests.Where(t => !string.IsNullOrEmpty(t.lab_comment)).Select(t => t.lab_comment).Distinct();
            if (comments.Any())
            {
                protocol += "\nКОММЕНТАРИИ:\n";
                foreach (var comment in comments)
                {
                    protocol += $"  • {comment}\n";
                }
            }

            protocol += "\n" + new string('─', 60) + "\n";
            protocol += $"Подпись лаборанта: __________________\n";
            protocol += $"Дата: {DateTime.Now:dd.MM.yyyy}";

            return protocol;
        }

        private void ShowProtocolWindow(string protocol, string batchNumber)
        {
            var window = new Window
            {
                Title = $"Протокол испытаний - Партия {batchNumber}",
                Width = 850,
                Height = 650,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this)
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var title = new TextBlock
            {
                Text = $"ПРОТОКОЛ ЛАБОРАТОРНЫХ ИСПЫТАНИЙ\nПартия {batchNumber}",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(10)
            };
            Grid.SetRow(title, 0);

            var textBox = new TextBox
            {
                Text = protocol,
                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10),
                IsReadOnly = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            Grid.SetRow(textBox, 1);

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(10) };
            var closeBtn = new Button { Content = "Закрыть", Width = 100, Height = 30, Margin = new Thickness(5) };
            closeBtn.Click += (s, args) => window.Close();
            buttonPanel.Children.Add(closeBtn);

            Grid.SetRow(buttonPanel, 2);

            grid.Children.Add(title);
            grid.Children.Add(textBox);
            grid.Children.Add(buttonPanel);

            window.Content = grid;
            window.ShowDialog();
        }
    }
}