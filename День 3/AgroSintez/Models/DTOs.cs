using System;
using System.Collections.Generic;

namespace AgroSintez.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }

    public class Product
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string product_type { get; set; }
        public string release_form { get; set; }
        public string status { get; set; }
        public DateTime? created_date { get; set; }
    }

    public class Recipe
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public string name { get; set; }
        public int version { get; set; }
        public string status { get; set; }
        public DateTime? creation_date { get; set; }
        public int created_by_id { get; set; }
    }

    public class RecipeComponent
    {
        public int Id { get; set; }
        public int RawMaterialId { get; set; }
        public string RawMaterialName { get; set; }
        public string RawMaterialCode { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal Percentage { get; set; }
        public int LoadingOrder { get; set; }
        public decimal? DeviationTolerance { get; set; }
    }

    public class TechCard
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public int version { get; set; }
        public string status { get; set; }
        public DateTime? approval_date { get; set; }
        public DateTime? created_date { get; set; }
    }

    public class TechCardStep
    {
        public int id { get; set; }
        public int tech_card_id { get; set; }
        public int? equipment_id { get; set; }
        public string equipment_name { get; set; }
        public int step_number { get; set; }
        public string step_name { get; set; }
        public string step_type { get; set; }
        public int? planned_temp_min { get; set; }
        public int? planned_temp_max { get; set; }
        public int planned_duration_min { get; set; }
        public decimal? planned_pressure_min { get; set; }
        public decimal? planned_pressure_max { get; set; }
        public int is_mandatory { get; set; }
        public string instruction { get; set; }
    }

    public class ProductionOrder
    {
        public int id { get; set; }
        public string order_number { get; set; }
        public int recipe_id { get; set; }
        public int tech_card_id { get; set; }
        public int planned_quantity_kg { get; set; }
        public string status { get; set; }
        public DateTime? planned_start_date { get; set; }
        public int? created_by_id { get; set; }
        public DateTime? created_date { get; set; }
    }

    public class ProductionBatch
    {
        public int id { get; set; }
        public string batch_number { get; set; }
        public int order_id { get; set; }
        public string order_number { get; set; }
        public int planned_quantity_kg { get; set; }
        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }
        public string status { get; set; }
        public int actual_quantity_kg { get; set; }
        public int? operator_id { get; set; }
        public string operator_name { get; set; }
        public DateTime? created_date { get; set; }
    }

    public class BatchStep
    {
        public int id { get; set; }
        public int batch_id { get; set; }
        public int step_number { get; set; }
        public string step_name { get; set; }
        public int? planned_temperature { get; set; }
        public int? actual_temperature { get; set; }
        public int? planned_duration_min { get; set; }
        public int? actual_duration_min { get; set; }
        public decimal? planned_pressure { get; set; }
        public decimal? actual_pressure { get; set; }
        public int has_deviation { get; set; }
        public string deviation_description { get; set; }
        public string operator_comment { get; set; }
        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }
    }

    public class Deviation
    {
        public int id { get; set; }
        public int batch_id { get; set; }
        public string batch_number { get; set; }
        public int step_number { get; set; }
        public string step_name { get; set; }
        public string deviation_description { get; set; }
        public string operator_comment { get; set; }
        public DateTime? end_time { get; set; }
    }

    public class Equipment
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string equipment_type { get; set; }
        public int? line_number { get; set; }
        public string status { get; set; }
        public DateTime? last_calibration_date { get; set; }
        public DateTime? created_date { get; set; }
    }

    public class RawMaterial
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string category { get; set; }
        public string unit_of_measure { get; set; }
        public int? hazard_class { get; set; }
        public DateTime? created_date { get; set; }
    }

    public class QualityControl
    {
        public int id { get; set; }
        public int? batch_id { get; set; }
        public int? raw_material_batch_id { get; set; }
        public int lab_technician_id { get; set; }
        public DateTime? analysis_date { get; set; }
        public string sample_type { get; set; }
        public string parameter_name { get; set; }
        public decimal? measured_value { get; set; }
        public string standard_value { get; set; }
        public string unit_of_measure { get; set; }
        public string result { get; set; }
        public string decision { get; set; }
        public string lab_comment { get; set; }
    }

    public class Notification
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public string message { get; set; }
        public int is_read { get; set; }
        public string entity_type { get; set; }
        public int entity_id { get; set; }
        public DateTime? created_date { get; set; }
    }

    // Дополнительные классы для отчетов и мониторинга

    public class Statistics
    {
        public int productsCount { get; set; }
        public int rawMaterialsCount { get; set; }
        public int equipmentCount { get; set; }
        public int activeEquipmentCount { get; set; }
        public int usersCount { get; set; }
        public int recipesCount { get; set; }
        public int productionOrdersCount { get; set; }
        public int batchesCount { get; set; }
        public int completedBatchesCount { get; set; }
        public int inProgressBatchesCount { get; set; }
        public int notificationsCount { get; set; }
    }

    public class BatchStepRecord
    {
        public int? ActualTemperature { get; set; }
        public int? ActualDurationMin { get; set; }
        public decimal? ActualPressure { get; set; }
        public string Comment { get; set; }
    }

    public class BatchComplete
    {
        public int ActualQuantityKg { get; set; }
    }

    public class ResolveDeviation
    {
        public string Resolution { get; set; }
        public string Comment { get; set; }
    }

    public class CreateNotification
    {
        public int UserId { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string EntityType { get; set; }
        public int EntityId { get; set; }
    }

    public class ExtruderSettings
    {
        public int Temperature { get; set; }
        public int Pressure { get; set; }
        public int ScrewSpeed { get; set; }
        public int FeedRate { get; set; }
        public int Vacuum { get; set; }
        public string Profile { get; set; }
    }
}