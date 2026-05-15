using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using AgroSintez.Models;

namespace AgroSintez.Views
{
    public partial class AddOrderDialog : Window
    {
        public int RecipeId { get; private set; }
        public int TechCardId { get; private set; }
        public int PlannedQuantityKg { get; private set; }

        public AddOrderDialog(List<Recipe> recipes, List<TechCard> techCards)
        {
            InitializeComponent();

            cbRecipe.ItemsSource = recipes;
            cbTechCard.ItemsSource = techCards;

            if (recipes.Count > 0)
                cbRecipe.SelectedIndex = 0;
            if (techCards.Count > 0)
                cbTechCard.SelectedIndex = 0;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (cbRecipe.SelectedItem == null)
            {
                MessageBox.Show("Выберите рецептуру", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbTechCard.SelectedItem == null)
            {
                MessageBox.Show("Выберите технологическую карту", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество (кг)", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var recipe = cbRecipe.SelectedItem as Recipe;
            var techCard = cbTechCard.SelectedItem as TechCard;

            RecipeId = recipe.id;
            TechCardId = techCard.id;
            PlannedQuantityKg = quantity;

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}