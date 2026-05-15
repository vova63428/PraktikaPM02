using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using AppDesktop.Models;
using AppDesktop.Services;

namespace AppDesktop
{
    public partial class BatchExecutionWindow : Window
    {
        private readonly OperatorApiService _api;
        private readonly int _batchId;
        private readonly string _batchNumber;
        private BatchStep _currentStep;
        private DispatcherTimer _telemetryTimer;

        public BatchExecutionWindow(OperatorApiService api, int batchId, string batchNumber)
        {
            InitializeComponent();
            _api = api;
            _batchId = batchId;
            _batchNumber = batchNumber;
            LoadBatchData();
        }

        private async void LoadBatchData()
        {
            try
            {
                txtBatchTitle.Text = $"Партия: {_batchNumber}";
                await LoadSteps();
                await LoadCurrentStep();
                StartTelemetry();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
                LoadMockData();
            }
        }

        private async Task LoadSteps()
        {
            try
            {
                var steps = await _api.GetBatchStepsAsync(_batchId);
                if (steps != null && steps.Count > 0)
                {
                    int savedStepNumber = LoadProgress(_batchId);

                    if (savedStepNumber > 0 && savedStepNumber <= steps.Count)
                    {
                        foreach (var step in steps)
                        {
                            if (step.step_number == savedStepNumber)
                                step.status = "in_progress";
                            else if (step.step_number < savedStepNumber)
                                step.status = "completed";
                            else
                                step.status = "pending";
                        }
                    }
                    else if (savedStepNumber > steps.Count)
                    {
                        foreach (var step in steps)
                            step.status = "completed";
                    }
                    else
                    {
                        steps.First().status = "in_progress";
                    }

                    lbSteps.ItemsSource = steps;
                    int completed = steps.Count(s => s.status == "completed");
                    int total = steps.Count;
                    progressSteps.Value = total > 0 ? (double)completed / total * 100 : 0;
                    txtProgress.Text = $"Выполнено {completed} из {total} шагов";

                    _currentStep = steps.FirstOrDefault(s => s.status == "in_progress");
                    if (_currentStep != null) UpdateStepUI();
                }
                else
                {
                    LoadMockSteps();
                }
            }
            catch
            {
                LoadMockSteps();
            }
        }

        private async Task LoadCurrentStep()
        {
            try
            {
                var response = await _api.GetCurrentStepAsync(_batchId);
                if (response == null || response.id == 0)
                {
                    var steps = lbSteps.ItemsSource as List<BatchStep>;
                    if (steps != null && steps.Any(s => s.status != "completed"))
                        LoadMockCurrentStep();
                    else
                        await CompleteBatchAndExit();
                    return;
                }

                _currentStep = response;
                UpdateStepUI();
            }
            catch
            {
                var steps = lbSteps.ItemsSource as List<BatchStep>;
                if (steps != null && steps.Any(s => s.status != "completed"))
                    LoadMockCurrentStep();
                else
                    await CompleteBatchAndExit();
            }
        }

        private async Task CompleteBatchAndExit()
        {
            try
            {
                await _api.CompleteBatchAsync(_batchId);
                await _api.NotifyLabAsync(_batchId);
            }
            catch { }

            ClearProgress(_batchId);
            _telemetryTimer?.Stop();

            var activeWindow = new ActiveBatchesWindow(_api);
            activeWindow.Show();
            Close();
        }

        private void LoadMockData()
        {
            txtBatchTitle.Text = $"Партия: {_batchNumber} (демо-режим)";
            LoadMockSteps();
            StartTelemetry();
        }

