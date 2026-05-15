using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AgroSintez.Models;

namespace AgroSintez.Controllers
{
    
    [RoutePrefix("api/reference")]
    public class ReferenceController : ApiController
    {
        private AgroSintezEntities db;

        public ReferenceController()
        {
            db = new AgroSintezEntities();
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        // ==================== ПРОДУКЦИЯ ====================

        // GET: api/reference/products
        [HttpGet]
        [Route("products")]
        public IHttpActionResult GetProducts()
        {
            var products = db.products
                .Where(p => p.status == "active")
                .Select(p => new
                {
                    p.id,
                    p.code,
                    p.name,
                    p.product_type,
                    p.release_form,
                    p.status,
                    p.created_date
                })
                .OrderBy(p => p.name)
                .ToList();

            return Ok(products);
        }

        // GET: api/reference/products/{id}
        [HttpGet]
        [Route("products/{id}")]
        public IHttpActionResult GetProduct(int id)
        {
            var product = db.products
                .Where(p => p.id == id)
                .Select(p => new
                {
                    p.id,
                    p.code,
                    p.name,
                    p.product_type,
                    p.release_form,
                    p.status,
                    p.created_date
                })
                .FirstOrDefault();

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // ==================== СЫРЬЕ ====================

        // GET: api/reference/raw-materials
        [HttpGet]
        [Route("raw-materials")]
        public IHttpActionResult GetRawMaterials()
        {
            var materials = db.raw_materials
                .Select(r => new
                {
                    r.id,
                    r.code,
                    r.name,
                    r.category,
                    r.unit_of_measure,
                    r.hazard_class,
                    r.created_date
                })
                .OrderBy(r => r.name)
                .ToList();

            return Ok(materials);
        }

        // GET: api/reference/raw-materials/{id}
        [HttpGet]
        [Route("raw-materials/{id}")]
        public IHttpActionResult GetRawMaterial(int id)
        {
            var material = db.raw_materials
                .Where(r => r.id == id)
                .Select(r => new
                {
                    r.id,
                    r.code,
                    r.name,
                    r.category,
                    r.unit_of_measure,
                    r.hazard_class,
                    r.created_date
                })
                .FirstOrDefault();

            if (material == null)
                return NotFound();

            return Ok(material);
        }

        // GET: api/reference/raw-materials/categories
        [HttpGet]
        [Route("raw-materials/categories")]
        public IHttpActionResult GetRawMaterialCategories()
        {
            var categories = db.raw_materials
                .Where(r => r.category != null)
                .Select(r => r.category)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            return Ok(categories);
        }

        // ==================== ОБОРУДОВАНИЕ ====================

        // GET: api/reference/equipment
        [HttpGet]
        [Route("equipment")]
        public IHttpActionResult GetEquipment()
        {
            var equipment = db.equipment
                .Select(e => new
                {
                    e.id,
                    e.code,
                    e.name,
                    e.equipment_type,
                    e.line_number,
                    e.status,
                    e.last_calibration_date,
                    e.created_date
                })
                .OrderBy(e => e.name)
                .ToList();

            return Ok(equipment);
        }

        // GET: api/reference/equipment/active
        [HttpGet]
        [Route("equipment/active")]
        public IHttpActionResult GetActiveEquipment()
        {
            var equipment = db.equipment
                .Where(e => e.status == "operational")
                .Select(e => new
                {
                    e.id,
                    e.code,
                    e.name,
                    e.equipment_type,
                    e.line_number
                })
                .OrderBy(e => e.name)
                .ToList();

            return Ok(equipment);
        }

        // GET: api/reference/equipment/{id}
        [HttpGet]
        [Route("equipment/{id}")]
        public IHttpActionResult GetEquipment(int id)
        {
            var equipment = db.equipment
                .Where(e => e.id == id)
                .Select(e => new
                {
                    e.id,
                    e.code,
                    e.name,
                    e.equipment_type,
                    e.line_number,
                    e.status,
                    e.last_calibration_date,
                    e.created_date
                })
                .FirstOrDefault();

            if (equipment == null)
                return NotFound();

            return Ok(equipment);
        }

        // GET: api/reference/equipment/types
        [HttpGet]
        [Route("equipment/types")]
        public IHttpActionResult GetEquipmentTypes()
        {
            var types = db.equipment
                .Where(e => e.equipment_type != null)
                .Select(e => e.equipment_type)
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            return Ok(types);
        }

        // ==================== РЕЦЕПТУРЫ ====================

        // GET: api/reference/recipes
        [HttpGet]
        [Route("recipes")]
        public IHttpActionResult GetRecipes()
        {
            var recipes = db.recipes
                .Select(r => new
                {
                    r.id,
                    r.name,
                    r.version,
                    r.status,
                    product_id = r.product_id,
                    r.creation_date,
                    created_by_id = r.created_by_id
                })
                .OrderBy(r => r.name)
                .ToList();

            return Ok(recipes);
        }

        // GET: api/reference/recipes/active
        [HttpGet]
        [Route("recipes/active")]
        public IHttpActionResult GetActiveRecipes()
        {
            var recipes = db.recipes
                .Where(r => r.status == "active")
                .Select(r => new
                {
                    r.id,
                    r.name,
                    r.version,
                    product_id = r.product_id
                })
                .OrderBy(r => r.name)
                .ToList();

            return Ok(recipes);
        }

        // GET: api/reference/recipes/{id}/components
        [HttpGet]
        [Route("recipes/{id}/components")]
        public IHttpActionResult GetRecipeComponents(int id)
        {
            var components = db.formula_components
                .Where(c => c.recipe_id == id)
                .Select(c => new
                {
                    c.id,
                    c.percentage,
                    c.loading_order,
                    c.deviation_tolerance,
                    raw_material_id = c.raw_material_id,
                    c.created_date
                })
                .OrderBy(c => c.loading_order)
                .ToList();

            // Добавляем информацию о сырье отдельным запросом
            var result = components.Select(c => new
            {
                c.id,
                c.percentage,
                c.loading_order,
                c.deviation_tolerance,
                c.raw_material_id,
                c.created_date,
                raw_material = db.raw_materials.Where(r => r.id == c.raw_material_id)
                    .Select(r => new { r.code, r.name, r.unit_of_measure })
                    .FirstOrDefault()
            }).ToList();

            return Ok(result);
        }

        // ==================== ТЕХНОЛОГИЧЕСКИЕ КАРТЫ ====================

        // GET: api/reference/technological-cards
        [HttpGet]
        [Route("technological-cards")]
        public IHttpActionResult GetTechnologicalCards()
        {
            var cards = db.technological_cards
                .Select(t => new
                {
                    t.id,
                    product_id = t.product_id,
                    t.version,
                    t.status,
                    t.approval_date,
                    approved_by_id = t.approved_by_id,
                    t.created_date
                })
                .OrderByDescending(t => t.created_date)
                .ToList();

            return Ok(cards);
        }

        // GET: api/reference/technological-cards/approved
        [HttpGet]
        [Route("technological-cards/approved")]
        public IHttpActionResult GetApprovedTechnologicalCards()
        {
            var cards = db.technological_cards
                .Where(t => t.status == "approved")
                .Select(t => new
                {
                    t.id,
                    product_id = t.product_id,
                    t.version
                })
                .ToList();

            return Ok(cards);
        }

        // GET: api/reference/technological-cards/{id}/steps
        [HttpGet]
        [Route("technological-cards/{id}/steps")]
        public IHttpActionResult GetTechCardSteps(int id)
        {
            var steps = db.tech_card_steps
                .Where(s => s.tech_card_id == id)
                .Select(s => new
                {
                    s.id,
                    s.step_number,
                    s.step_name,
                    s.step_type,
                    s.planned_temp_min,
                    s.planned_temp_max,
                    s.planned_duration_min,
                    s.planned_pressure_min,
                    s.planned_pressure_max,
                    s.is_mandatory,
                    s.instruction,
                    equipment_id = s.equipment_id,
                    equipment_name = s.equipment != null ? s.equipment.name : null
                })
                .OrderBy(s => s.step_number)
                .ToList();

            return Ok(steps);
        }

        // ==================== ПОЛЬЗОВАТЕЛИ ====================

        // GET: api/reference/users
        [HttpGet]
        [Route("users")]
        public IHttpActionResult GetUsers()
        {
            var users = db.users
                .Where(u => u.is_active == 1)
                .Select(u => new
                {
                    u.id,
                    u.login,
                    u.full_name,
                    u.role,
                    u.department
                })
                .OrderBy(u => u.full_name)
                .ToList();

            return Ok(users);
        }

        // GET: api/reference/users/roles
        [HttpGet]
        [Route("users/roles")]
        public IHttpActionResult GetUserRoles()
        {
            var roles = db.users
                .Where(u => u.role != null)
                .Select(u => u.role)
                .Distinct()
                .OrderBy(r => r)
                .ToList();

            return Ok(roles);
        }

        // GET: api/reference/users/technologists
        [HttpGet]
        [Route("users/technologists")]
        public IHttpActionResult GetTechnologists()
        {
            var technologists = db.users
                .Where(u => u.role == "technologist" && u.is_active == 1)
                .Select(u => new
                {
                    u.id,
                    u.login,
                    u.full_name,
                    u.department
                })
                .OrderBy(u => u.full_name)
                .ToList();

            return Ok(technologists);
        }

        // GET: api/reference/users/operators
        [HttpGet]
        [Route("users/operators")]
        public IHttpActionResult GetOperators()
        {
            var operators = db.users
                .Where(u => u.role == "operator" && u.is_active == 1)
                .Select(u => new
                {
                    u.id,
                    u.login,
                    u.full_name,
                    u.department
                })
                .OrderBy(u => u.full_name)
                .ToList();

            return Ok(operators);
        }

        // ==================== ПРОИЗВОДСТВЕННЫЕ ЗАКАЗЫ ====================

        // GET: api/reference/production-orders
        [HttpGet]
        [Route("production-orders")]
        public IHttpActionResult GetProductionOrders()
        {
            var orders = db.production_orders
                .Select(o => new
                {
                    o.id,
                    o.order_number,
                    o.planned_quantity_kg,
                    o.status,
                    o.planned_start_date,
                    recipe_id = o.recipe_id,
                    tech_card_id = o.tech_card_id,
                    created_by_id = o.created_by_id,
                    o.created_date
                })
                .OrderByDescending(o => o.created_date)
                .ToList();

            return Ok(orders);
        }

        // ==================== ПРОИЗВОДСТВЕННЫЕ ПАРТИИ ====================

        // GET: api/reference/production-batches
        [HttpGet]
        [Route("production-batches")]
        public IHttpActionResult GetProductionBatches()
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

        // ==================== СТАТУСЫ ====================

        // GET: api/reference/order-statuses
        [HttpGet]
        [Route("order-statuses")]
        public IHttpActionResult GetOrderStatuses()
        {
            var statuses = new[]
            {
                new { value = "planned", name = "Запланирован" },
                new { value = "in_progress", name = "В работе" },
                new { value = "completed", name = "Выполнен" },
                new { value = "cancelled", name = "Отменён" },
                new { value = "draft", name = "Черновик" }
            };

            return Ok(statuses);
        }

        // GET: api/reference/batch-statuses
        [HttpGet]
        [Route("batch-statuses")]
        public IHttpActionResult GetBatchStatuses()
        {
            var statuses = new[]
            {
                new { value = "planned", name = "Запланирована" },
                new { value = "in_progress", name = "В работе" },
                new { value = "completed", name = "Выполнена" },
                new { value = "rejected", name = "Забракована" }
            };

            return Ok(statuses);
        }

        // GET: api/reference/recipe-statuses
        [HttpGet]
        [Route("recipe-statuses")]
        public IHttpActionResult GetRecipeStatuses()
        {
            var statuses = new[]
            {
                new { value = "draft", name = "Черновик" },
                new { value = "active", name = "Активна" },
                new { value = "archived", name = "Архивирована" }
            };

            return Ok(statuses);
        }

        // ==================== СТАТИСТИКА ====================

        // GET: api/reference/statistics
        [HttpGet]
        [Route("statistics")]
        public IHttpActionResult GetStatistics()
        {
            var stats = new
            {
                productsCount = db.products.Count(),
                rawMaterialsCount = db.raw_materials.Count(),
                equipmentCount = db.equipment.Count(),
                activeEquipmentCount = db.equipment.Count(e => e.status == "operational"),
                usersCount = db.users.Count(u => u.is_active == 1),
                recipesCount = db.recipes.Count(r => r.status == "active"),
                productionOrdersCount = db.production_orders.Count(),
                batchesCount = db.production_batches.Count(),
                completedBatchesCount = db.production_batches.Count(b => b.status == "completed"),
                inProgressBatchesCount = db.production_batches.Count(b => b.status == "in_progress"),
                notificationsCount = db.notifications.Count(n => n.is_read == 0)
            };

            return Ok(stats);
        }

        // ==================== ПОИСК ====================

        // GET: api/reference/search?q=text
        [HttpGet]
        [Route("search")]
        public IHttpActionResult Search(string q)
        {
            if (string.IsNullOrEmpty(q) || q.Length < 2)
                return BadRequest("Search query must be at least 2 characters");

            var result = new
            {
                products = db.products
                    .Where(p => p.name.Contains(q) || p.code.Contains(q))
                    .Select(p => new { type = "product", p.id, p.code, p.name })
                    .Take(5)
                    .ToList(),

                rawMaterials = db.raw_materials
                    .Where(r => r.name.Contains(q) || r.code.Contains(q))
                    .Select(r => new { type = "raw_material", r.id, r.code, r.name })
                    .Take(5)
                    .ToList(),

                equipment = db.equipment
                    .Where(e => e.name.Contains(q) || e.code.Contains(q))
                    .Select(e => new { type = "equipment", e.id, e.code, e.name })
                    .Take(5)
                    .ToList(),

                recipes = db.recipes
                    .Where(r => r.name.Contains(q))
                    .Select(r => new { type = "recipe", r.id, name = r.name })
                    .Take(5)
                    .ToList()
            };

            return Ok(result);
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