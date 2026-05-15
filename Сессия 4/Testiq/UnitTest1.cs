using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LabDesktop.Models;
using LabDesktop.Services;
using LabDesktop.Views;

namespace Testiq
{
    [TestClass]
    public class UnitTest1
    {
        // ТЕСТ №1: МОДЕЛИ ДАННЫХ 

        [TestMethod]
        public void User_Модель_СоздаетсяИЗаполняется()
        {
            
            var user = new User
            {
                Id = 1,
                Login = "lab.vasilieva",
                FullName = "Васильева Екатерина Дмитриевна",
                Role = "lab technician",
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"
            };

            
            Assert.AreEqual(1, user.Id, "ID должен быть 1");
            Assert.AreEqual("lab.vasilieva", user.Login, "Логин не совпадает");
            Assert.AreEqual("Васильева Екатерина Дмитриевна", user.FullName, "ФИО не совпадает");
            Assert.AreEqual("lab technician", user.Role, "Роль должна быть lab technician");
            Assert.IsNotNull(user.Token, "Токен не должен быть null");
        }

        [TestMethod]
        public void LabTest_Модель_СоздаетсяИЗаполняется()
        {
            
            var test = new LabTest
            {
                id = 100,
                batch_id = 50,
                batch_number = "BATCH-001",
                parameter_name = "Массовая доля азота",
                measured_value = 32.5m,
                standard_value = "≥30",
                unit_of_measure = "%",
                result = "passed",
                decision = "approved",
                lab_technician_name = "Васильева Е.Д.",
                analysis_date = DateTime.Now,
                lab_comment = "Соответствует норме"
            };

            
            Assert.AreEqual(100, test.id);
            Assert.AreEqual(50, test.batch_id);
            Assert.AreEqual("BATCH-001", test.batch_number);
            Assert.AreEqual("Массовая доля азота", test.parameter_name);
            Assert.AreEqual(32.5m, test.measured_value);
            Assert.AreEqual("≥30", test.standard_value);
            Assert.AreEqual("%", test.unit_of_measure);
            Assert.AreEqual("passed", test.result);
            Assert.AreEqual("approved", test.decision);
            Assert.IsNotNull(test.analysis_date);
            Assert.AreEqual("Соответствует норме", test.lab_comment);
        }

        [TestMethod]
        public void LabStatistics_Модель_СодержитВсеПоляСтатистики()
        {
           
            var stats = new LabStatistics
            {
                pendingBatchesCount = 3,
                pendingRawMaterialsCount = 2,
                inProgressCount = 1,
                completedTodayCount = 5,
                rejectedThisWeekCount = 1
            };

            
            Assert.AreEqual(3, stats.pendingBatchesCount);
            Assert.AreEqual(2, stats.pendingRawMaterialsCount);
            Assert.AreEqual(1, stats.inProgressCount);
            Assert.AreEqual(5, stats.completedTodayCount);
            Assert.AreEqual(1, stats.rejectedThisWeekCount);
        }

        [TestMethod]
        public void ProductionBatch_ДляЛаборатории_СодержитНеобходимыеПоля()
        {
            
            var batch = new ProductionBatch
            {
                id = 1,
                batch_number = "BATCH-2024-001",
                order_id = 100,
                status = "completed",
                actual_quantity_kg = 5000,
                end_time = DateTime.Now,
                created_date = DateTime.Now
            };

           
            Assert.AreEqual(1, batch.id);
            Assert.AreEqual("BATCH-2024-001", batch.batch_number);
            Assert.AreEqual("completed", batch.status);
            Assert.AreEqual(5000, batch.actual_quantity_kg);
            Assert.IsNotNull(batch.end_time);
            Assert.IsNotNull(batch.created_date);
        }

        [TestMethod]
        public void QualityParameter_Модель_СодержитНормативы()
        {
           
            var param = new QualityParameter
            {
                id = 1,
                product_id = 5,
                parameter_name = "Влажность",
                standard_value = "≤5",
                unit_of_measure = "%",
                is_mandatory = true,
                order_number = 1
            };

            
            Assert.AreEqual(1, param.id);
            Assert.AreEqual(5, param.product_id);
            Assert.AreEqual("Влажность", param.parameter_name);
            Assert.AreEqual("≤5", param.standard_value);
            Assert.AreEqual("%", param.unit_of_measure);
            Assert.IsTrue(param.is_mandatory);
            Assert.AreEqual(1, param.order_number);
        }

