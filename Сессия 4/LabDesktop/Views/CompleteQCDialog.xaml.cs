using System.Windows;
using System.Windows.Controls;

namespace LabDesktop.Views
{
    public partial class CompleteQCDialog : Window
    {
        public string Decision { get; private set; }
        public string Comment => txtComment.Text;

        public CompleteQCDialog(string message)
        {
            InitializeComponent();
            txtMessage.Text = message;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = cbDecision.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                Decision = selectedItem.Tag?.ToString() ?? "approved";
            }
            else
            {
                Decision = "approved";
            }

            if (Decision == "rejected" && string.IsNullOrWhiteSpace(Comment))
            {
                MessageBox.Show("При браковке партии необходимо указать причину", "Ошибка",
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