using System;

namespace AppDesktop.Models
{
    public class EquipmentTelemetry
    {
        public int equipment_id { get; set; }
        public string equipment_name { get; set; }
        public decimal temperature { get; set; }
        public decimal pressure { get; set; }
        public decimal speed { get; set; }
        public decimal vibration { get; set; }
        public decimal amperage { get; set; }
        public bool is_running { get; set; }
        public DateTime timestamp { get; set; }
    }
}