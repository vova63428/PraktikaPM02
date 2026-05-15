using System;
using System.Linq;
using System.Web.Http;
using AgroSintez.Models;

namespace AgroSintez.Controllers
{
    // [Authorize]
    [RoutePrefix("api/production-batches")]
    public class ProductionBatchesController : ApiController
    {
        private AgroSintezEntities db;

        public ProductionBatchesController()
        {
            db = new AgroSintezEntities();
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        // GET: api/production-batches
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            var batches = db.production_batches
                .Select(b => new
                {
                    b.id,
                    b.batch_number,
                    b.order_id,
                    b.start_time,
                    b.end_time,
                    b.status,
                    b.actual_quantity_kg,
                    b.operator_id,
                    b.created_date
                })
                .OrderByDescending(b => b.created_date)
                .ToList();

            return Ok(batches);
        }

        // GET: api/production-batches/active
        [HttpGet]
        [Route("active")]
        public IHttpActionResult GetActive()
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

        // GET: api/production-batches/order/{orderId}
        [HttpGet]
        [Route("order/{orderId}")]
        public IHttpActionResult GetByOrder(int orderId)
        {
            var batches = db.production_batches
                .Where(b => b.order_id == orderId)
                .Select(b => new
                {
                    b.id,
                    b.batch_number,
                    b.start_time,
                    b.end_time,
                    b.status,
                    b.actual_quantity_kg,
                    b.operator_id,
                    b.created_date
                })
                .OrderBy(b => b.created_date)
                .ToList();

            return Ok(batches);
        }

        // GET: api/production-batches/{id}
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var batch = db.production_batches
                .Where(b => b.id == id)
                .Select(b => new
                {
                    b.id,
                    b.batch_number,
                    b.order_id,
                    b.start_time,
                    b.end_time,
                    b.status,
                    b.actual_quantity_kg,
                    b.operator_id,
                    b.created_date
                })
                .FirstOrDefault();

            if (batch == null)
                return NotFound();

            return Ok(batch);
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