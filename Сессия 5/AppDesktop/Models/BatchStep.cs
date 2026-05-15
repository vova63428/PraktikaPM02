using System;

namespace AppDesktop.Models
{
    public class BatchStep
    {
        public int id { get; set; }
        public int step_number { get; set; }
        public string step_name { get; set; }

        // Поля из API
        public decimal? planned_temperature { get; set; }
        public decimal? planned_pressure { get; set; }
        public int? planned_duration_min { get; set; }

        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }
        public string status { get; set; }

        // Для совместимости с существующим кодом (демо-режим)
        public decimal expected_temperature { get; set; }
        public decimal expected_pressure { get; set; }
        public decimal expected_speed { get; set; }
        public string description { get; set; }
        public string instruction { get; set; }
        public string equipment_type { get; set; }

        // Фактические параметры
        public decimal actual_temperature { get; set; }
        public decimal actual_pressure { get; set; }
        public decimal actual_speed { get; set; }
        public string operator_comment { get; set; }
        public bool has_deviation { get; set; }
        public string deviation_description { get; set; }

        // Planned min/max
        public decimal planned_temp_min { get; set; }
        public decimal planned_temp_max { get; set; }
        public decimal planned_pressure_min { get; set; }
        public decimal planned_pressure_max { get; set; }
    }
}