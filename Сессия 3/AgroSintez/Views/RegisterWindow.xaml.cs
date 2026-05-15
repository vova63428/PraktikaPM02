using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using AgroSintez.Services;

namespace AgroSintez.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly CaptchaService _captchaService;
        private string _currentCaptchaCode;

        public RegisterWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
            _captchaService = new CaptchaService();

            GenerateNewCaptcha();

            // Обработка нажатия Enter
            txtLogin.KeyDown += (s, e) => { if (e.Key == Key.Enter) BtnRegister_Click(null, null); };
            txtFullName.KeyDown += (s, e) => { if (e.Key == Key.Enter) BtnRegister_Click(null, null); };
            txtPassword.KeyDown += (s, e) => { if (e.Key == Key.Enter) BtnRegister_Click(null, null); };
            txtConfirmPassword.KeyDown += (s, e) => { if (e.Key == Key.Enter) BtnRegister_Click(null, null); };
            txtCaptcha.KeyDown += (s, e) => { if (e.Key == Key.Enter) BtnRegister_Click(null, null); };
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

        private async void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            txtError.Text = "";

            // Проверка логина
            if (string.IsNullOrEmpty(txtLogin.Text))
            {
                txtError.Text = "Введите логин";
                return;
            }

            // Проверка ФИО
            if (string.IsNullOrEmpty(txtFullName.Text))
            {
                txtError.Text = "Введите ФИО";
                return;
            }

            // Проверка пароля
            if (string.IsNullOrEmpty(txtPassword.Password))
            {
                txtError.Text = "Введите пароль";
                return;
            }

            // Проверка совпадения паролей
            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                txtError.Text = "Пароли не совпадают";
                return;
            }

            // Проверка CAPTCHA
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

            btnRegister.IsEnabled = false;

            try
            {
                var result = await _apiService.RegisterAsync(new
                {
                    login = txtLogin.Text,
                    fullName = txtFullName.Text,
                    password = txtPassword.Password,
                    role = "operator",
                    department = "Новый отдел"
                });

                if (result != null)
                {
                    MessageBox.Show("Регистрация прошла успешно! Теперь вы можете войти в систему.",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    var loginWindow = new LoginWindow();
                    loginWindow.Show();
                    Close();
                }
                else
                {
                    txtError.Text = "Ошибка регистрации. Возможно, такой логин уже существует.";
                    btnRegister.IsEnabled = true;
                    GenerateNewCaptcha();
                    txtCaptcha.Text = "";
                }
            }
            catch (Exception ex)
            {
                txtError.Text = $"Ошибка: {ex.Message}";
                btnRegister.IsEnabled = true;
                GenerateNewCaptcha();
                txtCaptcha.Text = "";
            }
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
    }
}