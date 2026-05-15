using System;

namespace AppDesktop.Models
{
    public class ProductionBatch
    {
        public int id { get; set; }
        public string batch_number { get; set; }
        public int order_id { get; set; }
        public string status { get; set; }
        public int? current_step_id { get; set; }
        public int? current_step_number { get; set; }
        public string current_step_name { get; set; }
        public int? line_number { get; set; }
        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }
        public int? actual_quantity_kg { get; set; }
        public string product_name { get; set; }
        public int total_steps { get; set; }
        public int completed_steps { get; set; }
    }
}