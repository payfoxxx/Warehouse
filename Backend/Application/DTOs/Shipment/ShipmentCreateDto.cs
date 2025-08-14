using System;

namespace Application.DTOs
{
    public class ShipmentDocumentCreateDto
    {
        public string Num { get; set; }
        public Guid ClientId { get; set; }
        public DateTime Date { get; set; }
        public List<ShipmentResourceCreateDto> ShipmentResources { get; set; } = new List<ShipmentResourceCreateDto>();
    }

    public class ShipmentResourceCreateDto
    {
        public Guid ResourceId { get; set; }
        public Guid MeasureUnitId { get; set; }
        public int Count { get; set; }
    }
}