using System.Text.Json.Serialization;

namespace Application.DTOs {
    public class ReceiptFilterDto 
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string[]? NumId { get; set; }
        public Guid[]? ResourceId { get; set; }
        public string[]? MeasureUnitId { get; set; }
    }
}