using System;
using System.Windows;
using System.Windows.Controls;
using AgroSintez.Services;
using AgroSintez.Models;

namespace AgroSintez.Views
{
    public partial class TechCardsView : Page
    {
        private readonly ApiService _api;
        private TechCard _selectedTechCard;

        public TechCardsView(ApiService api)
        {
            InitializeComponent();
            _api = api;

            dgTechCards.AutoGeneratingColumn += DgTechCards_AutoGeneratingColumn;
            dgTechCards.SelectionChanged += DgTechCards_SelectionChanged;
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var techCards = await _api.GetTechCardsAsync();
                dgTechCards.ItemsSource = techCards;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки: {ex.Message}");
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgTechCards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTechCards.SelectedItem != null)
            {
                var item = dgTechCards.SelectedItem;
                var type = item.GetType();

                _selectedTechCard = new TechCard
                {
                    id = Convert.ToInt32(type.GetProperty("id")?.GetValue(item, null)),
                    product_id = Convert.ToInt32(type.GetProperty("product_id")?.GetValue(item, null)),
                    version = Convert.ToInt32(type.GetProperty("version")?.GetValue(item, null)),
                    status = type.GetProperty("status")?.GetValue(item, null)?.ToString(),
                    approval_date = type.GetProperty("approval_date")?.GetValue(item, null) as DateTime?,
                    created_date = type.GetProperty("created_date")?.GetValue(item, null) as DateTime?
                };
            }
        }

        private void DgTechCards_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.Column.Header.ToString())
            {
                case "id": e.Column.Header = "ID"; break;
                case "product_id": e.Column.Header = "ID продукта"; break;
                case "version": e.Column.Header = "Версия"; break;
                case "status": e.Column.Header = "Статус"; break;
                case "approval_date": e.Column.Header = "Дата утверждения"; break;
                case "created_date": e.Column.Header = "Дата создания"; break;
            }
        }

        private async void CreateTechCard_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddTechCardDialog();
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var result = await _api.CreateTechCardAsync(new
                    {
                        productId = dialog.ProductId,
                        status = "draft"
                    });

                    if (result != null)
                    {
                        MessageBox.Show($"Технологическая карта для продукта ID={dialog.ProductId} успешно создана!",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при создании технологической карты", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void ApproveTechCard_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTechCard == null)
            {
                MessageBox.Show("Выберите технологическую карту для утверждения", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_selectedTechCard.status == "approved")
            {
                MessageBox.Show("Эта технологическая карта уже утверждена", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show($"Утвердить технологическую карту ID={_selectedTechCard.id}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    var result = await _api.ApproveTechCardAsync(_selectedTechCard.id);

                    if (result)
                    {
                        MessageBox.Show("Технологическая карта успешно утверждена!",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при утверждении технологической карты", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}