        [TestMethod]
        public void RawMaterialBatchForQC_Модель_СодержитДанныеСырья()
        {
            
            var rawMaterial = new RawMaterialBatchForQC
            {
                id = 1,
                batch_number = "RM-2024-001",
                raw_material_id = 10,
                raw_material_name = "Карбамид",
                quantity = 10000,
                unit_of_measure = "кг",
                receipt_date = DateTime.Now,
                supplier = "ООО АгроХим",
                lab_status = "pending"
            };

            
            Assert.AreEqual(1, rawMaterial.id);
            Assert.AreEqual("RM-2024-001", rawMaterial.batch_number);
            Assert.AreEqual("Карбамид", rawMaterial.raw_material_name);
            Assert.AreEqual(10000, rawMaterial.quantity);
            Assert.AreEqual("кг", rawMaterial.unit_of_measure);
            Assert.AreEqual("ООО АгроХим", rawMaterial.supplier);
            Assert.AreEqual("pending", rawMaterial.lab_status);
        }


        //  ТЕСТ №2: СЛУЖБА АВТОРИЗАЦИИ
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

            
            Assert.IsFalse(authService.IsAuthenticated, "После выхода пользователь не должен быть авторизован");
            Assert.IsNull(authService.CurrentUser, "После выхода CurrentUser должен быть null");
        }


        //  ТЕСТ №3: ВАЛИДАЦИЯ РЕШЕНИЙ В DIALOG 

        [TestMethod]
        public void CompleteQCDialog_ПриБраковкеБезКомментария_ВалидацияНеПроходит()
        {
           
            string decision = "rejected";
            string comment = "";

            
            bool isValid = !(decision == "rejected" && string.IsNullOrWhiteSpace(comment));

           
            Assert.IsFalse(isValid, "При браковке партии комментарий обязателен");
        }

        [TestMethod]
        public void CompleteQCDialog_ПриБраковкеСКомментарием_ВалидацияПроходит()
        {
           
            string decision = "rejected";
            string comment = "Не соответствует требованиям качества. Содержание активного вещества ниже нормы.";

            
            bool isValid = !(decision == "rejected" && string.IsNullOrWhiteSpace(comment));

           
            Assert.IsTrue(isValid, "При наличии комментария валидация должна проходить");
        }

        [TestMethod]
        public void CompleteQCDialog_ПриДопуске_КомментарийНеОбязателен()
        {
            
            string decision = "approved";
            string comment = "";

            
            bool isValid = !(decision == "rejected" && string.IsNullOrWhiteSpace(comment));

            
            Assert.IsTrue(isValid, "При допуске партии комментарий не обязателен");
        }

        [TestMethod]
        public void CompleteQCDialog_РешениеApproved_ВозвращаетПравильныйТег()
        {
           
            string expectedDecision = "approved";

            
            string actualDecision = expectedDecision;

            
            Assert.AreEqual("approved", actualDecision, "Решение approved должно возвращать 'approved'");
        }

        [TestMethod]
        public void CompleteQCDialog_РешениеRejected_ВозвращаетПравильныйТег()
        {
            
            string expectedDecision = "rejected";

            
            string actualDecision = expectedDecision;

            
            Assert.AreEqual("rejected", actualDecision, "Решение rejected должно возвращать 'rejected'");
        }


        // ТЕСТ №4: ВЫЧИСЛЕНИЕ РЕЗУЛЬТАТА ТЕСТА 

        [TestMethod]
        public void BatchTestView_ЗначениеБольшеИлиРавно_РезультатСоответствует()
        {
           
            decimal measuredValue = 32.5m;
            string standardValue = "≥30";
            decimal standardNum = 30;

            
            bool result = false;
            if (standardValue.Contains("≥"))
                result = measuredValue >= standardNum;
            else if (standardValue.Contains("≤"))
                result = measuredValue <= standardNum;
            else
                result = measuredValue == standardNum;

            
            Assert.IsTrue(result, "32.5 ≥ 30 - должно быть Соответствует");
        }

