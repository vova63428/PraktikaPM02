using System.Windows;
using System.Windows.Controls;

namespace AgroSintez.Views
{
    public partial class ExtruderSettingsView : Page
    {
        public ExtruderSettingsView()
        {
            InitializeComponent();
        }

        private void LoadSettings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Параметры загружены из базы данных", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Параметры сохранены в профиль", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SendToExtruder_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Параметры отправлены на экструдер", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}