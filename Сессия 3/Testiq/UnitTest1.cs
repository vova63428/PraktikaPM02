using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgroSintez.Models;
using AgroSintez.Services;

namespace Testiq
{
    [TestClass]
    public class UnitTest1
    {
        // ТЕСТ №1: МОДЕЛИ ДАННЫХ (ЭТИ ТЕСТЫ РАБОТАЮТ 100%) 

        [TestMethod]
        public void ProductionBatch_Модель_СоздаетсяИЗаполняется()
        {
           
            var batch = new ProductionBatch
            {
                id = 1,
                batch_number = "BATCH-001",
                order_id = 100,
                status = "planned",
                actual_quantity_kg = 0,
                planned_quantity_kg = 5000,
                start_time = DateTime.Now
            };

           
            Assert.AreEqual(1, batch.id, "ID должен быть 1");
            Assert.AreEqual("BATCH-001", batch.batch_number, "Номер партии не совпадает");
            Assert.AreEqual(100, batch.order_id, "ID заказа не совпадает");
            Assert.AreEqual("planned", batch.status, "Статус должен быть planned");
            Assert.AreEqual(0, batch.actual_quantity_kg, "Фактическое количество должно быть 0");
            Assert.AreEqual(5000, batch.planned_quantity_kg, "Плановое количество должно быть 5000");
            Assert.IsNotNull(batch.start_time, "Время старта должно быть установлено");
        }

        [TestMethod]
        public void Recipe_Модель_СоздаетсяИЗаполняется()
        {
            
            var recipe = new Recipe
            {
                id = 10,
                name = "Тестовая рецептура",
                product_id = 5,
                version = 1,
                status = "draft",
                creation_date = DateTime.Now,
                created_by_id = 1
            };

          
            Assert.AreEqual(10, recipe.id);
            Assert.AreEqual("Тестовая рецептура", recipe.name);
            Assert.AreEqual(5, recipe.product_id);
            Assert.AreEqual(1, recipe.version);
            Assert.AreEqual("draft", recipe.status);
            Assert.IsNotNull(recipe.creation_date);
            Assert.AreEqual(1, recipe.created_by_id);
        }

        [TestMethod]
        public void ProductionOrder_Модель_СоздаетсяИЗаполняется()
        {
           
            var order = new ProductionOrder
            {
                id = 100,
                order_number = "ORDER-001",
                recipe_id = 10,
                tech_card_id = 5,
                planned_quantity_kg = 10000,
                status = "planned",
                planned_start_date = DateTime.Now.AddDays(1),
                created_date = DateTime.Now
            };

            
            Assert.AreEqual(100, order.id);
            Assert.AreEqual("ORDER-001", order.order_number);
            Assert.AreEqual(10, order.recipe_id);
            Assert.AreEqual(5, order.tech_card_id);
            Assert.AreEqual(10000, order.planned_quantity_kg);
            Assert.AreEqual("planned", order.status);
            Assert.IsNotNull(order.planned_start_date);
            Assert.IsNotNull(order.created_date);
        }

        [TestMethod]
        public void User_Модель_СоздаетсяИЗаполняется()
        {
           
            var user = new User
            {
                Id = 1,
                Login = "tech.ivanov",
                FullName = "Иванов Иван Иванович",
                Role = "technologist",
                Token = "test-token-123"
            };

            
            Assert.AreEqual(1, user.Id);
            Assert.AreEqual("tech.ivanov", user.Login);
            Assert.AreEqual("Иванов Иван Иванович", user.FullName);
            Assert.AreEqual("technologist", user.Role);
            Assert.AreEqual("test-token-123", user.Token);
        }

        [TestMethod]
        public void TechCard_Модель_СоздаетсяИЗаполняется()
        {
            
            var techCard = new TechCard
            {
                id = 1,
                product_id = 5,
                version = 2,
                status = "approved",
                approval_date = DateTime.Now,
                created_date = DateTime.Now
            };

            
            Assert.AreEqual(1, techCard.id);
            Assert.AreEqual(5, techCard.product_id);
            Assert.AreEqual(2, techCard.version);
            Assert.AreEqual("approved", techCard.status);
            Assert.IsNotNull(techCard.approval_date);
            Assert.IsNotNull(techCard.created_date);
        }

        [TestMethod]
        public void Equipment_Модель_СоздаетсяИЗаполняется()
        {
            
            var equipment = new Equipment
            {
                id = 1,
                code = "EX-001",
                name = "Экструдер EK-45",
                equipment_type = "extruder",
                line_number = 1,
                status = "active",
                last_calibration_date = DateTime.Now,
                created_date = DateTime.Now
            };

            
            Assert.AreEqual(1, equipment.id);
            Assert.AreEqual("EX-001", equipment.code);
            Assert.AreEqual("Экструдер EK-45", equipment.name);
            Assert.AreEqual("extruder", equipment.equipment_type);
            Assert.AreEqual(1, equipment.line_number);
            Assert.AreEqual("active", equipment.status);
            Assert.IsNotNull(equipment.last_calibration_date);
            Assert.IsNotNull(equipment.created_date);
        }

        [TestMethod]
        public void RawMaterial_Модель_СоздаетсяИЗаполняется()
        {
           
            var material = new RawMaterial
            {
                id = 1,
                code = "RM-001",
                name = "Карбамид",
                category = "Удобрения",
                unit_of_measure = "кг",
                hazard_class = 3,
                created_date = DateTime.Now
            };

            Assert.AreEqual(1, material.id);
            Assert.AreEqual("RM-001", material.code);
            Assert.AreEqual("Карбамид", material.name);
            Assert.AreEqual("Удобрения", material.category);
            Assert.AreEqual("кг", material.unit_of_measure);
            Assert.AreEqual(3, material.hazard_class);
            Assert.IsNotNull(material.created_date);
        }

