using System;
using System.Windows;
using AppDesktop.Services;

namespace AppDesktop
{
    public partial class MainWindow : Window
    {
        private readonly OperatorApiService _api;

        public MainWindow()
        {
            InitializeComponent();
            _api = new OperatorApiService();

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

            try
            {
                bool success = await _api.LoginAsync(txtLogin.Text, txtPassword.Password);

                if (success)
                {
                    var activeWindow = new ActiveBatchesWindow(_api);
                    activeWindow.Show();
                    Close();
                }
                else
                {
                    txtError.Text = "Неверный логин, пароль или недостаточно прав";
                    btnLogin.IsEnabled = true;
                }
            }
            catch
            {
                txtError.Text = "Ошибка подключения к серверу";
                btnLogin.IsEnabled = true;
            }
        }
    }
}