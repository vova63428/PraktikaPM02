using System;
using System.Windows;
using System.Windows.Controls;
using AgroSintez.Services;
using AgroSintez.Models;

namespace AgroSintez.Views
{
    public partial class ProductionOrdersView : Page
    {
        private readonly ApiService _api;
        private ProductionOrder _selectedOrder;

        public ProductionOrdersView(ApiService api)
        {
            InitializeComponent();
            _api = api;

            dgOrders.AutoGeneratingColumn += DgOrders_AutoGeneratingColumn;
            dgOrders.SelectionChanged += DgOrders_SelectionChanged;
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var orders = await _api.GetProductionOrdersAsync();
                dgOrders.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки: {ex.Message}");
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgOrders.SelectedItem != null)
            {
                var item = dgOrders.SelectedItem;
                var type = item.GetType();

                _selectedOrder = new ProductionOrder
                {
                    id = Convert.ToInt32(type.GetProperty("id")?.GetValue(item, null)),
                    order_number = type.GetProperty("order_number")?.GetValue(item, null)?.ToString(),
                    recipe_id = Convert.ToInt32(type.GetProperty("recipe_id")?.GetValue(item, null)),
                    tech_card_id = Convert.ToInt32(type.GetProperty("tech_card_id")?.GetValue(item, null)),
                    planned_quantity_kg = Convert.ToInt32(type.GetProperty("planned_quantity_kg")?.GetValue(item, null)),
                    status = type.GetProperty("status")?.GetValue(item, null)?.ToString(),
                    planned_start_date = type.GetProperty("planned_start_date")?.GetValue(item, null) as DateTime?,
                    created_date = type.GetProperty("created_date")?.GetValue(item, null) as DateTime?
                };
            }
        }

        private void DgOrders_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.Column.Header.ToString())
            {
                case "id": e.Column.Header = "ID"; break;
                case "order_number": e.Column.Header = "Номер заказа"; break;
                case "recipe_id": e.Column.Header = "ID рецептуры"; break;
                case "tech_card_id": e.Column.Header = "ID техкарты"; break;
                case "planned_quantity_kg": e.Column.Header = "План, кг"; break;
                case "status": e.Column.Header = "Статус"; break;
                case "planned_start_date": e.Column.Header = "Плановая дата"; break;
                case "created_date": e.Column.Header = "Дата создания"; break;
                case "created_by_id": e.Column.Header = "Создал"; break;
            }
        }

        private async void CreateOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var recipes = await _api.GetActiveRecipesAsync();
                var techCards = await _api.GetTechCardsAsync();

                if (recipes == null || recipes.Count == 0)
                {
                    MessageBox.Show("Нет активных рецептур. Сначала создайте рецептуру.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (techCards == null || techCards.Count == 0)
                {
                    MessageBox.Show("Нет технологических карт. Сначала создайте техкарту.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var dialog = new AddOrderDialog(recipes, techCards);
                dialog.Owner = Window.GetWindow(this);

                if (dialog.ShowDialog() == true)
                {
                    var result = await _api.CreateOrderAsync(new
                    {
                        recipeId = dialog.RecipeId,
                        techCardId = dialog.TechCardId,
                        plannedQuantityKg = dialog.PlannedQuantityKg,
                        status = "planned"
                    });

                    if (result != null)
                    {
                        MessageBox.Show($"Заказ #{result.order_number} успешно создан!",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при создании заказа", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void StartOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ для запуска", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_selectedOrder.status != "planned")
            {
                MessageBox.Show("Можно запустить только заказ со статусом 'planned'", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Запустить заказ #{_selectedOrder.order_number}?\nБудет создана первая производственная партия.",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    var result = await _api.StartOrderAsync(_selectedOrder.id);

                    if (result)
                    {
                        MessageBox.Show($"Заказ #{_selectedOrder.order_number} запущен! Создана первая партия.",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при запуске заказа", "Ошибка",
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