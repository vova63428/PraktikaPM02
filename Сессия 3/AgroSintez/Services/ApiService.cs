using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AgroSintez.Models;

namespace AgroSintez.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;
        private string _token;

        public ApiService()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:52940/");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void SetToken(string token)
        {
            _token = token;
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await _client.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json);
            }
            return default;
        }

        private async Task<T> PostAsync<T>(string endpoint, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(endpoint, content);
            if (response.IsSuccessStatusCode)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(resultJson);
            }
            return default;
        }

        private async Task<bool> PutAsync(string endpoint, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(endpoint, content);
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> DeleteAsync(string endpoint)
        {
            var response = await _client.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }

        // ==================== АВТОРИЗАЦИЯ ====================
        public async Task<User> LoginAsync(string login, string password)
        {
            return await PostAsync<User>("api/auth/login", new { login, password });
        }

        // ==================== ПРОДУКЦИЯ ====================
        public async Task<List<Product>> GetProductsAsync()
        {
            return await GetAsync<List<Product>>("api/reference/products");
        }

        public async Task<Product> GetProductAsync(int id)
        {
            return await GetAsync<Product>($"api/reference/products/{id}");
        }

        // ==================== СЫРЬЕ ====================
        public async Task<List<RawMaterial>> GetRawMaterialsAsync()
        {
            return await GetAsync<List<RawMaterial>>("api/reference/raw-materials");
        }

        public async Task<RawMaterial> GetRawMaterialAsync(int id)
        {
            return await GetAsync<RawMaterial>($"api/reference/raw-materials/{id}");
        }

        public async Task<List<string>> GetRawMaterialCategoriesAsync()
        {
            return await GetAsync<List<string>>("api/reference/raw-materials/categories");
        }

        // ==================== ОБОРУДОВАНИЕ ====================
        public async Task<List<Equipment>> GetEquipmentAsync()
        {
            return await GetAsync<List<Equipment>>("api/reference/equipment");
        }

        public async Task<List<Equipment>> GetActiveEquipmentAsync()
        {
            return await GetAsync<List<Equipment>>("api/reference/equipment/active");
        }

        public async Task<Equipment> GetEquipmentAsync(int id)
        {
            return await GetAsync<Equipment>($"api/reference/equipment/{id}");
        }

        public async Task<List<string>> GetEquipmentTypesAsync()
        {
            return await GetAsync<List<string>>("api/reference/equipment/types");
        }

        // ==================== РЕЦЕПТУРЫ ====================
        public async Task<List<Recipe>> GetRecipesAsync()
        {
            return await GetAsync<List<Recipe>>("api/recipes");
        }
        public async Task<bool> UpdateRecipeStatusAsync(int id, string status)
        {
            var data = new { status = status };
            return await PutAsync($"api/recipes/{id}/status", data);
        }

        public async Task<List<Recipe>> GetActiveRecipesAsync()
        {
            return await GetAsync<List<Recipe>>("api/recipes/active");
        }

        public async Task<Recipe> GetRecipeAsync(int id)
        {
            return await GetAsync<Recipe>($"api/recipes/{id}");
        }

        public async Task<Recipe> CreateRecipeAsync(object recipe)
        {
            return await PostAsync<Recipe>("api/recipes", recipe);
        }

        public async Task<bool> UpdateRecipeAsync(int id, object recipe)
        {
            return await PutAsync($"api/recipes/{id}", recipe);
        }

        public async Task<bool> ApproveRecipeAsync(int id)
        {
            return await PostAsync<bool>($"api/recipes/{id}/approve", null);
        }

        public async Task<bool> DeleteRecipeAsync(int id)
        {
            return await DeleteAsync($"api/recipes/{id}");
        }

        public async Task<List<RecipeComponent>> GetRecipeComponentsAsync(int recipeId)
        {
            return await GetAsync<List<RecipeComponent>>($"api/recipes/{recipeId}/components");
        }

        public async Task<bool> AddComponentAsync(int recipeId, object component)
        {
            return await PostAsync<bool>($"api/recipes/{recipeId}/components", component);
        }

        public async Task<bool> UpdateComponentAsync(int componentId, object component)
        {
            return await PutAsync($"api/recipes/components/{componentId}", component);
        }

        public async Task<bool> DeleteComponentAsync(int componentId)
        {
            return await DeleteAsync($"api/recipes/components/{componentId}");
        }

        // ==================== ТЕХНОЛОГИЧЕСКИЕ КАРТЫ ====================
        public async Task<List<TechCard>> GetTechCardsAsync()
        {
            return await GetAsync<List<TechCard>>("api/tech-cards");
        }

        public async Task<TechCard> GetTechCardAsync(int id)
        {
            return await GetAsync<TechCard>($"api/tech-cards/{id}");
        }

        public async Task<TechCard> CreateTechCardAsync(object card)
        {
            return await PostAsync<TechCard>("api/tech-cards", card);
        }

        public async Task<bool> UpdateTechCardAsync(int id, object card)
        {
            return await PutAsync($"api/tech-cards/{id}", card);
        }

        public async Task<bool> ApproveTechCardAsync(int id)
        {
            return await PostAsync<bool>($"api/tech-cards/{id}/approve", null);
        }

        public async Task<bool> DeleteTechCardAsync(int id)
        {
            return await DeleteAsync($"api/tech-cards/{id}");
        }

        public async Task<List<TechCardStep>> GetTechCardStepsAsync(int techCardId)
        {
            return await GetAsync<List<TechCardStep>>($"api/tech-cards/{techCardId}/steps");
        }

        public async Task<bool> AddTechCardStepAsync(int techCardId, object step)
        {
            return await PostAsync<bool>($"api/tech-cards/{techCardId}/steps", step);
        }

        public async Task<bool> UpdateTechCardStepAsync(int stepId, object step)
        {
            return await PutAsync($"api/tech-cards/steps/{stepId}", step);
        }

        public async Task<bool> DeleteTechCardStepAsync(int stepId)
        {
            return await DeleteAsync($"api/tech-cards/steps/{stepId}");
        }

        // ==================== ПРОИЗВОДСТВЕННЫЕ ЗАКАЗЫ ====================
        public async Task<List<ProductionOrder>> GetProductionOrdersAsync()
        {
            return await GetAsync<List<ProductionOrder>>("api/production-orders");
        }

        public async Task<ProductionOrder> GetProductionOrderAsync(int id)
        {
            return await GetAsync<ProductionOrder>($"api/production-orders/{id}");
        }

        public async Task<List<ProductionOrder>> GetProductionOrdersByStatusAsync(string status)
        {
            return await GetAsync<List<ProductionOrder>>($"api/production-orders/status/{status}");
        }

        public async Task<ProductionOrder> CreateOrderAsync(object order)
        {
            return await PostAsync<ProductionOrder>("api/production-orders", order);
        }

        public async Task<bool> UpdateOrderAsync(int id, object order)
        {
            return await PutAsync($"api/production-orders/{id}", order);
        }

        public async Task<bool> StartOrderAsync(int id)
        {
            return await PostAsync<bool>($"api/production-orders/{id}/start", null);
        }

        public async Task<bool> CompleteOrderAsync(int id)
        {
            return await PostAsync<bool>($"api/production-orders/{id}/complete", null);
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            return await DeleteAsync($"api/production-orders/{id}");
        }

        // ==================== ПРОИЗВОДСТВЕННЫЕ ПАРТИИ ====================
        public async Task<List<ProductionBatch>> GetProductionBatchesAsync()
        {
            return await GetAsync<List<ProductionBatch>>("api/production-batches");
        }

        public async Task<ProductionBatch> CreateProductionBatchAsync(object batch)
        {
            return await PostAsync<ProductionBatch>("api/production-batches", batch);
        }

        public async Task<ProductionBatch> GetProductionBatchAsync(int id)
        {
            return await GetAsync<ProductionBatch>($"api/production-batches/{id}");
        }

        public async Task<List<ProductionBatch>> GetBatchesByOrderAsync(int orderId)
        {
            return await GetAsync<List<ProductionBatch>>($"api/production-batches/order/{orderId}");
        }

        // ==================== МОНИТОРИНГ ====================
        public async Task<List<ProductionBatch>> GetActiveBatchesAsync()
        {
            return await GetAsync<List<ProductionBatch>>("api/production-batches/active");
        }

        public async Task<List<BatchStep>> GetBatchStepsAsync(int batchId)
        {
            return await GetAsync<List<BatchStep>>($"api/shopfloor/batches/{batchId}/steps");
        }

        public async Task<bool> StartBatchAsync(int batchId)
        {
            return await PostAsync<bool>($"api/shopfloor/batches/{batchId}/start", null);
        }

        // ==================== ЛАБОРАТОРИЯ ====================
        public async Task<object> GetPendingLabTestsAsync()
        {
            return await GetAsync<object>("api/lab-tests/pending");
        }

        public async Task<List<QualityControl>> GetBatchTestsAsync(int batchId)
        {
            return await GetAsync<List<QualityControl>>($"api/lab-tests/batch/{batchId}");
        }

        public async Task<object> CreateBatchTestAsync(object test)
        {
            return await PostAsync<object>("api/lab-tests/for-batch", test);
        }

        public async Task<object> CreateRawMaterialTestAsync(object test)
        {
            return await PostAsync<object>("api/lab-tests/for-raw-material", test);
        }

        // ==================== ОТКЛОНЕНИЯ ====================
        public async Task<List<Deviation>> GetDeviationsAsync()
        {
            return await GetAsync<List<Deviation>>("api/deviations");
        }

        public async Task<List<Deviation>> GetBatchDeviationsAsync(int batchId)
        {
            return await GetAsync<List<Deviation>>($"api/deviations/batch/{batchId}");
        }

        public async Task<object> GetEventsAsync()
        {
            return await GetAsync<object>("api/deviations/events");
        }

        public async Task<object> GetBatchEventsAsync(int batchId)
        {
            return await GetAsync<object>($"api/deviations/events/batch/{batchId}");
        }

        public async Task<bool> ResolveDeviationAsync(int deviationId, object resolveData)
        {
            return await PostAsync<bool>($"api/deviations/resolve/{deviationId}", resolveData);
        }

        // ==================== УВЕДОМЛЕНИЯ ====================
        public async Task<List<Notification>> GetNotificationsAsync()
        {
            return await GetAsync<List<Notification>>("api/deviations/notifications");
        }

        public async Task<List<Notification>> GetUnreadNotificationsAsync()
        {
            return await GetAsync<List<Notification>>("api/deviations/notifications/unread");
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(int userId)
        {
            return await GetAsync<List<Notification>>($"api/deviations/notifications/user/{userId}");
        }

        public async Task<bool> MarkNotificationAsReadAsync(int id)
        {
            return await PutAsync($"api/deviations/notifications/{id}/read", null);
        }

        public async Task<bool> MarkAllNotificationsAsReadAsync(int userId)
        {
            return await PutAsync($"api/deviations/notifications/mark-all-read", userId);
        }

        public async Task<bool> CreateNotificationAsync(object notification)
        {
            return await PostAsync<bool>("api/deviations/notifications", notification);
        }

        // ==================== СТАТИСТИКА ====================
        public async Task<object> GetStatisticsAsync()
        {
            return await GetAsync<object>("api/reference/statistics");
        }

        public async Task<object> SearchAsync(string query)
        {
            return await GetAsync<object>($"api/reference/search?q={Uri.EscapeDataString(query)}");
        }

        // ==================== СТАТУСЫ ====================
        public async Task<object> GetOrderStatusesAsync()
        {
            return await GetAsync<object>("api/reference/order-statuses");
        }

        public async Task<object> GetBatchStatusesAsync()
        {
            return await GetAsync<object>("api/reference/batch-statuses");
        }

        public async Task<object> GetRecipeStatusesAsync()
        {
            return await GetAsync<object>("api/reference/recipe-statuses");
        }

        // ==================== ПОЛЬЗОВАТЕЛИ ====================
        public async Task<List<User>> GetUsersAsync()
        {
            return await GetAsync<List<User>>("api/reference/users");
        }

        public async Task<List<User>> GetTechnologistsAsync()
        {
            return await GetAsync<List<User>>("api/reference/users/technologists");
        }

        public async Task<List<User>> GetOperatorsAsync()
        {
            return await GetAsync<List<User>>("api/reference/users/operators");
        }

        public async Task<List<string>> GetUserRolesAsync()
        {
            return await GetAsync<List<string>>("api/reference/users/roles");
        }
        // ==================== РЕГИСТРАЦИЯ ====================
        public async Task<User> RegisterAsync(object registerData)
        {
            return await PostAsync<User>("api/auth/register", registerData);
        }
    }
}