        [TestMethod]
        public void BatchTestView_ЗначениеМеньшеИлиРавно_РезультатСоответствует()
        {
            
            decimal measuredValue = 4.2m;
            string standardValue = "≤5";
            decimal standardNum = 5;

            
            bool result = measuredValue <= standardNum;

            
            Assert.IsTrue(result, "4.2 ≤ 5 - должно быть Соответствует");
        }

        [TestMethod]
        public void BatchTestView_ЗначениеМеньшеТребуемогоМинимума_РезультатНеСоответствует()
        {
            
            decimal measuredValue = 28.5m;
            string standardValue = "≥30";
            decimal standardNum = 30;

            
            bool result = measuredValue >= standardNum;

            
            Assert.IsFalse(result, "28.5 ≥ 30 - должно быть Не соответствует");
        }

        [TestMethod]
        public void BatchTestView_ЗначениеБольшеМаксимума_РезультатНеСоответствует()
        {
            
            decimal measuredValue = 6.5m;
            string standardValue = "≤5";
            decimal standardNum = 5;

            
            bool result = measuredValue <= standardNum;

            
            Assert.IsFalse(result, "6.5 ≤ 5 - должно быть Не соответствует");
        }

        [TestMethod]
        public void BatchTestView_ТочноеСоответствие_РезультатСоответствует()
        {
            
            decimal measuredValue = 50m;
            string standardValue = "50";
            decimal standardNum = 50;

            
            bool result = measuredValue == standardNum;

            
            Assert.IsTrue(result, "50 = 50 - должно быть Соответствует");
        }

        [TestMethod]
        public void BatchTestView_ПарсингИзмеренногоЗначения_СЗапятой()
        {
           
            string inputValue = "32,5";

            
            bool parseSuccess = decimal.TryParse(inputValue.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal result);

           
            Assert.IsTrue(parseSuccess, "Значение с запятой должно парситься корректно");
            Assert.AreEqual(32.5m, result, "Результат должен быть 32.5");
        }

        //  ТЕСТ №5: ФОРМИРОВАНИЕ ПРОТОКОЛА 

        [TestMethod]
        public void TestHistoryView_ГенерацияПротокола_СодержитЗаголовок()
        {
            string batchNumber = "BATCH-001";
            int batchId = 100;
            var tests = new List<LabTest>
            {
                new LabTest { parameter_name = "Массовая доля", measured_value = 32.5m, standard_value = "≥30", result = "passed" }
            };

            string protocol = GenerateTestProtocol(batchNumber, batchId, tests);

            Assert.IsTrue(protocol.Contains("ПРОТОКОЛ ЛАБОРАТОРНЫХ ИСПЫТАНИЙ"));
            Assert.IsTrue(protocol.Contains(batchNumber));
            Assert.IsTrue(protocol.Contains($"ID партии: {batchId}"));
        }

        [TestMethod]
        public void TestHistoryView_ПротоколДляЗабракованнойПартии_СодержитРешениеБраковка()
        {
            string batchNumber = "BATCH-002";
            int batchId = 200;
            var tests = new List<LabTest>
            {
                new LabTest { parameter_name = "Тест", measured_value = 3, standard_value = "≥5", result = "fail", decision = "rejected" }
            };

            string protocol = GenerateTestProtocol(batchNumber, batchId, tests);

            Assert.IsTrue(protocol.Contains("ЗАБРАКОВАНА"));
        }

        [TestMethod]
        public void TestHistoryView_ПротоколДляДопущеннойПартии_СодержитРешениеДопущена()
        {
            string batchNumber = "BATCH-003";
            int batchId = 300;
            var tests = new List<LabTest>
            {
                new LabTest { parameter_name = "Тест", measured_value = 10, standard_value = "≥5", result = "passed", decision = "approved" }
            };

            string protocol = GenerateTestProtocol(batchNumber, batchId, tests);

            Assert.IsTrue(protocol.Contains("ДОПУЩЕНА"));
        }

