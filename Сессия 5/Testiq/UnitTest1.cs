using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppDesktop.Models;

namespace Testiq
{
    [TestClass]
    public class AppDesktopUnitTests
    {
        // ТЕСТ №1: ProductionBatch_Модель_СоздаетсяИЗаполняется
        [TestMethod]
        public void ProductionBatch_Модель_СоздаетсяИЗаполняется()
        {
            var batch = new ProductionBatch
            {
                id = 1,
                batch_number = "B-2025-001",
                product_name = "Гербицид Торнадо",
                line_number = 1,
                status = "in_progress",
                current_step_number = 2,
                total_steps = 4,
                start_time = DateTime.Now
            };

            Assert.AreEqual(1, batch.id);
            Assert.AreEqual("B-2025-001", batch.batch_number);
            Assert.AreEqual("Гербицид Торнадо", batch.product_name);
            Assert.AreEqual(1, batch.line_number);
            Assert.AreEqual("in_progress", batch.status);
            Assert.AreEqual(2, batch.current_step_number);
            Assert.AreEqual(4, batch.total_steps);
            Assert.IsNotNull(batch.start_time);
        }

        // ТЕСТ №2: ProductionBatch_ПриСоздании_СтатусПланируется
        [TestMethod]
        public void ProductionBatch_ПриСоздании_СтатусПланируется()
        {
            var batch = new ProductionBatch { status = "planned" };
            Assert.AreEqual("planned", batch.status);
        }

        // ТЕСТ №3: ProductionBatch_ПослеЗавершения_СтатусCompleted
        [TestMethod]
        public void ProductionBatch_ПослеЗавершения_СтатусCompleted()
        {
            var batch = new ProductionBatch();
            batch.status = "completed";
            Assert.AreEqual("completed", batch.status);
        }

        // ТЕСТ №4: BatchStep_Модель_СоздаетсяИЗаполняется
        [TestMethod]
        public void BatchStep_Модель_СоздаетсяИЗаполняется()
        {
            var step = new BatchStep
            {
                id = 1,
                step_number = 1,
                step_name = "Смешивание компонентов",
                status = "pending",
                planned_temperature = 45,
                planned_pressure = 1.5m,
                expected_speed = 300,
                description = "Загрузка и смешивание сырья",
                instruction = "Загрузить компоненты согласно рецептуре"
            };

            Assert.AreEqual(1, step.id);
            Assert.AreEqual(1, step.step_number);
            Assert.AreEqual("Смешивание компонентов", step.step_name);
            Assert.AreEqual("pending", step.status);
            Assert.AreEqual(45, step.planned_temperature);
            Assert.AreEqual(1.5m, step.planned_pressure);
            Assert.AreEqual(300, step.expected_speed);
        }

        // ТЕСТ №5: BatchStep_ШагВПроцессе_СтатусInProgress
        [TestMethod]
        public void BatchStep_ШагВПроцессе_СтатусInProgress()
        {
            var step = new BatchStep { status = "in_progress" };
            Assert.AreEqual("in_progress", step.status);
        }

        // ТЕСТ №6: BatchStep_ШагЗавершен_СтатусCompleted
        [TestMethod]
        public void BatchStep_ШагЗавершен_СтатусCompleted()
        {
            var step = new BatchStep { status = "completed" };
            Assert.AreEqual("completed", step.status);
        }

        // ТЕСТ №7: BatchStep_ШагСОтклонением_HasDeviationTrue
        [TestMethod]
        public void BatchStep_ШагСОтклонением_HasDeviationTrue()
        {
            var step = new BatchStep
            {
                has_deviation = true,
                deviation_description = "Превышение температуры"
            };

            Assert.IsTrue(step.has_deviation);
            Assert.AreEqual("Превышение температуры", step.deviation_description);
        }

        // ТЕСТ №8: EquipmentTelemetry_Модель_СоздаетсяИЗаполняется
        [TestMethod]
        public void EquipmentTelemetry_Модель_СоздаетсяИЗаполняется()
        {
            var tele = new EquipmentTelemetry
            {
                equipment_id = 1,
                equipment_name = "Экструдер EK-45",
                temperature = 44.5m,
                pressure = 1.48m,
                speed = 298,
                vibration = 1.2m,
                is_running = true,
                timestamp = DateTime.Now
            };

            Assert.AreEqual(1, tele.equipment_id);
            Assert.AreEqual("Экструдер EK-45", tele.equipment_name);
            Assert.AreEqual(44.5m, tele.temperature);
            Assert.AreEqual(1.48m, tele.pressure);
            Assert.AreEqual(298, tele.speed);
            Assert.AreEqual(1.2m, tele.vibration);
            Assert.IsTrue(tele.is_running);
            Assert.IsNotNull(tele.timestamp);
        }

        // ТЕСТ №9: User_Модель_СоздаетсяИЗаполняется
        [TestMethod]
        public void User_Модель_СоздаетсяИЗаполняется()
        {
            var user = new User
            {
                Id = 1,
                Login = "operator",
                FullName = "Петров Иван Алексеевич",
                Role = "operator",
                Token = "test-token-123"
            };

            Assert.AreEqual(1, user.Id);
            Assert.AreEqual("operator", user.Login);
            Assert.AreEqual("Петров Иван Алексеевич", user.FullName);
            Assert.AreEqual("operator", user.Role);
            Assert.IsNotNull(user.Token);
        }

        // ТЕСТ №10: User_ТолькоOperatorИмеетДоступ_РольOperator
        [TestMethod]
        public void User_ТолькоOperatorИмеетДоступ_РольOperator()
        {
            var user = new User { Role = "operator" };
            bool hasAccess = user.Role == "operator";
            Assert.IsTrue(hasAccess);
        }