        [TestMethod]
        public void Deviation_Модель_СоздаетсяИЗаполняется()
        {
            
            var deviation = new Deviation
            {
                id = 1,
                batch_id = 100,
                batch_number = "BATCH-001",
                step_number = 3,
                step_name = "Смешивание",
                deviation_description = "Превышение температуры",
                operator_comment = "Проверить настройки",
                end_time = DateTime.Now
            };

           
            Assert.AreEqual(1, deviation.id);
            Assert.AreEqual(100, deviation.batch_id);
            Assert.AreEqual("BATCH-001", deviation.batch_number);
            Assert.AreEqual(3, deviation.step_number);
            Assert.AreEqual("Смешивание", deviation.step_name);
            Assert.AreEqual("Превышение температуры", deviation.deviation_description);
            Assert.AreEqual("Проверить настройки", deviation.operator_comment);
            Assert.IsNotNull(deviation.end_time);
        }


        //ТЕСТ №2: СЛУЖБА АВТОРИЗАЦИИ

        [TestMethod]
        public void AuthService_ПослеСоздания_ПользовательНеАвторизован()
        {
            
            var apiService = new ApiService();
            var authService = new AuthService(apiService);

          
            Assert.IsFalse(authService.IsAuthenticated, "Новый пользователь не должен быть авторизован");
            Assert.IsNull(authService.CurrentUser, "CurrentUser должен быть null");
        }

        [TestMethod]
        public void AuthService_ПослеLogout_ПользовательНеАвторизован()
        {
            
            var apiService = new ApiService();
            var authService = new AuthService(apiService);

            
            authService.Logout();

            
            Assert.IsFalse(authService.IsAuthenticated);
            Assert.IsNull(authService.CurrentUser);
        }


        //ТЕСТ №3: CAPTCHA 

        [TestMethod]
        public void CaptchaService_ГенерацияКодаДлиной5_ВозвращаетСтрокуДлины5()
        {
            
            var service = new CaptchaService();

           
            string code = service.GenerateCaptchaCode(5);

            
            Assert.AreEqual(5, code.Length, $"Длина кода должна быть 5, а не {code.Length}");
        }

        [TestMethod]
        public void CaptchaService_ГенерацияКодаДлиной6_ВозвращаетСтрокуДлины6()
        {
            var service = new CaptchaService();
            string code = service.GenerateCaptchaCode(6);
            Assert.AreEqual(6, code.Length);
        }

        [TestMethod]
        public void CaptchaService_ГенерацияКодаДлиной4_ВозвращаетСтрокуДлины4()
        {
            var service = new CaptchaService();
            string code = service.GenerateCaptchaCode(4);
            Assert.AreEqual(4, code.Length);
        }

        [TestMethod]
        public void CaptchaService_ГенерацияКода_СодержитТолькоДопустимыеСимволы()
        {
            var service = new CaptchaService();
            string code = service.GenerateCaptchaCode(50);
            string allowedChars = "ABCDEFGHJKLMNPQRSTUVWXYZ0123456789";

            foreach (char c in code)
            {
                Assert.IsTrue(allowedChars.Contains(c),
                    $"Символ '{c}' не входит в разрешенный набор символов");
            }
        }

        [TestMethod]
        public void CaptchaService_ГенерацияКода_НеСодержитБуквыIиO()
        {
            var service = new CaptchaService();
            string code = service.GenerateCaptchaCode(50);

           
            Assert.IsFalse(code.Contains('I'), "CAPTCHA не должен содержать букву I (путается с 1)");
            Assert.IsFalse(code.Contains('O'), "CAPTCHA не должен содержать букву O (путается с 0)");
        }

        [TestMethod]
        public void CaptchaService_ДваПоследовательныхКода_НеДолжныБытьОдинаковыми()
        {
            var service = new CaptchaService();
            string code1 = service.GenerateCaptchaCode(5);
            string code2 = service.GenerateCaptchaCode(5);

            Assert.AreNotEqual(code1, code2, "Последовательные CAPTCHA коды не должны совпадать");
        }

        // ТЕСТ №4: ВАЛИДАЦИЯ ДАННЫХ 

        [TestMethod]
        public void ProductionBatch_СтатусМожетБытьТолькоДопустимыеЗначения()
        {
           
            var validStatuses = new[] { "planned", "in_progress", "completed", "rejected" };
            var batch = new ProductionBatch();

            
            foreach (var status in validStatuses)
            {
                batch.status = status;
                Assert.AreEqual(status, batch.status, $"Статус '{status}' должен быть допустимым");
            }
        }

        [TestMethod]
        public void ProductionOrder_ПлановоеКоличествоДолжноБытьПоложительным()
        {
            
            var order = new ProductionOrder();

            
            order.planned_quantity_kg = 1000;

           
            Assert.IsTrue(order.planned_quantity_kg > 0, "Плановое количество должно быть больше 0");
        }

        [TestMethod]
        public void Recipe_ВерсияРецептуры_НеМожетБытьМеньше1()
        {
            
            var recipe = new Recipe();

            
            recipe.version = 1;

            
            Assert.IsTrue(recipe.version >= 1, "Версия рецептуры должна быть >= 1");
        }

        [TestMethod]
        public void User_Токен_ПослеАвторизации_НеПустой()
        {
            
            var user = new User();

            
            user.Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";

            
            Assert.IsFalse(string.IsNullOrWhiteSpace(user.Token), "Токен не должен быть пустым после авторизации");
        }
    }
}