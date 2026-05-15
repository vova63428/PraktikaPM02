using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using AgroSintez.Services;

namespace AgroSintez.Views
{
    public partial class LoginWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly AuthService _authService;
        private readonly CaptchaService _captchaService;
        private string _currentCaptchaCode;

        public LoginWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
            _authService = new AuthService(_apiService);
            _captchaService = new CaptchaService();

            txtLogin.Text = "tech.ivanov";
            txtPassword.Password = "password123";

            GenerateNewCaptcha();

            txtLogin.KeyDown += (s, e) => { if (e.Key == Key.Enter) BtnLogin_Click(null, null); };
            txtPassword.KeyDown += (s, e) => { if (e.Key == Key.Enter) BtnLogin_Click(null, null); };
            txtCaptcha.KeyDown += (s, e) => { if (e.Key == Key.Enter) BtnLogin_Click(null, null); };
        }

        private void GenerateNewCaptcha()
        {
            _currentCaptchaCode = _captchaService.GenerateCaptchaCode(5);
            byte[] imageBytes = _captchaService.GenerateCaptchaImage(_currentCaptchaCode);

            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                imgCaptcha.Source = bitmap;
            }
        }

        private void RefreshCaptcha_Click(object sender, RoutedEventArgs e)
        {
            GenerateNewCaptcha();
            txtCaptcha.Text = "";
        }

        private void ImgCaptcha_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            GenerateNewCaptcha();
            txtCaptcha.Text = "";
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtLogin.Text) || string.IsNullOrEmpty(txtPassword.Password))
            {
                txtError.Text = "Введите логин и пароль";
                return;
            }

            if (string.IsNullOrEmpty(txtCaptcha.Text))
            {
                txtError.Text = "Введите код с картинки";
                return;
            }

            if (!txtCaptcha.Text.Equals(_currentCaptchaCode, StringComparison.OrdinalIgnoreCase))
            {
                txtError.Text = "Неверный код с картинки";
                GenerateNewCaptcha();
                txtCaptcha.Text = "";
                return;
            }

            btnLogin.IsEnabled = false;
            txtError.Text = "";

            var success = await _authService.LoginAsync(txtLogin.Text, txtPassword.Password);

            if (success && (_authService.CurrentUser?.Role == "technologist" || _authService.CurrentUser?.Role == "admin"))
            {
                var mainWindow = new MainWindow(_apiService, _authService);
                mainWindow.Show();
                Close();
            }
            else
            {
                txtError.Text = _authService.CurrentUser == null ?
                    "Неверный логин или пароль" :
                    "Доступ запрещен. Требуется роль технолога или администратора";
                btnLogin.IsEnabled = true;
                GenerateNewCaptcha();
                txtCaptcha.Text = "";
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            Close();
        }
    }
}