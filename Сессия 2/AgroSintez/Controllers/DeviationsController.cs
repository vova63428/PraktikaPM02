using System;
using System.Linq;
using System.Web.Http;
using AgroSintez.Models;

namespace AgroSintez.Controllers
{
    // [Authorize]
    [RoutePrefix("api/deviations")]
    public class DeviationsController : ApiController
    {
        private AgroSintezEntities db;

        public DeviationsController()
        {
            db = new AgroSintezEntities();
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        // GET: api/deviations
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllDeviations()
        {
            var deviations = db.batch_steps
                .Where(s => s.has_deviation == 1 && s.deviation_description != null)
                .Select(s => new
                {
                    s.id,
                    s.batch_id,
                    s.step_number,
                    s.step_name,
                    s.deviation_description,
                    s.operator_comment,
                    s.start_time,
                    s.end_time,
                    s.actual_temperature,
                    s.planned_temperature,
                    s.actual_pressure,
                    s.planned_pressure
                })
                .OrderByDescending(s => s.end_time)
                .ToList();

            return Ok(deviations);
        }

        // GET: api/deviations/batch/{batchId}
        [HttpGet]
        [Route("batch/{batchId}")]
        public IHttpActionResult GetBatchDeviations(int batchId)
        {
            var deviations = db.batch_steps
                .Where(s => s.batch_id == batchId && s.has_deviation == 1)
                .Select(s => new
                {
                    s.id,
                    s.step_number,
                    s.step_name,
                    s.deviation_description,
                    s.operator_comment,
                    s.actual_temperature,
                    s.planned_temperature,
                    s.actual_duration_min,
                    s.planned_duration_min,
                    s.actual_pressure,
                    s.planned_pressure,
                    s.start_time,
                    s.end_time
                })
                .OrderBy(s => s.step_number)
                .ToList();

            return Ok(deviations);
        }

        // GET: api/deviations/events
        [HttpGet]
        [Route("events")]
        public IHttpActionResult GetAllEvents()
        {
            // Получаем данные из БД без форматирования
            var batchEventsRaw = db.production_batches
                .Select(b => new
                {
                    type = "batch",
                    b.id,
                    b.batch_number,
                    b.status,
                    event_date = b.start_time ?? b.created_date
                })
                .ToList();

            var orderEventsRaw = db.production_orders
                .Select(o => new
                {
                    type = "order",
                    o.id,
                    o.order_number,
                    o.status,
                    event_date = o.created_date
                })
                .ToList();

            // Форматируем сообщения в памяти (после получения из БД)
            var batchEvents = batchEventsRaw.Select(b => new
            {
                b.type,
                b.id,
                b.batch_number,
                b.status,
                b.event_date,
                message = "Партия " + b.batch_number + " - статус: " + b.status
            });

            var orderEvents = orderEventsRaw.Select(o => new
            {
                o.type,
                o.id,
                o.order_number,
                o.status,
                o.event_date,
                message = "Заказ " + o.order_number + " - статус: " + o.status
            });

            var allEvents = batchEvents.Cast<object>()
                .Concat(orderEvents.Cast<object>())
                .OrderByDescending(e => (DateTime?)e.GetType().GetProperty("event_date")?.GetValue(e))
                .Take(50)
                .ToList();

            return Ok(allEvents);
        }

        // GET: api/deviations/events/batch/{batchId}
        [HttpGet]
        [Route("events/batch/{batchId}")]
        public IHttpActionResult GetBatchEvents(int batchId)
        {
            var batch = db.production_batches.Find(batchId);
            if (batch == null)
                return NotFound();

            var steps = db.batch_steps
                .Where(s => s.batch_id == batchId)
                .Select(s => new
                {
                    type = "step",
                    s.step_number,
                    s.step_name,
                    s.has_deviation,
                    s.deviation_description,
                    s.start_time,
                    s.end_time,
                    status = s.end_time != null ? "completed" : "in_progress"
                })
                .OrderBy(s => s.step_number)
                .ToList();

            return Ok(new
            {
                batch = new
                {
                    batch.id,
                    batch.batch_number,
                    batch.status,
                    batch.start_time,
                    batch.end_time
                },
                steps = steps
            });
        }

        // GET: api/deviations/notifications
        [HttpGet]
        [Route("notifications")]
        public IHttpActionResult GetAllNotifications()
        {
            var notifications = db.notifications
                .Select(n => new
                {
                    n.id,
                    n.user_id,
                    n.type,
                    n.title,
                    n.message,
                    n.is_read,
                    n.entity_type,
                    n.entity_id,
                    n.created_date
                })
                .OrderByDescending(n => n.created_date)
                .ToList();

            return Ok(notifications);
        }

        // GET: api/deviations/notifications/unread
        [HttpGet]
        [Route("notifications/unread")]
        public IHttpActionResult GetUnreadNotifications()
        {
            var notifications = db.notifications
                .Where(n => n.is_read == 0)
                .Select(n => new
                {
                    n.id,
                    n.user_id,
                    n.type,
                    n.title,
                    n.message,
                    n.created_date
                })
                .OrderByDescending(n => n.created_date)
                .ToList();

            return Ok(notifications);
        }

        // GET: api/deviations/notifications/user/{userId}
        [HttpGet]
        [Route("notifications/user/{userId}")]
        public IHttpActionResult GetUserNotifications(int userId)
        {
            var notifications = db.notifications
                .Where(n => n.user_id == userId)
                .Select(n => new
                {
                    n.id,
                    n.type,
                    n.title,
                    n.message,
                    n.is_read,
                    n.entity_type,
                    n.entity_id,
                    n.created_date
                })
                .OrderByDescending(n => n.created_date)
                .ToList();

            return Ok(notifications);
        }

        // PUT: api/deviations/notifications/{id}/read
        [HttpPut]
        [Route("notifications/{id}/read")]
        public IHttpActionResult MarkAsRead(int id)
        {
            try
            {
                var notification = db.notifications.Find(id);
                if (notification == null)
                    return NotFound();

                notification.is_read = 1;
                db.SaveChanges();

                return Ok(new { message = "Уведомление отмечено как прочитанное" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/deviations/notifications
        [HttpPost]
        [Route("notifications")]
        public IHttpActionResult CreateNotification([FromBody] NotificationDto notifyDto)
        {
            try
            {
                var notification = new notifications
                {
                    user_id = notifyDto.UserId,
                    type = notifyDto.Type,
                    title = notifyDto.Title,
                    message = notifyDto.Message,
                    entity_type = notifyDto.EntityType,
                    entity_id = notifyDto.EntityId,
                    created_date = DateTime.Now,
                    is_read = 0
                };

                db.notifications.Add(notification);
                db.SaveChanges();

                return Ok(new { id = notification.id, message = "Уведомление создано" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/deviations/resolve/{deviationId}
        [HttpPost]
        [Route("resolve/{deviationId}")]
        public IHttpActionResult ResolveDeviation(int deviationId, [FromBody] ResolveDto resolveDto)
        {
            try
            {
                var step = db.batch_steps.Find(deviationId);
                if (step == null)
                    return NotFound();

                step.deviation_description = step.deviation_description + " | РЕШЕНО: " + resolveDto.Resolution;
                step.operator_comment = resolveDto.Comment;

                db.SaveChanges();

                return Ok(new { message = "Отклонение решено" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/deviations/notifications/send-to-technologists
        [HttpPost]
        [Route("notifications/send-to-technologists")]
        public IHttpActionResult SendToTechnologists([FromBody] SendNotificationDto notifyDto)
        {
            try
            {
                var technologists = db.users
                    .Where(u => u.role == "technologist" && u.is_active == 1)
                    .ToList();

                var createdIds = new System.Collections.Generic.List<int>();

                foreach (var tech in technologists)
                {
                    var notification = new notifications
                    {
                        user_id = tech.id,
                        type = notifyDto.Type,
                        title = notifyDto.Title,
                        message = notifyDto.Message,
                        entity_type = notifyDto.EntityType,
                        entity_id = notifyDto.EntityId,
                        created_date = DateTime.Now,
                        is_read = 0
                    };

                    db.notifications.Add(notification);
                    createdIds.Add(notification.id);
                }

                db.SaveChanges();

                return Ok(new { count = createdIds.Count, message = $"Уведомления отправлены {createdIds.Count} технологам" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/deviations/notifications/send-to-operators
        [HttpPost]
        [Route("notifications/send-to-operators")]
        public IHttpActionResult SendToOperators([FromBody] SendNotificationDto notifyDto)
        {
            try
            {
                var operators = db.users
                    .Where(u => u.role == "operator" && u.is_active == 1)
                    .ToList();

                var createdIds = new System.Collections.Generic.List<int>();

                foreach (var op in operators)
                {
                    var notification = new notifications
                    {
                        user_id = op.id,
                        type = notifyDto.Type,
                        title = notifyDto.Title,
                        message = notifyDto.Message,
                        entity_type = notifyDto.EntityType,
                        entity_id = notifyDto.EntityId,
                        created_date = DateTime.Now,
                        is_read = 0
                    };

                    db.notifications.Add(notification);
                    createdIds.Add(notification.id);
                }

                db.SaveChanges();

                return Ok(new { count = createdIds.Count, message = $"Уведомления отправлены {createdIds.Count} операторам" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/deviations/unresolved
        [HttpGet]
        [Route("unresolved")]
        public IHttpActionResult GetUnresolvedDeviations()
        {
            var unresolved = db.batch_steps
                .Where(s => s.has_deviation == 1 && s.end_time == null)
                .Select(s => new
                {
                    s.id,
                    s.batch_id,
                    s.step_number,
                    s.step_name,
                    s.deviation_description
                })
                .ToList();

            var result = unresolved.Select(s =>
            {
                var batch = db.production_batches.FirstOrDefault(b => b.id == s.batch_id);
                return new
                {
                    s.id,
                    s.batch_id,
                    batch_number = batch?.batch_number,
                    s.step_number,
                    s.step_name,
                    s.deviation_description
                };
            }).ToList();

            return Ok(result);
        }

        // ==================== DTO КЛАССЫ ====================

        public class NotificationDto
        {
            public int UserId { get; set; }
            public string Type { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
            public string EntityType { get; set; }
            public int EntityId { get; set; }
        }

        public class ResolveDto
        {
            public string Resolution { get; set; }
            public string Comment { get; set; }
        }

        public class SendNotificationDto
        {
            public string Type { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
            public string EntityType { get; set; }
            public int EntityId { get; set; }
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