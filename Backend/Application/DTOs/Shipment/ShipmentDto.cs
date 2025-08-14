using System;

namespace Application.DTOs
{
    public class ShipmentDocumentDto
    {
        public Guid Id { get; set; }
        public string Num { get; set; }
        public string ClientName { get; set; }
        public Guid ClientId { get; set; }
        public DateTime Date { get; set; }
        public string StatusName { get; set; }
        public int StatusId { get; set; }
        public List<ShipmentResourceDto> ShipmentResources { get; set; } = new List<ShipmentResourceDto>();
        
    }

    public class ShipmentResourceDto
    {
        public Guid Id { get; set; }
        public string ResourceName { get; set; }
        public Guid ResourceId { get; set; }
        public string MeasureUnitName { get; set; }
        public Guid MeasureUnitId { get; set; }
        public int Count { get; set; }
    }
}