using System;
using System.Windows;
using System.Windows.Input;
using LabDesktop.Services;

namespace LabDesktop.Views
{
    public partial class LoginWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly AuthService _authService;

        public LoginWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
            _authService = new AuthService(_apiService);

            txtPassword.Password = "password123";

            txtLogin.KeyDown += (s, e) => { if (e.Key == Key.Enter) BtnLogin_Click(null, null); };
            txtPassword.KeyDown += (s, e) => { if (e.Key == Key.Enter) BtnLogin_Click(null, null); };
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtLogin.Text) || string.IsNullOrEmpty(txtPassword.Password))
            {
                txtError.Text = "Введите логин и пароль";
                return;
            }

            btnLogin.IsEnabled = false;
            txtError.Text = "";

            var success = await _authService.LoginAsync(txtLogin.Text, txtPassword.Password);

            if (success)
            {
                App.CurrentUser = _authService.CurrentUser;
                var mainWindow = new MainWindow(_apiService, _authService);
                mainWindow.Show();
                Close();
            }
            else
            {
                txtError.Text = "Неверный логин, пароль или недостаточно прав";
                btnLogin.IsEnabled = true;
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            Close();
        }
    }
}