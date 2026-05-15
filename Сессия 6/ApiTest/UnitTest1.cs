using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ApiTest
{
    public class SimpleApiTests
    {
        private readonly HttpClient _client;


        public SimpleApiTests()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:52940/"); 
            _client.Timeout = TimeSpan.FromSeconds(30); 
        }


        //  Проверка работоспособности Api

        [Fact]
        public async Task Test_ApiIsAlive()
        {


            Console.WriteLine($"Проверка API на {_client.BaseAddress}");

            
            var response = await _client.GetAsync("/api/auth/simple");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Статус: {response.StatusCode}");
            Console.WriteLine($"Ответ: {content}");

            
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

           
            Assert.Contains("Server works", content);
        }

       
        //  Регистрация Нового пользователя

        [Fact]
        public async Task Test_Register()
        {
     

            var newUser = new
            {
                login = $"test_{Guid.NewGuid():N}",      
                fullName = "Test User",                
                password = "test123",                     
                role = "operator"                         
            };

          
            var json = JsonSerializer.Serialize(newUser);

            
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            
            var response = await _client.PostAsync("/api/auth/register", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Регистрация: {response.StatusCode}");
            Console.WriteLine($"Ответ: {responseContent}");

            
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

          
            Assert.Contains("Token", responseContent);  
        }


        //  Авторизация пользователя

        [Fact]
        public async Task Test_Login()
        {
            var login = $"user_{Guid.NewGuid():N}";  

            var registerData = new { login, password = "pass123", role = "operator" };
            var registerJson = JsonSerializer.Serialize(registerData);

            // Регистрируем пользователя
            await _client.PostAsync("/api/auth/register",
                new StringContent(registerJson, Encoding.UTF8, "application/json"));

            var loginData = new { login, password = "pass123" };  
            var loginJson = JsonSerializer.Serialize(loginData);

            var response = await _client.PostAsync("/api/auth/login",
                new StringContent(loginJson, Encoding.UTF8, "application/json"));

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            // Проверяем, что в ответе есть JWT токен (пользователь авторизован)
            Assert.Contains("Token", responseContent);
        }


        // Получение статистики системы

        [Fact]
        public async Task Test_GetStatistics()
        {
 
            var response = await _client.GetAsync("/api/reference/statistics");
            var content = await response.Content.ReadAsStringAsync();


            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);


            Assert.Contains("usersCount", content);

       
        }


        // Получение списка продуктов
        [Fact]
        public async Task Test_GetProducts()
        {
            var response = await _client.GetAsync("/api/reference/products");
          
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

           
        }
    }
}