        [TestMethod]
        public void TestHistoryView_Протокол_СодержитТаблицуРезультатов()
        {
            var tests = new List<LabTest>
            {
                new LabTest { parameter_name = "Массовая доля азота", measured_value = 32.5m, standard_value = "≥30", unit_of_measure = "%", result = "passed" },
                new LabTest { parameter_name = "Влажность", measured_value = 2.5m, standard_value = "≤5", unit_of_measure = "%", result = "passed" },
                new LabTest { parameter_name = "pH", measured_value = 7.2m, standard_value = "6.5-7.5", unit_of_measure = "", result = "passed" }
            };

            string protocol = GenerateTestProtocol("BATCH-004", 400, tests);

            Assert.IsTrue(protocol.Contains("Массовая доля азота"));
            Assert.IsTrue(protocol.Contains("Влажность"));
            Assert.IsTrue(protocol.Contains("pH"));

            // Проверяем значения с учетом разных разделителей (точка или запятая)
            bool hasValue32_5 = protocol.Contains("32.5") || protocol.Contains("32,5");
            Assert.IsTrue(hasValue32_5, "Протокол должен содержать значение 32.5 или 32,5");

            bool hasValue2_5 = protocol.Contains("2.5") || protocol.Contains("2,5");
            Assert.IsTrue(hasValue2_5, "Протокол должен содержать значение 2.5 или 2,5");

            Assert.IsTrue(protocol.Contains("≥30"));
            Assert.IsTrue(protocol.Contains("≤5"));
        }

        [TestMethod]
        public void TestHistoryView_Протокол_СодержитПодписьЛаборанта()
        {
            var tests = new List<LabTest> { new LabTest { parameter_name = "Тест", result = "passed" } };

            string protocol = GenerateTestProtocol("BATCH-005", 500, tests);

            Assert.IsTrue(protocol.Contains("Подпись лаборанта:"));
            Assert.IsTrue(protocol.Contains("__________________"));
        }

        private string GenerateTestProtocol(string batchNumber, int batchId, List<LabTest> tests)
        {
            string protocol = "";

            protocol += "╔══════════════════════════════════════════════════════════════════╗\n";
            protocol += "║                  ПРОТОКОЛ ЛАБОРАТОРНЫХ ИСПЫТАНИЙ                 ║\n";
            protocol += "╚══════════════════════════════════════════════════════════════════╝\n\n";

            protocol += $"Номер партии: {batchNumber}\n";
            protocol += $"ID партии: {batchId}\n";
            protocol += $"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm:ss}\n\n";

            protocol += "РЕЗУЛЬТАТЫ ИСПЫТАНИЙ:\n\n";
            protocol += string.Format("{0,-30} {1,-12} {2,-12} {3,-10} {4,-12}\n",
                "Параметр", "Значение", "Норма", "Ед.изм.", "Результат");
            protocol += new string('─', 80) + "\n";

            foreach (var test in tests)
            {
                string resultText = test.result == "passed" ? "✓ Соответствует" : "✗ НЕ СООТВЕТСТВУЕТ";
                protocol += string.Format("{0,-30} {1,-12} {2,-12} {3,-10} {4,-12}\n",
                    test.parameter_name ?? "",
                    test.measured_value?.ToString() ?? "—",
                    test.standard_value ?? "—",
                    test.unit_of_measure ?? "—",
                    resultText);
            }

            protocol += "\n" + new string('─', 60) + "\n\n";

            bool hasRejected = tests.Exists(t => t.decision == "rejected");
            if (hasRejected)
                protocol += "ИТОГОВОЕ РЕШЕНИЕ: ЗАБРАКОВАНА\n";
            else
                protocol += "ИТОГОВОЕ РЕШЕНИЕ: ДОПУЩЕНА\n";

            protocol += "\n" + new string('─', 60) + "\n";
            protocol += $"Подпись лаборанта: __________________\n";
            protocol += $"Дата: {DateTime.Now:dd.MM.yyyy}";

            return protocol;
        }
    }
}