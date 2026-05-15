using System;
using System.Linq;
using System.Web.Http;
using AgroSintez.Models;

namespace AgroSintez.Controllers
{
    // [Authorize]
    [RoutePrefix("api/shopfloor")]
    public class ShopFloorController : ApiController
    {
        private AgroSintezEntities db;

        public ShopFloorController()
        {
            db = new AgroSintezEntities();
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        // ==================== ПРОСМОТР АКТИВНЫХ ПАРТИЙ ====================

        // GET: api/shopfloor/batches/active
        [HttpGet]
        [Route("batches/active")]
        public IHttpActionResult GetActiveBatches()
        {
            var batches = db.production_batches
                .Where(b => b.status == "in_progress" || b.status == "planned")
                .Select(b => new
                {
                    b.id,
                    b.batch_number,
                    b.order_id,
                    b.status,
                    b.start_time,
                    b.actual_quantity_kg
                })
                .ToList();

            return Ok(batches);
        }

        // GET: api/shopfloor/batches/{batchId}/steps
        [HttpGet]
        [Route("batches/{batchId}/steps")]
        public IHttpActionResult GetBatchSteps(int batchId)
        {
            var steps = db.batch_steps
                .Where(s => s.batch_id == batchId)
                .Select(s => new
                {
                    s.id,
                    s.step_number,
                    s.step_name,
                    s.planned_temperature,
                    s.actual_temperature,
                    s.planned_duration_min,
                    s.actual_duration_min,
                    s.planned_pressure,
                    s.actual_pressure,
                    s.has_deviation,
                    s.deviation_description,
                    s.operator_comment,
                    s.start_time,
                    s.end_time,
                    s.equipment_id
                })
                .OrderBy(s => s.step_number)
                .ToList();

            return Ok(steps);
        }

        // GET: api/shopfloor/batches/{batchId}/current-step
        [HttpGet]
        [Route("batches/{batchId}/current-step")]
        public IHttpActionResult GetCurrentStep(int batchId)
        {
            var currentStep = db.batch_steps
                .Where(s => s.batch_id == batchId && s.end_time == null)
                .OrderBy(s => s.step_number)
                .FirstOrDefault();

            if (currentStep == null)
                return Ok(new { message = "Все шаги выполнены" });

            return Ok(new
            {
                currentStep.id,
                currentStep.step_number,
                currentStep.step_name,
                currentStep.planned_temperature,
                currentStep.planned_duration_min,
                currentStep.planned_pressure,
                currentStep.start_time
            });
        }

        // ==================== ЗАПУСК ПАРТИИ ====================

        // POST: api/shopfloor/batches/{batchId}/start
        [HttpPost]
        [Route("batches/{batchId}/start")]
        public IHttpActionResult StartBatch(int batchId)
        {
            try
            {
                var batch = db.production_batches.Find(batchId);
                if (batch == null)
                    return NotFound();

                if (batch.status != "planned")
                    return BadRequest("Партия уже запущена или завершена");

                batch.status = "in_progress";
                batch.start_time = DateTime.Now;

                // Создаем шаги партии из техкарты
                var order = db.production_orders.Find(batch.order_id);
                if (order != null)
                {
                    var techSteps = db.tech_card_steps
                        .Where(s => s.tech_card_id == order.tech_card_id)
                        .OrderBy(s => s.step_number)
                        .ToList();

                    foreach (var step in techSteps)
                    {
                        var batchStep = new batch_steps
                        {
                            batch_id = batch.id,
                            tech_card_step_id = step.id,
                            equipment_id = step.equipment_id,
                            step_number = step.step_number,
                            step_name = step.step_name,
                            planned_temperature = step.planned_temp_min,
                            planned_duration_min = step.planned_duration_min,
                            planned_pressure = step.planned_pressure_min,
                            start_time = DateTime.Now,
                            has_deviation = 0
                        };
                        db.batch_steps.Add(batchStep);
                    }
                }

                db.SaveChanges();

                return Ok(new
                {
                    batch_id = batch.id,
                    batch_number = batch.batch_number,
                    status = batch.status,
                    message = "Партия запущена"
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // ==================== ВВОД ФАКТИЧЕСКИХ ДАННЫХ ====================

        // POST: api/shopfloor/steps/{stepId}/record
        [HttpPost]
        [Route("steps/{stepId}/record")]
        public IHttpActionResult RecordStep(int stepId, [FromBody] StepRecord record)
        {
            try
            {
                var step = db.batch_steps.Find(stepId);
                if (step == null)
                    return NotFound();

                // Записываем фактические параметры
                if (record.ActualTemperature.HasValue)
                    step.actual_temperature = record.ActualTemperature.Value;

                if (record.ActualDurationMin.HasValue)
                    step.actual_duration_min = record.ActualDurationMin.Value;

                if (record.ActualPressure.HasValue)
                    step.actual_pressure = record.ActualPressure.Value;

                step.operator_comment = record.Comment;
                step.end_time = DateTime.Now;

                // Проверяем отклонения
                CheckDeviation(step);

                step.has_deviation = step.deviation_description != null ? 1 : 0;

                db.SaveChanges();

                return Ok(new
                {
                    step_id = step.id,
                    step_number = step.step_number,
                    step_name = step.step_name,
                    has_deviation = step.has_deviation == 1,
                    deviation_description = step.deviation_description,
                    message = "Шаг выполнен"
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/shopfloor/steps/{stepId}/record-with-deviation
        [HttpPost]
        [Route("steps/{stepId}/record-with-deviation")]
        public IHttpActionResult RecordStepWithDeviation(int stepId, [FromBody] StepWithDeviation record)
        {
            try
            {
                var step = db.batch_steps.Find(stepId);
                if (step == null)
                    return NotFound();

                // Записываем фактические параметры
                if (record.ActualTemperature.HasValue)
                    step.actual_temperature = record.ActualTemperature.Value;

                if (record.ActualDurationMin.HasValue)
                    step.actual_duration_min = record.ActualDurationMin.Value;

                if (record.ActualPressure.HasValue)
                    step.actual_pressure = record.ActualPressure.Value;

                step.operator_comment = record.Comment;
                step.deviation_description = record.DeviationDescription;
                step.has_deviation = 1;
                step.end_time = DateTime.Now;

                db.SaveChanges();

                return Ok(new
                {
                    step_id = step.id,
                    step_number = step.step_number,
                    has_deviation = 1,
                    deviation_description = step.deviation_description,
                    message = "Шаг выполнен с фиксацией отклонения"
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // ==================== ЗАВЕРШЕНИЕ ВСЕЙ ПАРТИИ ====================

        // POST: api/shopfloor/batches/{batchId}/complete
        [HttpPost]
        [Route("batches/{batchId}/complete")]
        public IHttpActionResult CompleteBatch(int batchId, [FromBody] BatchComplete complete)
        {
            try
            {
                var batch = db.production_batches.Find(batchId);
                if (batch == null)
                    return NotFound();

                batch.status = "completed";
                batch.end_time = DateTime.Now;
                batch.actual_quantity_kg = complete.ActualQuantityKg;

                db.SaveChanges();

                return Ok(new
                {
                    batch_id = batch.id,
                    batch_number = batch.batch_number,
                    actual_quantity = batch.actual_quantity_kg,
                    message = "Партия завершена"
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // ==================== ПРОВЕРКА ОТКЛОНЕНИЙ ====================

        private void CheckDeviation(batch_steps step)
        {
            var techStep = db.tech_card_steps.Find(step.tech_card_step_id);
            if (techStep == null) return;

            // Проверка температуры
            if (step.actual_temperature.HasValue && techStep.planned_temp_min.HasValue && techStep.planned_temp_max.HasValue)
            {
                if (step.actual_temperature < techStep.planned_temp_min ||
                    step.actual_temperature > techStep.planned_temp_max)
                {
                    step.deviation_description = $"Температура {step.actual_temperature}°C вне нормы ({techStep.planned_temp_min}-{techStep.planned_temp_max}°C)";
                }
            }

            // Проверка давления
            if (step.actual_pressure.HasValue && techStep.planned_pressure_min.HasValue && techStep.planned_pressure_max.HasValue)
            {
                if (step.actual_pressure < techStep.planned_pressure_min ||
                    step.actual_pressure > techStep.planned_pressure_max)
                {
                    var devMsg = $"Давление {step.actual_pressure} вне нормы ({techStep.planned_pressure_min}-{techStep.planned_pressure_max})";
                    step.deviation_description = step.deviation_description == null ? devMsg : step.deviation_description + "; " + devMsg;
                }
            }
        }

        // ==================== DTO КЛАССЫ ====================

        public class StepRecord
        {
            public int? ActualTemperature { get; set; }
            public int? ActualDurationMin { get; set; }
            public decimal? ActualPressure { get; set; }
            public string Comment { get; set; }
        }

        public class StepWithDeviation
        {
            public int? ActualTemperature { get; set; }
            public int? ActualDurationMin { get; set; }
            public decimal? ActualPressure { get; set; }
            public string Comment { get; set; }
            public string DeviationDescription { get; set; }
        }

        public class BatchComplete
        {
            public int ActualQuantityKg { get; set; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}