        private void LoadMockSteps()
        {
            int savedStepNumber = LoadProgress(_batchId);
            var steps = new List<BatchStep>
            {
                new BatchStep { id = 1, step_number = 1, step_name = "Смешивание компонентов", status = "pending", description = "Загрузка и смешивание сырья", instruction = "Загрузить компоненты согласно рецептуре", planned_temperature = 45, planned_pressure = 1.5m, expected_speed = 300 },
                new BatchStep { id = 2, step_number = 2, step_name = "Выдержка", status = "pending", description = "Выдержка при температуре", instruction = "Поддерживать температуру 60°C", planned_temperature = 60, planned_pressure = 2.0m, expected_speed = 0 },
                new BatchStep { id = 3, step_number = 3, step_name = "Экструзия", status = "pending", description = "Экструдирование массы", instruction = "Подать массу на экструдер", planned_temperature = 80, planned_pressure = 3.0m, expected_speed = 450 },
                new BatchStep { id = 4, step_number = 4, step_name = "Охлаждение", status = "pending", description = "Охлаждение продукта", instruction = "Охладить до комнатной температуры", planned_temperature = 25, planned_pressure = 1.0m, expected_speed = 0 }
            };

            if (savedStepNumber > 0 && savedStepNumber <= steps.Count)
            {
                foreach (var step in steps)
                {
                    if (step.step_number == savedStepNumber)
                        step.status = "in_progress";
                    else if (step.step_number < savedStepNumber)
                        step.status = "completed";
                    else
                        step.status = "pending";
                }
            }
            else
            {
                steps.First().status = "in_progress";
            }

            lbSteps.ItemsSource = steps;
            _currentStep = steps.FirstOrDefault(s => s.status == "in_progress");
            if (_currentStep != null) UpdateStepUI();

            int completed = steps.Count(s => s.status == "completed");
            int total = steps.Count;
            progressSteps.Value = total > 0 ? (double)completed / total * 100 : 0;
            txtProgress.Text = $"Выполнено {completed} из {total} шагов";
        }

        private void LoadMockCurrentStep()
        {
            var steps = lbSteps.ItemsSource as List<BatchStep>;
            if (steps != null)
            {
                _currentStep = steps.FirstOrDefault(s => s.status == "in_progress");
                if (_currentStep == null)
                {
                    _currentStep = steps.FirstOrDefault();
                    if (_currentStep != null) _currentStep.status = "in_progress";
                }
            }

            if (_currentStep == null)
            {
                _currentStep = new BatchStep
                {
                    id = 1,
                    step_number = 1,
                    step_name = "Смешивание компонентов",
                    description = "Загрузка и смешивание сырья",
                    instruction = "Загрузить компоненты согласно рецептуре",
                    status = "in_progress",
                    planned_temperature = 45,
                    planned_pressure = 1.5m,
                    expected_speed = 300
                };
            }
            UpdateStepUI();
        }

        private void UpdateStepUI()
        {
            txtStepName.Text = $"{_currentStep.step_number}. {_currentStep.step_name}";
            txtStepDesc.Text = !string.IsNullOrEmpty(_currentStep.description) ? _currentStep.description : "Нет описания";
            txtStepInstruction.Text = $"Инструкция: {(!string.IsNullOrEmpty(_currentStep.instruction) ? _currentStep.instruction : "Следуйте технологии")}";

            decimal expectedTemp = _currentStep.planned_temperature ?? _currentStep.expected_temperature;
            decimal expectedPressure = _currentStep.planned_pressure ?? _currentStep.expected_pressure;
            decimal expectedSpeed = _currentStep.expected_speed;

            txtExpectedTemp.Text = expectedTemp > 0 ? $"{expectedTemp} °C" : "—";
            txtExpectedPressure.Text = expectedPressure > 0 ? $"{expectedPressure} бар" : "—";
            txtExpectedSpeed.Text = expectedSpeed > 0 ? $"{expectedSpeed} rpm" : "—";

            if (string.IsNullOrWhiteSpace(txtActualTemp.Text) && expectedTemp > 0)
                txtActualTemp.Text = expectedTemp.ToString();
            if (string.IsNullOrWhiteSpace(txtActualPressure.Text) && expectedPressure > 0)
                txtActualPressure.Text = expectedPressure.ToString();
            if (string.IsNullOrWhiteSpace(txtActualSpeed.Text) && expectedSpeed > 0)
                txtActualSpeed.Text = expectedSpeed.ToString();

            btnStartStep.IsEnabled = true;
            btnCompleteStep.IsEnabled = true;
        }

