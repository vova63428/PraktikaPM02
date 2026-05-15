using System.Windows;
using System.Windows.Controls;

namespace AgroSintez.Views
{
    public partial class AddRecipeDialog : Window
    {
        public string RecipeName => txtName.Text;
        public int ProductId => int.TryParse(txtProductId.Text, out int id) ? id : 1;
        public string Status => (cbStatus.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "draft";

        public AddRecipeDialog()
        {
            InitializeComponent();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RecipeName))
            {
                MessageBox.Show("Введите наименование рецептуры", "Ошибка",
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