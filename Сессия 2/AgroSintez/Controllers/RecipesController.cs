using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AgroSintez.Models;

namespace AgroSintez.Controllers
{
    // [Authorize]
    [RoutePrefix("api/recipes")]
    public class RecipesController : ApiController
    {
        private AgroSintezEntities db;

        public RecipesController()
        {
            db = new AgroSintezEntities();
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
        }

        // DTO для создания/обновления рецептуры
        public class RecipeDto
        {
            public int? Id { get; set; }
            public int ProductId { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
            public int? CreatedById { get; set; }
        }

        public class RecipeComponentDto
        {
            public int? Id { get; set; }
            public int RawMaterialId { get; set; }
            public decimal Percentage { get; set; }
            public int LoadingOrder { get; set; }
            public decimal? DeviationTolerance { get; set; }
        }

        // GET: api/recipes
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            var recipes = db.recipes
                .Select(r => new
                {
                    r.id,
                    r.name,
                    r.version,
                    r.status,
                    r.product_id,
                    r.creation_date,
                    r.created_by_id
                })
                .OrderByDescending(r => r.creation_date)
                .ToList();

            return Ok(recipes);
        }

        // GET: api/recipes/active
        [HttpGet]
        [Route("active")]
        public IHttpActionResult GetActive()
        {
            var recipes = db.recipes
                .Where(r => r.status == "active")
                .Select(r => new
                {
                    r.id,
                    r.name,
                    r.version,
                    r.product_id
                })
                .OrderBy(r => r.name)
                .ToList();

            return Ok(recipes);
        }

        // GET: api/recipes/{id}
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var recipe = db.recipes
                .Where(r => r.id == id)
                .Select(r => new
                {
                    r.id,
                    r.name,
                    r.version,
                    r.status,
                    r.product_id,
                    r.creation_date,
                    r.created_by_id
                })
                .FirstOrDefault();

            if (recipe == null)
                return NotFound();

            return Ok(recipe);
        }

        // GET: api/recipes/{id}/components
        [HttpGet]
        [Route("{id}/components")]
        public IHttpActionResult GetComponents(int id)
        {
            var components = db.formula_components
                .Where(c => c.recipe_id == id)
                .Select(c => new
                {
                    c.id,
                    c.raw_material_id,
                    c.percentage,
                    c.loading_order,
                    c.deviation_tolerance,
                    c.created_date
                })
                .OrderBy(c => c.loading_order)
                .ToList();

            // Добавляем информацию о сырье
            var result = components.Select(c =>
            {
                var rawMaterial = db.raw_materials.FirstOrDefault(r => r.id == c.raw_material_id);
                return new
                {
                    c.id,
                    c.raw_material_id,
                    raw_material_code = rawMaterial?.code,
                    raw_material_name = rawMaterial?.name,
                    unit_of_measure = rawMaterial?.unit_of_measure,
                    c.percentage,
                    c.loading_order,
                    c.deviation_tolerance,
                    c.created_date
                };
            }).ToList();

            return Ok(result);
        }

        // POST: api/recipes
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(RecipeDto recipeDto)
        {
            try
            {
                if (recipeDto == null || string.IsNullOrEmpty(recipeDto.Name))
                    return BadRequest("Recipe name is required");

                // Получаем максимальную версию для продукта
                int maxVersion = db.recipes
                    .Where(r => r.product_id == recipeDto.ProductId)
                    .Select(r => r.version)
                    .DefaultIfEmpty(0)
                    .Max();

                var recipe = new recipes
                {
                    product_id = recipeDto.ProductId,
                    name = recipeDto.Name,
                    version = maxVersion + 1,
                    status = recipeDto.Status ?? "draft",
                    creation_date = DateTime.Now,
                    created_by_id = recipeDto.CreatedById ?? 1
                };

                db.recipes.Add(recipe);
                db.SaveChanges();

                return Ok(new { id = recipe.id, message = "Recipe created", version = recipe.version });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT: api/recipes/{id}
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Update(int id, RecipeDto recipeDto)
        {
            try
            {
                var recipe = db.recipes.Find(id);
                if (recipe == null)
                    return NotFound();

                recipe.name = recipeDto.Name;
                recipe.status = recipeDto.Status ?? recipe.status;

                db.SaveChanges();

                return Ok(new { message = "Recipe updated" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/recipes/{id}/components
        [HttpPost]
        [Route("{id}/components")]
        public IHttpActionResult AddComponent(int id, RecipeComponentDto componentDto)
        {
            try
            {
                var recipe = db.recipes.Find(id);
                if (recipe == null)
                    return NotFound();

                // Проверяем сумму процентов
                var existingSum = db.formula_components
                    .Where(c => c.recipe_id == id)
                    .Sum(c => c.percentage);

                if (existingSum + componentDto.Percentage > 100)
                    return BadRequest("Total percentage cannot exceed 100%");

                var component = new formula_components
                {
                    recipe_id = id,
                    raw_material_id = componentDto.RawMaterialId,
                    percentage = componentDto.Percentage,
                    loading_order = componentDto.LoadingOrder,
                    deviation_tolerance = componentDto.DeviationTolerance ?? 0.5m,
                    created_date = DateTime.Now
                };

                db.formula_components.Add(component);
                db.SaveChanges();

                return Ok(new { id = component.id, message = "Component added" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT: api/recipes/components/{componentId}
        [HttpPut]
        [Route("components/{componentId}")]
        public IHttpActionResult UpdateComponent(int componentId, RecipeComponentDto componentDto)
        {
            try
            {
                var component = db.formula_components.Find(componentId);
                if (component == null)
                    return NotFound();

                component.percentage = componentDto.Percentage;
                component.loading_order = componentDto.LoadingOrder;
                component.deviation_tolerance = componentDto.DeviationTolerance ?? 0.5m;

                db.SaveChanges();

                return Ok(new { message = "Component updated" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE: api/recipes/components/{componentId}
        [HttpDelete]
        [Route("components/{componentId}")]
        public IHttpActionResult DeleteComponent(int componentId)
        {
            try
            {
                var component = db.formula_components.Find(componentId);
                if (component == null)
                    return NotFound();

                db.formula_components.Remove(component);
                db.SaveChanges();

                return Ok(new { message = "Component deleted" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/recipes/{id}/approve
        [HttpPost]
        [Route("{id}/approve")]
        public IHttpActionResult Approve(int id)
        {
            try
            {
                var recipe = db.recipes.Find(id);
                if (recipe == null)
                    return NotFound();

                recipe.status = "active";
                db.SaveChanges();

                return Ok(new { message = "Recipe approved" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE: api/recipes/{id}
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var recipe = db.recipes.Find(id);
                if (recipe == null)
                    return NotFound();

                // Удаляем связанные компоненты
                var components = db.formula_components.Where(c => c.recipe_id == id);
                db.formula_components.RemoveRange(components);

                db.recipes.Remove(recipe);
                db.SaveChanges();

                return Ok(new { message = "Recipe deleted" });
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