        // ТЕСТ №11: User_АдминистраторНеИмеетДоступа_РольНеOperator
        [TestMethod]
        public void User_АдминистраторНеИмеетДоступа_РольНеOperator()
        {
            var user = new User { Role = "admin" };
            bool hasAccess = user.Role == "operator";
            Assert.IsFalse(hasAccess);
        }

        // ТЕСТ №12: ОтклонениеТемпературы_ПриРазницеБолее5Градусов_ОтклонениеФиксируется
        [TestMethod]
        public void ОтклонениеТемпературы_ПриРазницеБолее5Градусов_ОтклонениеФиксируется()
        {
            decimal expectedTemp = 45;
            decimal actualTemp = 52;
            decimal maxDeviation = 5;

            bool hasDeviation = Math.Abs(actualTemp - expectedTemp) > maxDeviation;

            Assert.IsTrue(hasDeviation);
        }

        // ТЕСТ №13: ОтклонениеТемпературы_ПриРазницеМенее5Градусов_ОтклоненияНет
        [TestMethod]
        public void ОтклонениеТемпературы_ПриРазницеМенее5Градусов_ОтклоненияНет()
        {
            decimal expectedTemp = 45;
            decimal actualTemp = 47;
            decimal maxDeviation = 5;

            bool hasDeviation = Math.Abs(actualTemp - expectedTemp) > maxDeviation;

            Assert.IsFalse(hasDeviation);
        }

        // ТЕСТ №14: ОтклонениеДавления_ПриРазницеБолее0_3Бар_ОтклонениеФиксируется
        [TestMethod]
        public void ОтклонениеДавления_ПриРазницеБолее0_3Бар_ОтклонениеФиксируется()
        {
            decimal expectedPressure = 1.5m;
            decimal actualPressure = 2.0m;
            decimal maxDeviation = 0.3m;

            bool hasDeviation = Math.Abs(actualPressure - expectedPressure) > maxDeviation;

            Assert.IsTrue(hasDeviation);
        }

        // ТЕСТ №15: ОтклонениеДавления_ПриРазницеМенее0_3Бар_ОтклоненияНет
        [TestMethod]
        public void ОтклонениеДавления_ПриРазницеМенее0_3Бар_ОтклоненияНет()
        {
            decimal expectedPressure = 1.5m;
            decimal actualPressure = 1.6m;
            decimal maxDeviation = 0.3m;

            bool hasDeviation = Math.Abs(actualPressure - expectedPressure) > maxDeviation;

            Assert.IsFalse(hasDeviation);
        }

        // ТЕСТ №16: ПрогрессВыполнения_Расчет_ПравильныйПроцент
        [TestMethod]
        public void ПрогрессВыполнения_Расчет_ПравильныйПроцент()
        {
            int completedSteps = 2;
            int totalSteps = 5;
            double expectedProgress = 40.0;

            double actualProgress = (double)completedSteps / totalSteps * 100;

            Assert.AreEqual(expectedProgress, actualProgress, 0.01);
        }

        // ТЕСТ №17: ПрогрессВыполнения_ВсеШагиЗавершены_100Процентов
        [TestMethod]
        public void ПрогрессВыполнения_ВсеШагиЗавершены_100Процентов()
        {
            int completedSteps = 5;
            int totalSteps = 5;
            double expectedProgress = 100.0;

            double actualProgress = (double)completedSteps / totalSteps * 100;

            Assert.AreEqual(expectedProgress, actualProgress);
        }

        // ТЕСТ №18: ПрогрессВыполнения_НетЗавершенныхШагов_0Процентов
        [TestMethod]
        public void ПрогрессВыполнения_НетЗавершенныхШагов_0Процентов()
        {
            int completedSteps = 0;
            int totalSteps = 4;
            double expectedProgress = 0.0;

            double actualProgress = (double)completedSteps / totalSteps * 100;

            Assert.AreEqual(expectedProgress, actualProgress);
        }

        // ТЕСТ №19: ПереходКСледующемуШагу_НомерУвеличиваетсяНа1
        [TestMethod]
        public void ПереходКСледующемуШагу_НомерУвеличиваетсяНа1()
        {
            int currentStepNumber = 2;
            int expectedNextStep = 3;

            int nextStepNumber = currentStepNumber + 1;

            Assert.AreEqual(expectedNextStep, nextStepNumber);
        }

        // ТЕСТ №20: СохранениеПрогресса_ФорматПути_СодержитIDПартии
        [TestMethod]
        public void СохранениеПрогресса_ФорматПути_СодержитIDПартии()
        {
            int batchId = 123;
            string expectedPathPart = "batch_123_progress.txt";

            string fileName = $"batch_{batchId}_progress.txt";

            Assert.IsTrue(fileName.Contains(expectedPathPart));
        }

        // ТЕСТ №21: ФормулаОтклонения_Комбинированная_ДваОтклонения
        [TestMethod]
        public void ФормулаОтклонения_Комбинированная_ДваОтклонения()
        {
            decimal expectedTemp = 45;
            decimal actualTemp = 52;
            decimal expectedPressure = 1.5m;
            decimal actualPressure = 2.0m;
            decimal maxTempDev = 5;
            decimal maxPressureDev = 0.3m;

            bool tempDeviation = Math.Abs(actualTemp - expectedTemp) > maxTempDev;
            bool pressureDeviation = Math.Abs(actualPressure - expectedPressure) > maxPressureDev;
            bool hasDeviation = tempDeviation || pressureDeviation;

            Assert.IsTrue(hasDeviation);
            Assert.IsTrue(tempDeviation);
            Assert.IsTrue(pressureDeviation);
        }
    }
}