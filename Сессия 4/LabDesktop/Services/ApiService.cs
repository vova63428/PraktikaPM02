using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LabDesktop.Models;

namespace LabDesktop.Services
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
            try
            {
                var response = await _client.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(json);
                }
                return default;
            }
            catch
            {
                return default;
            }
        }

        private async Task<T> PostAsync<T>(string endpoint, object data)
        {
            try
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
            catch
            {
                return default;
            }
        }

        public async Task<User> LoginAsync(string login, string password)
        {
            return await PostAsync<User>("api/auth/login", new { login, password });
        }

        public async Task<LabStatistics> GetStatisticsAsync()
        {
            return await GetAsync<LabStatistics>("api/lab-tests/statistics");
        }

        public async Task<List<LabTest>> GetTestHistoryAsync(DateTime? from, DateTime? to)
        {
            string url = "api/lab-tests/history";
            var parameters = new List<string>();
            if (from.HasValue)
                parameters.Add($"from={from.Value:yyyy-MM-dd}");
            if (to.HasValue)
                parameters.Add($"to={to.Value:yyyy-MM-dd}");
            if (parameters.Count > 0)
                url += "?" + string.Join("&", parameters);
            return await GetAsync<List<LabTest>>(url);
        }

        public async Task<List<dynamic>> GetBatchesForQCAsync()
        {
            return await GetAsync<List<dynamic>>("api/lab-tests/batches/pending");
        }

        public async Task<List<dynamic>> GetRawMaterialBatchesForQCAsync()
        {
            return await GetAsync<List<dynamic>>("api/lab-tests/raw-materials/pending");
        }

        public async Task<List<LabTest>> GetBatchTestsAsync(int batchId)
        {
            return await GetAsync<List<LabTest>>($"api/lab-tests/batch/{batchId}");
        }

        public async Task<LabTest> CreateBatchTestAsync(object test)
        {
            return await PostAsync<LabTest>("api/lab-tests/for-batch", test);
        }
        public async Task<bool> RegisterAsync(string login, string password, string fullName, string role)
        {
            try
            {
                var data = new { login, password, fullName, role };
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("api/auth/register", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }


        public async Task<bool> CompleteBatchQCAsync(int batchId, string decision, string comment)
        {
            try
            {
                // Пробуем разные форматы
                var data = new { decision = decision, comment = comment };
                var json = JsonConvert.SerializeObject(data);

                System.Diagnostics.Debug.WriteLine($"=== ОТПРАВКА ===");
                System.Diagnostics.Debug.WriteLine($"URL: api/lab-tests/batch/{batchId}/complete");
                System.Diagnostics.Debug.WriteLine($"Body: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync($"api/lab-tests/batch/{batchId}/complete", content);

                var resultJson = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"=== ОТВЕТ ===");
                System.Diagnostics.Debug.WriteLine($"StatusCode: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Body: {resultJson}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                return false;
            }
        }
    }
}