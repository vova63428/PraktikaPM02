using System;
using System.Linq;
using System.Web.Http;
using AgroSintez.Models;

namespace AgroSintez.Controllers
{
    // [Authorize]
    [RoutePrefix("api/lab-tests")]
    public class LabTestsController : ApiController
    {
        private AgroSintezEntities db;

        public LabTestsController()
        {
            db = new AgroSintezEntities();
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        // ==================== ПРОСМОТР ====================

        // GET: api/lab-tests
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            var tests = db.quality_control
                .Select(t => new
                {
                    t.id,
                    t.batch_id,
                    t.raw_material_batch_id,
                    t.lab_technician_id,
                    t.analysis_date,
                    t.sample_type,
                    t.parameter_name,
                    t.measured_value,
                    t.standard_value,
                    t.unit_of_measure,
                    t.result,
                    t.decision,
                    t.lab_comment
                })
                .OrderByDescending(t => t.analysis_date)
                .ToList();

            return Ok(tests);
        }

        // GET: api/lab-tests/{id}
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var test = db.quality_control
                .Where(t => t.id == id)
                .Select(t => new
                {
                    t.id,
                    t.batch_id,
                    t.raw_material_batch_id,
                    t.lab_technician_id,
                    t.analysis_date,
                    t.sample_type,
                    t.parameter_name,
                    t.measured_value,
                    t.standard_value,
                    t.unit_of_measure,
                    t.result,
                    t.decision,
                    t.lab_comment
                })
                .FirstOrDefault();

            if (test == null)
                return NotFound();

            return Ok(test);
        }

        // GET: api/lab-tests/batch/{batchId}
        [HttpGet]
        [Route("batch/{batchId}")]
        public IHttpActionResult GetByBatch(int batchId)
        {
            var tests = db.quality_control
                .Where(t => t.batch_id == batchId)
                .Select(t => new
                {
                    t.id,
                    t.parameter_name,
                    t.measured_value,
                    t.standard_value,
                    t.unit_of_measure,
                    t.result,
                    t.decision,
                    t.analysis_date,
                    t.lab_comment
                })
                .OrderByDescending(t => t.analysis_date)
                .ToList();

            return Ok(tests);
        }

        // GET: api/lab-tests/raw-material/{rawMaterialBatchId}
        [HttpGet]
        [Route("raw-material/{rawMaterialBatchId}")]
        public IHttpActionResult GetByRawMaterialBatch(int rawMaterialBatchId)
        {
            var tests = db.quality_control
                .Where(t => t.raw_material_batch_id == rawMaterialBatchId)
                .Select(t => new
                {
                    t.id,
                    t.parameter_name,
                    t.measured_value,
                    t.standard_value,
                    t.unit_of_measure,
                    t.result,
                    t.decision,
                    t.analysis_date,
                    t.lab_comment
                })
                .OrderByDescending(t => t.analysis_date)
                .ToList();

            return Ok(tests);
        }

        // GET: api/lab-tests/pending
        [HttpGet]
        [Route("pending")]
        public IHttpActionResult GetPendingTests()
        {
            // Партии, ожидающие лабораторного контроля
            var pendingBatches = db.production_batches
                .Where(b => b.status == "completed" &&
                       !db.quality_control.Any(q => q.batch_id == b.id))
                .Select(b => new
                {
                    b.id,
                    b.batch_number,
                    b.order_id,
                    b.actual_quantity_kg,
                    b.end_time
                })
                .ToList();

            // Сырье, ожидающее лабораторного контроля
            var pendingRawMaterials = db.raw_material_batches
                .Where(r => r.lab_status == "pending" &&
                       !db.quality_control.Any(q => q.raw_material_batch_id == r.id))
                .Select(r => new
                {
                    r.id,
                    r.batch_number,
                    r.raw_material_id,
                    r.quantity,
                    r.supplier,
                    r.receipt_date
                })
                .ToList();

            return Ok(new
            {
                pending_batches = pendingBatches,
                pending_raw_materials = pendingRawMaterials
            });
        }

        // ==================== РЕГИСТРАЦИЯ ТЕСТОВ ====================

