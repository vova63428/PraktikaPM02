using System;
using System.Linq;
using System.Web.Http;
using AgroSintez.Models;

namespace AgroSintez.Controllers
{
    // [Authorize]
    [RoutePrefix("api/tech-cards")]
    public class TechCardsController : ApiController
    {
        private AgroSintezEntities db;

        public TechCardsController()
        {
            db = new AgroSintezEntities();
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        public class TechCardDto
        {
            public int? Id { get; set; }
            public int ProductId { get; set; }
            public string Status { get; set; }
            public int? ApprovedById { get; set; }
        }

        public class TechCardStepDto
        {
            public int? Id { get; set; }
            public int? EquipmentId { get; set; }
            public int StepNumber { get; set; }
            public string StepName { get; set; }
            public string StepType { get; set; }
            public int? PlannedTempMin { get; set; }
            public int? PlannedTempMax { get; set; }
            public int PlannedDurationMin { get; set; }
            public decimal? PlannedPressureMin { get; set; }
            public decimal? PlannedPressureMax { get; set; }
            public int? IsMandatory { get; set; }
            public string Instruction { get; set; }
        }

        // GET: api/tech-cards
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            var cards = db.technological_cards
                .Select(t => new
                {
                    t.id,
                    t.product_id,
                    t.version,
                    t.status,
                    t.approval_date,
                    t.approved_by_id,
                    t.created_date
                })
                .OrderByDescending(t => t.created_date)
                .ToList();

            return Ok(cards);
        }

        // GET: api/tech-cards/{id}
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var card = db.technological_cards
                .Where(t => t.id == id)
                .Select(t => new
                {
                    t.id,
                    t.product_id,
                    t.version,
                    t.status,
                    t.approval_date,
                    t.approved_by_id,
                    t.created_date
                })
                .FirstOrDefault();

            if (card == null)
                return NotFound();

            return Ok(card);
        }

        // GET: api/tech-cards/{id}/steps
        [HttpGet]
        [Route("{id}/steps")]
        public IHttpActionResult GetSteps(int id)
        {
            var steps = db.tech_card_steps
                .Where(s => s.tech_card_id == id)
                .Select(s => new
                {
                    s.id,
                    s.equipment_id,
                    s.step_number,
                    s.step_name,
                    s.step_type,
                    s.planned_temp_min,
                    s.planned_temp_max,
                    s.planned_duration_min,
                    s.planned_pressure_min,
                    s.planned_pressure_max,
                    s.is_mandatory,
                    s.instruction
                })
                .OrderBy(s => s.step_number)
                .ToList();

            // Добавляем информацию об оборудовании
            var result = steps.Select(s =>
            {
                var equipment = s.equipment_id.HasValue ? db.equipment.FirstOrDefault(e => e.id == s.equipment_id) : null;
                return new
                {
                    s.id,
                    s.equipment_id,
                    equipment_name = equipment?.name,
                    equipment_code = equipment?.code,
                    s.step_number,
                    s.step_name,
                    s.step_type,
                    s.planned_temp_min,
                    s.planned_temp_max,
                    s.planned_duration_min,
                    s.planned_pressure_min,
                    s.planned_pressure_max,
                    s.is_mandatory,
                    s.instruction
                };
            }).ToList();

            return Ok(result);
        }

        // POST: api/tech-cards
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(TechCardDto cardDto)
        {
            try
            {
                // Получаем максимальную версию для продукта
                int maxVersion = db.technological_cards
                    .Where(t => t.product_id == cardDto.ProductId)
                    .Select(t => t.version)
                    .DefaultIfEmpty(0)
                    .Max();

                var card = new technological_cards
                {
                    product_id = cardDto.ProductId,
                    version = maxVersion + 1,
                    status = cardDto.Status ?? "draft",
                    created_date = DateTime.Now
                };

                db.technological_cards.Add(card);
                db.SaveChanges();

                return Ok(new { id = card.id, message = "Technological card created", version = card.version });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/tech-cards/{id}/steps
        [HttpPost]
        [Route("{id}/steps")]
        public IHttpActionResult AddStep(int id, TechCardStepDto stepDto)
        {
            try
            {
                var card = db.technological_cards.Find(id);
                if (card == null)
                    return NotFound();

                var step = new tech_card_steps
                {
                    tech_card_id = id,
                    equipment_id = stepDto.EquipmentId,
                    step_number = stepDto.StepNumber,
                    step_name = stepDto.StepName,
                    step_type = stepDto.StepType,
                    planned_temp_min = stepDto.PlannedTempMin,
                    planned_temp_max = stepDto.PlannedTempMax,
                    planned_duration_min = stepDto.PlannedDurationMin,
                    planned_pressure_min = stepDto.PlannedPressureMin,
                    planned_pressure_max = stepDto.PlannedPressureMax,
                    is_mandatory = stepDto.IsMandatory ?? 1,
                    instruction = stepDto.Instruction
                };

                db.tech_card_steps.Add(step);
                db.SaveChanges();

                return Ok(new { id = step.id, message = "Step added" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT: api/tech-cards/steps/{stepId}
        [HttpPut]
        [Route("steps/{stepId}")]
        public IHttpActionResult UpdateStep(int stepId, TechCardStepDto stepDto)
        {
            try
            {
                var step = db.tech_card_steps.Find(stepId);
                if (step == null)
                    return NotFound();

                step.equipment_id = stepDto.EquipmentId;
                step.step_number = stepDto.StepNumber;
                step.step_name = stepDto.StepName;
                step.step_type = stepDto.StepType;
                step.planned_temp_min = stepDto.PlannedTempMin;
                step.planned_temp_max = stepDto.PlannedTempMax;
                step.planned_duration_min = stepDto.PlannedDurationMin;
                step.planned_pressure_min = stepDto.PlannedPressureMin;
                step.planned_pressure_max = stepDto.PlannedPressureMax;
                step.is_mandatory = stepDto.IsMandatory ?? 1;
                step.instruction = stepDto.Instruction;

                db.SaveChanges();

                return Ok(new { message = "Step updated" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE: api/tech-cards/steps/{stepId}
        [HttpDelete]
        [Route("steps/{stepId}")]
        public IHttpActionResult DeleteStep(int stepId)
        {
            try
            {
                var step = db.tech_card_steps.Find(stepId);
                if (step == null)
                    return NotFound();

                db.tech_card_steps.Remove(step);
                db.SaveChanges();

                return Ok(new { message = "Step deleted" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/tech-cards/{id}/approve
        [HttpPost]
        [Route("{id}/approve")]
        public IHttpActionResult Approve(int id)
        {
            try
            {
                var card = db.technological_cards.Find(id);
                if (card == null)
                    return NotFound();

                card.status = "approved";
                card.approval_date = DateTime.Now;
                card.approved_by_id = 1; // текущий пользователь

                db.SaveChanges();

                return Ok(new { message = "Technological card approved" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
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