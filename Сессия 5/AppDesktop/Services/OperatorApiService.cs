using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AppDesktop.Models;

namespace AppDesktop.Services
{
    public class OperatorApiService
    {
        private readonly HttpClient _client;
        private string _token;

        public OperatorApiService()
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
                var resultJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(resultJson);
                }
                return default;
            }
            catch
            {
                return default;
            }
        }

        private async Task<T> PostEmptyAsync<T>(string endpoint)
        {
            try
            {
                var response = await _client.PostAsync(endpoint, null);
                var resultJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(resultJson);
                }
                return default;
            }
            catch
            {
                return default;
            }
        }

        public async Task<bool> LoginAsync(string login, string password)
        {
            try
            {
                var response = await PostAsync<dynamic>("api/auth/login", new { login, password });

                if (response == null) return false;

                string token = null;
                if (response.token != null) token = response.token.ToString();
                else if (response.Token != null) token = response.Token.ToString();

                if (string.IsNullOrEmpty(token)) return false;

                int id = 0;
                if (response.id != null) id = Convert.ToInt32(response.id);
                else if (response.Id != null) id = Convert.ToInt32(response.Id);

                string userLogin = login;
                if (response.login != null) userLogin = response.login.ToString();
                else if (response.Login != null) userLogin = response.Login.ToString();

                string role = "operator";
                if (response.role != null) role = response.role.ToString();
                else if (response.Role != null) role = response.Role.ToString();

                if (role.ToLower() != "operator") return false;

                string fullName = userLogin;
                if (response.fullName != null) fullName = response.fullName.ToString();
                else if (response.FullName != null) fullName = response.FullName.ToString();

                SetToken(token);
                App.CurrentUser = new User
                {
                    Id = id,
                    Login = userLogin,
                    FullName = fullName,
                    Role = role,
                    Token = token
                };

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<ProductionBatch>> GetActiveBatchesAsync()
        {
            var batches = await GetAsync<List<ProductionBatch>>("api/shopfloor/batches/active");
            if (batches == null || batches.Count == 0)
            {
                batches = await GetAsync<List<ProductionBatch>>("api/production-batches/active");
            }
            return batches ?? new List<ProductionBatch>();
        }

        public async Task<List<BatchStep>> GetBatchStepsAsync(int batchId)
        {
            var steps = await GetAsync<List<BatchStep>>($"api/shopfloor/batches/{batchId}/steps");
            if (steps == null)
            {
                steps = await GetAsync<List<BatchStep>>($"api/production-batches/{batchId}/steps");
            }
            return steps ?? new List<BatchStep>();
        }

        public async Task<BatchStep> GetCurrentStepAsync(int batchId)
        {
            var step = await GetAsync<BatchStep>($"api/shopfloor/batches/{batchId}/current-step");
            if (step == null)
            {
                step = await GetAsync<BatchStep>($"api/production-batches/{batchId}/current-step");
            }

            
            if (step != null)
            {
                if (step.expected_temperature == 0 && step.planned_temperature.HasValue && step.planned_temperature.Value > 0)
                {
                    step.expected_temperature = step.planned_temperature.Value;
                }
                if (step.expected_pressure == 0 && step.planned_pressure.HasValue && step.planned_pressure.Value > 0)
                {
                    step.expected_pressure = step.planned_pressure.Value;
                }
            }

            return step;
        }
        public async Task<bool> NotifyLabAsync(int batchId)
        {
            try
            {
                return await PostEmptyAsync<bool>($"api/notifications/batch/{batchId}/lab");
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> StartStepAsync(int stepId)
        {
            try
            {
                return await PostEmptyAsync<bool>($"api/shopfloor/steps/{stepId}/start");
            }
            catch
            {
                return true;
            }
        }

        public async Task<bool> CompleteStepAsync(int stepId, object actualParameters)
        {
            try
            {
                return await PostAsync<bool>($"api/shopfloor/steps/{stepId}/record", actualParameters);
            }
            catch
            {
                return true;
            }
        }

        public async Task<bool> CompleteBatchAsync(int batchId)
        {
            try
            {
               
                var response = await PostAsync<bool>($"api/shopfloor/batches/{batchId}/complete", new { });
                return response;
            }
            catch
            {
                return false;
            }
        }

        public async Task<EquipmentTelemetry> GetTelemetryAsync(int equipmentId)
        {
            var tele = await GetAsync<EquipmentTelemetry>($"api/shopfloor/telemetry/{equipmentId}");
            if (tele == null)
            {
                tele = await GetAsync<EquipmentTelemetry>($"api/reference/equipment/{equipmentId}");
            }
            return tele;
        }
    }
}