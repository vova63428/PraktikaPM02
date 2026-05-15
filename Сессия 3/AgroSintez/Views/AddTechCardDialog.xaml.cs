using System.Windows;
using System.Windows.Controls;

namespace AgroSintez.Views
{
    public partial class AddTechCardDialog : Window
    {
        public int ProductId => int.TryParse(txtProductId.Text, out int id) ? id : 1;
        public string Status => (cbStatus.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "draft";

        public AddTechCardDialog()
        {
            InitializeComponent();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (ProductId <= 0)
            {
                MessageBox.Show("Введите корректный ID продукта", "Ошибка",
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