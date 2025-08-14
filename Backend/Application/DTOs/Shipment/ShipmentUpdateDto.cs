using System;

namespace Application.DTOs
{
    public class ShipmentDocumentUpdateDto
    {
        public Guid Id { get; set; }
        public string Num { get; set; }
        public Guid ClientId { get; set; }
        public DateTime Date { get; set; }
        // public string StatusName { get; set; }
        public int StatusId { get; set; }
        public List<ShipmentResourceUpdateDto> ShipmentResources { get; set; }
    }

    public class ShipmentResourceUpdateDto
    {
        public Guid Id { get; set; }
        public Guid ResourceId { get; set; }
        public Guid MeasureUnitId { get; set; }
        public int Count { get; set; } 
    }
}