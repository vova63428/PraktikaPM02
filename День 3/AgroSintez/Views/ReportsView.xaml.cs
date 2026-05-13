using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using AgroSintez.Services;

namespace AgroSintez.Views
{
    public partial class ReportsView : Page
    {
        private readonly ApiService _api;

        public ReportsView(ApiService api)
        {
            InitializeComponent();
            _api = api;
        }

        private async void RecipesReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtReportTitle.Text = "Отчет по рецептурам";
                var recipes = await _api.GetRecipesAsync();

                if (recipes == null || recipes.Count == 0)
                {
                    txtReportContent.Text = "Нет данных о рецептурах";
                    return;
                }

                var sb = new StringBuilder();
                sb.AppendLine("=== РЕЦЕПТУРЫ ===\n");
                foreach (var r in recipes)
                {
                    sb.AppendLine($"ID: {r.id}");
                    sb.AppendLine($"Наименование: {r.name}");
                    sb.AppendLine($"Версия: {r.version}");
                    sb.AppendLine($"Статус: {r.status}");
                    sb.AppendLine($"Дата создания: {r.creation_date}");
                    sb.AppendLine(new string('-', 50));
                }
                txtReportContent.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                txtReportContent.Text = $"Ошибка: {ex.Message}";
            }
        }

        private async void ProductionReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtReportTitle.Text = "Отчет по производству";

                var orders = await _api.GetProductionOrdersAsync();
                var batches = await _api.GetProductionBatchesAsync();

                var sb = new StringBuilder();
                sb.AppendLine("=== ПРОИЗВОДСТВЕННЫЕ ЗАКАЗЫ ===\n");

                if (orders != null && orders.Count > 0)
                {
                    foreach (var o in orders)
                    {
                        sb.AppendLine($"Заказ: {o.order_number}, План: {o.planned_quantity_kg} кг, Статус: {o.status}");
                    }
                }
                else
                {
                    sb.AppendLine("Нет данных о производственных заказах");
                }

                sb.AppendLine("\n=== ПРОИЗВОДСТВЕННЫЕ ПАРТИИ ===\n");

                if (batches != null && batches.Count > 0)
                {
                    foreach (var b in batches)
                    {
                        sb.AppendLine($"Партия: {b.batch_number}, Статус: {b.status}, Факт: {b.actual_quantity_kg} кг");
                    }
                }
                else
                {
                    sb.AppendLine("Нет данных о производственных партиях");
                }

                txtReportContent.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                txtReportContent.Text = $"Ошибка: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Ошибка в ProductionReport: {ex.Message}");
            }
        }

        private async void DeviationsReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtReportTitle.Text = "Отчет по отклонениям";
                var deviations = await _api.GetDeviationsAsync();

                if (deviations == null || deviations.Count == 0)
                {
                    txtReportContent.Text = "Нет данных об отклонениях";
                    return;
                }

                var sb = new StringBuilder();
                sb.AppendLine("=== ЗАФИКСИРОВАННЫЕ ОТКЛОНЕНИЯ ===\n");
                foreach (var d in deviations)
                {
                    sb.AppendLine($"ID: {d.id}");
                    sb.AppendLine($"Шаг: {d.step_name}");
                    sb.AppendLine($"Отклонение: {d.deviation_description}");
                    sb.AppendLine($"Дата: {d.end_time}");
                    sb.AppendLine(new string('-', 50));
                }
                txtReportContent.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                txtReportContent.Text = $"Ошибка: {ex.Message}";
            }
        }
    }
}