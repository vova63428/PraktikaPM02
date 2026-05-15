using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using AgroSintez.Services;
using AgroSintez.Models;

namespace AgroSintez.Views
{
    public partial class AddBatchDialog : Window
    {
        private readonly ApiService _api;
        public List<ProductionOrder> Orders { get; set; }

        public string BatchNumber => txtBatchNumber.Text;
        public int OrderId => cbOrderId.SelectedItem != null ? (int)cbOrderId.SelectedValue : 0;
        public string Status => (cbStatus.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "planned";

        public AddBatchDialog(ApiService api)
        {
            InitializeComponent();
            _api = api;
            LoadOrders();
        }

        private async void LoadOrders()
        {
            Orders = await _api.GetProductionOrdersAsync();
            cbOrderId.ItemsSource = Orders;
            if (Orders.Count > 0)
                cbOrderId.SelectedIndex = 0;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BatchNumber))
            {
                MessageBox.Show("Введите номер партии", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (OrderId == 0)
            {
                MessageBox.Show("Выберите заказ", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}