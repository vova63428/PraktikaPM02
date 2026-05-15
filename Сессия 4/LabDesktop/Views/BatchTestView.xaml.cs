using System;
using System.Windows;
using LabDesktop.Services;

namespace LabDesktop.Views
{
    public partial class BatchTestView : Window
    {
        private readonly ApiService _api;

        public BatchTestView(ApiService api)
        {
            InitializeComponent();
            _api = api;
            LoadBatches();
        }

        private async void LoadBatches()
        {
            try
            {
                var batches = await _api.GetBatchesForQCAsync();
                cbBatchNumber.ItemsSource = batches;

                if (batches != null && batches.Count > 0)
                {
                    cbBatchNumber.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("Нет доступных партий для контроля");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки партий: {ex.Message}");
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cbBatchNumber.SelectedItem == null)
            {
                MessageBox.Show("Выберите партию");
                return;
            }

            dynamic selectedBatch = cbBatchNumber.SelectedItem;
            int batchId = selectedBatch.id;

            if (string.IsNullOrEmpty(txtParameterName.Text))
            {
                MessageBox.Show("Введите наименование параметра");
                return;
            }

            if (string.IsNullOrEmpty(txtStandardValue.Text))
            {
                MessageBox.Show("Введите нормативное значение");
                return;
            }

            if (string.IsNullOrEmpty(txtMeasuredValue.Text))
            {
                MessageBox.Show("Введите измеренное значение");
                return;
            }

            try
            {
                string measuredValueStr = txtMeasuredValue.Text.Replace(',', '.');
                if (!decimal.TryParse(measuredValueStr, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal measuredValue))
                {
                    MessageBox.Show("Некорректное значение. Введите число");
                    return;
                }

                // Вычисляем результат
                string result = "passed";
                if (decimal.TryParse(txtStandardValue.Text.Replace("≥", "").Replace("≤", "").Replace(">", "").Replace("<", ""), out decimal standardNum))
                {
                    if (txtStandardValue.Text.Contains("≥"))
                        result = measuredValue >= standardNum ? "passed" : "fail";
                    else if (txtStandardValue.Text.Contains("≤"))
                        result = measuredValue <= standardNum ? "passed" : "fail";
                    else
                        result = measuredValue == standardNum ? "passed" : "fail";
                }

                var testData = new
                {
                    BatchId = batchId,
                    LabTechnicianId = App.CurrentUser?.Id ?? 1,
                    LabTechnicianName = App.CurrentUser?.FullName ?? "Неизвестно",
                    SampleType = "finished_product",
                    ParameterName = txtParameterName.Text,
                    MeasuredValue = measuredValue,
                    StandardValue = txtStandardValue.Text,
                    UnitOfMeasure = txtUnitOfMeasure.Text,
                    LabComment = txtComment.Text,
                    Result = result,
                    Decision = ""
                };

                var resultTest = await _api.CreateBatchTestAsync(testData);

                if (resultTest != null && resultTest.id > 0)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении теста");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}