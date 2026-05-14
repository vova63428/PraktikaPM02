using System;
using System.Windows;
using System.Windows.Controls;
using LabDesktop.Services;

namespace LabDesktop.Views
{
    public partial class RawMaterialBatchesView : Page
    {
        private readonly ApiService _api;

        public RawMaterialBatchesView(ApiService api)
        {
            InitializeComponent();
            _api = api;

            // Настройка русских заголовков
            dgRawMaterials.AutoGeneratingColumn += DgRawMaterials_AutoGeneratingColumn;

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var materials = await _api.GetRawMaterialBatchesForQCAsync();
                dgRawMaterials.ItemsSource = materials;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private void DgRawMaterials_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.Column.Header.ToString())
            {
                case "id":
                    e.Column.Header = "ID";
                    break;
                case "batch_number":
                    e.Column.Header = "Номер партии";
                    break;
                case "raw_material_id":
                    e.Column.Header = "ID сырья";
                    break;
                case "quantity":
                    e.Column.Header = "Количество";
                    break;
                case "unit_of_measure":
                    e.Column.Header = "Ед. изм.";
                    break;
                case "receipt_date":
                    e.Column.Header = "Дата поступления";
                    break;
                case "supplier":
                    e.Column.Header = "Поставщик";
                    break;
                case "lab_status":
                    e.Column.Header = "Статус";
                    break;
                case "created_date":
                    e.Column.Header = "Дата создания";
                    break;
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}