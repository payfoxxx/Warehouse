using System.Text.Json.Serialization;

namespace Application.DTOs {
    public class ShipmentFilterDto 
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string[]? NumId { get; set; }
        public string[]? ClientId { get; set; }
        public Guid[]? ResourceId { get; set; }
        public string[]? MeasureUnitId { get; set; }
    }
}