        // POST: api/lab-tests/for-batch
        [HttpPost]
        [Route("for-batch")]
        public IHttpActionResult CreateTestForBatch([FromBody] BatchTestDto testDto)
        {
            try
            {
                if (testDto == null)
                    return BadRequest("Нет данных");

                var batch = db.production_batches.Find(testDto.BatchId);
                if (batch == null)
                    return NotFound();

                var test = new quality_control
                {
                    batch_id = testDto.BatchId,
                    lab_technician_id = testDto.LabTechnicianId,
                    analysis_date = DateTime.Now,
                    sample_type = testDto.SampleType,
                    parameter_name = testDto.ParameterName,
                    measured_value = testDto.MeasuredValue,
                    standard_value = testDto.StandardValue,
                    unit_of_measure = testDto.UnitOfMeasure,
                    lab_comment = testDto.LabComment
                };

                // Определяем результат
                test.result = DetermineResult(test.measured_value, test.standard_value);
                test.decision = test.result == "pass" ? "approved" : "rejected";

                db.quality_control.Add(test);
                db.SaveChanges();

                // Проверяем, все ли тесты пройдены для партии
                CheckBatchQuality(testDto.BatchId);

                return Ok(new
                {
                    test.id,
                    test.result,
                    test.decision,
                    message = "Тест зарегистрирован"
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/lab-tests/for-raw-material
        [HttpPost]
        [Route("for-raw-material")]
        public IHttpActionResult CreateTestForRawMaterial([FromBody] RawMaterialTestDto testDto)
        {
            try
            {
                if (testDto == null)
                    return BadRequest("Нет данных");

                var rawMaterialBatch = db.raw_material_batches.Find(testDto.RawMaterialBatchId);
                if (rawMaterialBatch == null)
                    return NotFound();

                var test = new quality_control
                {
                    raw_material_batch_id = testDto.RawMaterialBatchId,
                    lab_technician_id = testDto.LabTechnicianId,
                    analysis_date = DateTime.Now,
                    sample_type = testDto.SampleType,
                    parameter_name = testDto.ParameterName,
                    measured_value = testDto.MeasuredValue,
                    standard_value = testDto.StandardValue,
                    unit_of_measure = testDto.UnitOfMeasure,
                    lab_comment = testDto.LabComment
                };

                // Определяем результат
                test.result = DetermineResult(test.measured_value, test.standard_value);
                test.decision = test.result == "pass" ? "approved" : "rejected";

                db.quality_control.Add(test);
                db.SaveChanges();

                // Обновляем статус партии сырья
                UpdateRawMaterialStatus(testDto.RawMaterialBatchId);

                return Ok(new
                {
                    test.id,
                    test.result,
                    message = "Тест сырья зарегистрирован"
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // ==================== ПАКЕТНАЯ РЕГИСТРАЦИЯ ====================

        // POST: api/lab-tests/batch/{batchId}/tests
        [HttpPost]
        [Route("batch/{batchId}/tests")]
        public IHttpActionResult CreateMultipleTests(int batchId, [FromBody] MultipleTestsDto testsDto)
        {
            try
            {
                var batch = db.production_batches.Find(batchId);
                if (batch == null)
                    return NotFound();

                var createdTests = new System.Collections.Generic.List<object>();

                foreach (var testDto in testsDto.Tests)
                {
                    var test = new quality_control
                    {
                        batch_id = batchId,
                        lab_technician_id = testsDto.LabTechnicianId,
                        analysis_date = DateTime.Now,
                        sample_type = testDto.SampleType,
                        parameter_name = testDto.ParameterName,
                        measured_value = testDto.MeasuredValue,
                        standard_value = testDto.StandardValue,
                        unit_of_measure = testDto.UnitOfMeasure,
                        lab_comment = testDto.LabComment
                    };

                    test.result = DetermineResult(test.measured_value, test.standard_value);
                    test.decision = test.result == "pass" ? "approved" : "rejected";

                    db.quality_control.Add(test);
                    createdTests.Add(new { test.id, test.parameter_name, test.result });
                }

                db.SaveChanges();

                // Проверяем, все ли тесты пройдены для партии
                CheckBatchQuality(batchId);

                return Ok(new
                {
                    tests_count = createdTests.Count,
                    tests = createdTests,
                    message = "Тесты зарегистрированы"
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // ==================== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ====================

        private string DetermineResult(decimal? measuredValue, string standardValue)
        {
            if (!measuredValue.HasValue || string.IsNullOrEmpty(standardValue))
                return "pending";

            // Проверка на диапазон (например: "40-50")
            if (standardValue.Contains("-"))
            {
                var parts = standardValue.Split('-');
                if (decimal.TryParse(parts[0], out decimal min) && decimal.TryParse(parts[1], out decimal max))
                {
                    if (measuredValue >= min && measuredValue <= max)
                        return "pass";
                    else
                        return "fail";
                }
            }

            // Проверка на "меньше" (например: "<5")
            if (standardValue.StartsWith("<"))
            {
                if (decimal.TryParse(standardValue.Substring(1), out decimal max))
                {
                    return measuredValue < max ? "pass" : "fail";
                }
            }

            // Проверка на "больше" (например: ">90")
            if (standardValue.StartsWith(">"))
            {
                if (decimal.TryParse(standardValue.Substring(1), out decimal min))
                {
                    return measuredValue > min ? "pass" : "fail";
                }
            }

            // Точное совпадение
            if (decimal.TryParse(standardValue, out decimal exact))
            {
                return measuredValue == exact ? "pass" : "fail";
            }

            return "pending";
        }

        private void CheckBatchQuality(int batchId)
        {
            var allTests = db.quality_control.Where(t => t.batch_id == batchId).ToList();
            if (allTests.Any())
            {
                var hasFailure = allTests.Any(t => t.result == "fail");
                var hasPending = allTests.Any(t => t.result == "pending");

                var batch = db.production_batches.Find(batchId);
                if (batch != null)
                {
                    if (hasFailure)
                    {
                        batch.status = "rejected";
                    }
                    else if (!hasPending && allTests.All(t => t.result == "pass"))
                    {
                        batch.status = "completed";
                    }
                    db.SaveChanges();
                }
            }
        }

        private void UpdateRawMaterialStatus(int rawMaterialBatchId)
        {
            var allTests = db.quality_control
                .Where(t => t.raw_material_batch_id == rawMaterialBatchId)
                .ToList();

            if (allTests.Any())
            {
                var hasFailure = allTests.Any(t => t.result == "fail");
                var rawMaterialBatch = db.raw_material_batches.Find(rawMaterialBatchId);

                if (rawMaterialBatch != null)
                {
                    if (hasFailure)
                    {
                        rawMaterialBatch.lab_status = "rejected";
                    }
                    else if (allTests.All(t => t.result == "pass"))
                    {
                        rawMaterialBatch.lab_status = "approved";
                    }
                    db.SaveChanges();
                }
            }
        }

        // ==================== DTO КЛАССЫ ====================

        public class BatchTestDto
        {
            public int BatchId { get; set; }
            public int LabTechnicianId { get; set; }
            public string SampleType { get; set; }
            public string ParameterName { get; set; }
            public decimal? MeasuredValue { get; set; }
            public string StandardValue { get; set; }
            public string UnitOfMeasure { get; set; }
            public string LabComment { get; set; }
        }

        public class RawMaterialTestDto
        {
            public int RawMaterialBatchId { get; set; }
            public int LabTechnicianId { get; set; }
            public string SampleType { get; set; }
            public string ParameterName { get; set; }
            public decimal? MeasuredValue { get; set; }
            public string StandardValue { get; set; }
            public string UnitOfMeasure { get; set; }
            public string LabComment { get; set; }
        }

        public class SingleTestDto
        {
            public string SampleType { get; set; }
            public string ParameterName { get; set; }
            public decimal? MeasuredValue { get; set; }
            public string StandardValue { get; set; }
            public string UnitOfMeasure { get; set; }
            public string LabComment { get; set; }
        }

        public class MultipleTestsDto
        {
            public int LabTechnicianId { get; set; }
            public System.Collections.Generic.List<SingleTestDto> Tests { get; set; }
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