        private void StartTelemetry()
        {
            _telemetryTimer = new DispatcherTimer();
            _telemetryTimer.Interval = TimeSpan.FromSeconds(3);
            _telemetryTimer.Tick += OnTelemetryTick;
            _telemetryTimer.Start();
        }

        private async void OnTelemetryTick(object sender, EventArgs e) => await UpdateTelemetry();

        private async Task UpdateTelemetry()
        {
            try
            {
                var tele = await _api.GetTelemetryAsync(1);
                var mockTele = new EquipmentTelemetry
                {
                    temperature = tele?.temperature ?? 44.5m,
                    pressure = tele?.pressure ?? 1.48m,
                    speed = tele?.speed ?? 298,
                    vibration = tele?.vibration ?? 1.2m,
                    timestamp = DateTime.Now
                };
                UpdateTelemetryUI(mockTele);
            }
            catch
            {
                var mockTele = new EquipmentTelemetry
                {
                    temperature = 44.5m,
                    pressure = 1.48m,
                    speed = 298,
                    vibration = 1.2m,
                    timestamp = DateTime.Now
                };
                UpdateTelemetryUI(mockTele);
            }
        }

        private void UpdateTelemetryUI(EquipmentTelemetry tele)
        {
            txtTeleTemp.Text = $"{tele.temperature:F1} °C";
            txtTelePressure.Text = $"{tele.pressure:F1} бар";
            txtTeleSpeed.Text = $"{tele.speed:F0} rpm";
            txtTeleVibration.Text = $"{tele.vibration:F2} mm/s";
            txtTeleTimestamp.Text = tele.timestamp.ToString("HH:mm:ss");

            if (_currentStep != null)
            {
                decimal expectedTemp = _currentStep.planned_temperature ?? _currentStep.expected_temperature;
                decimal tempDiff = Math.Abs(tele.temperature - expectedTemp);
                txtTeleTemp.Foreground = tempDiff > 5
                    ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red)
                    : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.DarkGreen);
            }
        }

        private decimal GetExpectedTemperature() => _currentStep?.planned_temperature ?? _currentStep?.expected_temperature ?? 0;
        private decimal GetExpectedPressure() => _currentStep?.planned_pressure ?? _currentStep?.expected_pressure ?? 0;

        private string ProgressFilePath => System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AppDesktop", $"batch_{_batchId}_progress.txt");

        private void SaveProgress(int batchId, int stepNumber)
        {
            try
            {
                string dir = System.IO.Path.GetDirectoryName(ProgressFilePath);
                if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
                System.IO.File.WriteAllText(ProgressFilePath, stepNumber.ToString());
            }
            catch { }
        }

        private int LoadProgress(int batchId)
        {
            try
            {
                if (System.IO.File.Exists(ProgressFilePath))
                {
                    string content = System.IO.File.ReadAllText(ProgressFilePath);
                    if (int.TryParse(content, out int step)) return step;
                }
            }
            catch { }
            return 0;
        }

        private void ClearProgress(int batchId)
        {
            try { if (System.IO.File.Exists(ProgressFilePath)) System.IO.File.Delete(ProgressFilePath); } catch { }
        }

        private async void BtnStartStep_Click(object sender, RoutedEventArgs e)
        {
            if (_currentStep == null) return;
            try { await _api.StartStepAsync(_currentStep.id); } catch { }
            _currentStep.status = "in_progress";
            UpdateStepUI();
            MessageBox.Show($"Шаг \"{_currentStep.step_name}\" начат.", "Информация");
        }

        private async void BtnCompleteStep_Click(object sender, RoutedEventArgs e)
        {
            if (_currentStep == null)
            {
                await CompleteBatchAndExit();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtActualTemp.Text))
                txtActualTemp.Text = GetExpectedTemperature().ToString();
            if (string.IsNullOrWhiteSpace(txtActualPressure.Text))
                txtActualPressure.Text = GetExpectedPressure().ToString();
            if (string.IsNullOrWhiteSpace(txtActualSpeed.Text))
                txtActualSpeed.Text = _currentStep.expected_speed.ToString();

            decimal actualTemp = Convert.ToDecimal(txtActualTemp.Text);
            decimal actualPressure = Convert.ToDecimal(txtActualPressure.Text);
            decimal actualSpeed = Convert.ToDecimal(txtActualSpeed.Text);

            // Проверяем отклонения
            decimal expectedTemp = GetExpectedTemperature();
            decimal expectedPressure = GetExpectedPressure();

            bool hasDeviation = false;
            string deviationMessage = "";

            if (Math.Abs(actualTemp - expectedTemp) > 5)
            {
                hasDeviation = true;
                deviationMessage += $"Температура {actualTemp}°C (норма {expectedTemp}°C); ";
            }
            if (Math.Abs(actualPressure - expectedPressure) > 0.3m)
            {
                hasDeviation = true;
                deviationMessage += $"Давление {actualPressure} бар (норма {expectedPressure} бар); ";
            }

            // Показываем блок отклонения
            if (hasDeviation)
            {
                panelDeviation.Visibility = Visibility.Visible;
                txtDeviation.Text = deviationMessage;
            }
            else
            {
                panelDeviation.Visibility = Visibility.Collapsed;
            }

            var actualParams = new
            {
                actual_temperature = actualTemp,
                actual_pressure = actualPressure,
                actual_speed = actualSpeed,
                comment = txtComment.Text,
                has_deviation = hasDeviation,
                deviation_description = deviationMessage
            };

            try { await _api.CompleteStepAsync(_currentStep.id, actualParams); } catch { }

            SaveProgress(_batchId, _currentStep.step_number + 1);
            await SwitchToNextStep();
        }

        private async Task SwitchToNextStep()
        {
            var steps = lbSteps.ItemsSource as List<BatchStep>;
            if (steps == null)
            {
                LoadMockSteps();
                steps = lbSteps.ItemsSource as List<BatchStep>;
            }

            if (steps != null && _currentStep != null)
            {
                var currentStep = steps.FirstOrDefault(s => s.id == _currentStep.id);
                if (currentStep != null) currentStep.status = "completed";

                var nextStep = steps.FirstOrDefault(s => s.status == "pending");

                if (nextStep != null)
                {
                    nextStep.status = "in_progress";
                    _currentStep = nextStep;
                    lbSteps.ItemsSource = null;
                    lbSteps.ItemsSource = steps;

                    int completed = steps.Count(s => s.status == "completed");
                    int total = steps.Count;
                    progressSteps.Value = total > 0 ? (double)completed / total * 100 : 0;
                    txtProgress.Text = $"Выполнено {completed} из {total} шагов";
                    UpdateStepUI();
                    SaveProgress(_batchId, nextStep.step_number);

                    // Скрываем блок отклонения при переходе к следующему шагу
                    panelDeviation.Visibility = Visibility.Collapsed;

                    MessageBox.Show($"Переход к шагу: {nextStep.step_name}", "Информация");
                }
                else
                {
                    await CompleteBatchAndExit();
                }
            }
            else
            {
                await CompleteBatchAndExit();
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            _telemetryTimer?.Stop();
            new ActiveBatchesWindow(_api).Show();
            Close();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            _telemetryTimer?.Stop();
            new MainWindow().Show();
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _telemetryTimer?.Stop();
            base.OnClosed(e);
        }
    }
}