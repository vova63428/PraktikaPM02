using System;
using System.Collections.Generic;

namespace LabDesktop.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }

    public class BatchForQC
    {
        public int id { get; set; }
        public string batch_number { get; set; }
        public int order_id { get; set; }
        public int actual_quantity_kg { get; set; }
        public DateTime? end_time { get; set; }
        public DateTime? created_date { get; set; }
    }

    // Партия сырья для контроля
    public class RawMaterialBatchForQC
    {
        public int id { get; set; }
        public string batch_number { get; set; }
        public int raw_material_id { get; set; }
        public string raw_material_name { get; set; }
        public decimal quantity { get; set; }
        public string unit_of_measure { get; set; }
        public DateTime? receipt_date { get; set; }
        public string supplier { get; set; }
        public string lab_status { get; set; } // pending, in_progress, approved, rejected
    }

    public class LabTest
    {
        public int id { get; set; }
        public int? batch_id { get; set; }
        public int? raw_material_batch_id { get; set; }
        public int lab_technician_id { get; set; }
        public string lab_technician_name { get; set; } // ДОБАВИТЬ ЭТУ СТРОКУ
        public DateTime? analysis_date { get; set; }
        public string sample_type { get; set; }
        public string parameter_name { get; set; }
        public decimal? measured_value { get; set; }
        public string standard_value { get; set; }
        public string unit_of_measure { get; set; }
        public string result { get; set; }
        public string decision { get; set; }
        public string lab_comment { get; set; }
        public string batch_number { get; set; }
    }
    public class RegisterModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; } // "lab technician" или "admin"
    }

    // Параметр контроля (нормативы)
    public class QualityParameter
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public string parameter_name { get; set; }
        public string standard_value { get; set; }
        public string unit_of_measure { get; set; }
        public bool is_mandatory { get; set; }
        public int order_number { get; set; }
    }

    // Протокол испытаний
    public class TestProtocol
    {
        public int id { get; set; }
        public int batch_id { get; set; }
        public string batch_number { get; set; }
        public string protocol_number { get; set; }
        public DateTime protocol_date { get; set; }
        public string conclusion { get; set; }
        public string signed_by { get; set; }
        public List<LabTest> tests { get; set; }
    }

    // Статистика для дашборда
    public class LabStatistics
    {
        public int pendingBatchesCount { get; set; }
        public int pendingRawMaterialsCount { get; set; }
        public int inProgressCount { get; set; }
        public int completedTodayCount { get; set; }
        public int rejectedThisWeekCount { get; set; }
    }

    // НОВЫЙ КЛАСС - производственные партии для истории
    public class ProductionBatch
    {
        public int id { get; set; }
        public string batch_number { get; set; }
        public int order_id { get; set; }
        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }
        public string status { get; set; }
        public int? actual_quantity_kg { get; set; }
        public int? operator_id { get; set; }
        public DateTime? created_date { get; set; }
    }

}