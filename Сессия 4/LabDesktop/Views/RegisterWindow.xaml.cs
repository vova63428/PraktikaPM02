using System;
using System.Windows;
using LabDesktop.Services;

namespace LabDesktop.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly ApiService _api;

        public RegisterWindow()
        {
            InitializeComponent();
            _api = new ApiService();
        }

        private async void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string fullName = txtFullName.Text.Trim();
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            // Получаем роль из ComboBox
            string role = "lab technician";
            if (cbRole.SelectedItem is System.Windows.Controls.ComboBoxItem selectedItem)
            {
                role = selectedItem.Tag?.ToString() ?? "lab technician";
            }

            // Проверки
            if (string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Введите логин", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(fullName))
            {
                MessageBox.Show("Введите ФИО", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Пароль должен быть не менее 6 символов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            btnRegister.IsEnabled = false;

            try
            {
                bool success = await _api.RegisterAsync(login, password, fullName, role);

                if (success)
                {
                    MessageBox.Show($"Пользователь {login} успешно зарегистрирован!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Открываем окно входа
                    var loginWindow = new LoginWindow();
                    loginWindow.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка регистрации. Возможно, логин уже занят.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnRegister.IsEnabled = true;
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
    }
}