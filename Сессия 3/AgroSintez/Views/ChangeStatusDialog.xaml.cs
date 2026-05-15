using System.Windows;
using System.Windows.Controls;

namespace AgroSintez.Views
{
    public partial class ChangeStatusDialog : Window
    {
        public string NewStatus => (cbStatus.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "draft";

        public ChangeStatusDialog(string recipeName, string currentStatus)
        {
            InitializeComponent();
            txtRecipeName.Text = recipeName;

            // Выбираем текущий статус в комбобоксе
            foreach (ComboBoxItem item in cbStatus.Items)
            {
                if (item.Tag.ToString() == currentStatus)
                {
                    item.IsSelected = true;
                    break;
                }
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
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