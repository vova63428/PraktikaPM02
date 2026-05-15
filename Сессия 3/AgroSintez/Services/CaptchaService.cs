using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace AgroSintez.Services
{
    public class CaptchaService
    {
        private static readonly Random _random = new Random();

        // Генерация случайного кода (4-6 символов)
        public string GenerateCaptchaCode(int length = 5)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ0123456789";
            char[] code = new char[length];
            for (int i = 0; i < length; i++)
            {
                code[i] = chars[_random.Next(chars.Length)];
            }
            return new string(code);
        }

        // Создание изображения CAPTCHA
        public byte[] GenerateCaptchaImage(string captchaCode)
        {
            int width = 220;
            int height = 80;

            using (Bitmap bitmap = new Bitmap(width, height))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Белый фон
                graphics.Clear(Color.White);

                // Рисуем рамку
                graphics.DrawRectangle(new Pen(Color.LightGray, 1), 0, 0, width - 1, height - 1);

                // Добавляем случайные линии (шум)
                for (int i = 0; i < 20; i++)
                {
                    int x1 = _random.Next(width);
                    int y1 = _random.Next(height);
                    int x2 = _random.Next(width);
                    int y2 = _random.Next(height);
                    graphics.DrawLine(new Pen(Color.LightGray, 1), x1, y1, x2, y2);
                }

                // Добавляем точки-шум (200 точек)
                for (int i = 0; i < 200; i++)
                {
                    int x = _random.Next(width);
                    int y = _random.Next(height);
                    bitmap.SetPixel(x, y, Color.FromArgb(150, 150, 150));
                }

                // Рисуем каждую букву с искажениями
                using (Font font = new Font("Arial", 26, FontStyle.Bold))
                {
                    for (int i = 0; i < captchaCode.Length; i++)
                    {
                        // Случайное смещение для каждой буквы
                        int x = 20 + i * 35 + _random.Next(-5, 5);
                        int y = 20 + _random.Next(-8, 8);

                        // Случайный угол поворота
                        float angle = _random.Next(-20, 20);

                        // Случайный цвет для каждой буквы
                        Color textColor = Color.FromArgb(
                            _random.Next(50, 180),
                            _random.Next(50, 180),
                            _random.Next(50, 180)
                        );

                        using (SolidBrush brush = new SolidBrush(textColor))
                        {
                            // Сохраняем состояние графики для поворота
                            graphics.TranslateTransform(x, y);
                            graphics.RotateTransform(angle);
                            graphics.DrawString(captchaCode[i].ToString(), font, brush, 0, 0);
                            graphics.RotateTransform(-angle);
                            graphics.TranslateTransform(-x, -y);
                        }
                    }
                }

                // Сохраняем в массив байтов
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }
    }
}