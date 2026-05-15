using System;
using System.Windows;
using System.Windows.Controls;
using AgroSintez.Services;
using AgroSintez.Models;

namespace AgroSintez.Views
{
    public partial class RecipesView : Page
    {
        private readonly ApiService _api;
        private Recipe _selectedRecipe;

        public RecipesView(ApiService api)
        {
            InitializeComponent();
            _api = api;

            dgRecipes.AutoGeneratingColumn += DgRecipes_AutoGeneratingColumn;
            dgRecipes.SelectionChanged += DgRecipes_SelectionChanged;
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var recipes = await _api.GetRecipesAsync();
                dgRecipes.ItemsSource = recipes;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка: {ex.Message}");
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgRecipes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgRecipes.SelectedItem != null)
            {
                var item = dgRecipes.SelectedItem;
                var type = item.GetType();

                _selectedRecipe = new Recipe
                {
                    id = Convert.ToInt32(type.GetProperty("id")?.GetValue(item, null)),
                    name = type.GetProperty("name")?.GetValue(item, null)?.ToString(),
                    product_id = Convert.ToInt32(type.GetProperty("product_id")?.GetValue(item, null)),
                    version = Convert.ToInt32(type.GetProperty("version")?.GetValue(item, null)),
                    status = type.GetProperty("status")?.GetValue(item, null)?.ToString(),
                    creation_date = type.GetProperty("creation_date")?.GetValue(item, null) as DateTime?,
                    created_by_id = Convert.ToInt32(type.GetProperty("created_by_id")?.GetValue(item, null))
                };
            }
        }

        private void DgRecipes_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.Column.Header.ToString())
            {
                case "id": e.Column.Header = "ID"; break;
                case "name": e.Column.Header = "Наименование"; break;
                case "product_id": e.Column.Header = "ID продукта"; break;
                case "version": e.Column.Header = "Версия"; break;
                case "status": e.Column.Header = "Статус"; break;
                case "creation_date": e.Column.Header = "Дата создания"; break;
                case "created_by_id": e.Column.Header = "Создал"; break;
            }
        }

        private async void AddRecipe_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddRecipeDialog();
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var result = await _api.CreateRecipeAsync(new
                    {
                        productId = dialog.ProductId,
                        name = dialog.RecipeName,
                        status = "draft"
                    });

                    if (result != null)
                    {
                        MessageBox.Show($"Рецептура \"{dialog.RecipeName}\" успешно добавлена!",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при добавлении рецептуры", "Ошибка",
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

        // НОВЫЙ МЕТОД: Активация/изменение статуса рецептуры
        private async void ActivateRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedRecipe == null)
            {
                MessageBox.Show("Выберите рецептуру", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new ChangeStatusDialog(_selectedRecipe.name, _selectedRecipe.status);
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var result = await _api.UpdateRecipeStatusAsync(_selectedRecipe.id, dialog.NewStatus);

                    if (result)
                    {
                        string statusText = dialog.NewStatus == "active" ? "активирована" :
                                           (dialog.NewStatus == "archived" ? "архивирована" : "переведена в черновик");

                        MessageBox.Show($"Рецептура \"{_selectedRecipe.name}\" успешно {statusText}!",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при изменении статуса", "Ошибка",
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