namespace Domain.Entities {
    public class ShipmentFilter 
    {
        public DateTime? DateFrom { get;  set; }
        public DateTime? DateTo { get;  set; }
        public Guid[]? NumId { get;  set; }
        public Guid[]? ResourceId { get;  set; }
        public Guid[]? MeasureUnitId { get; set; }
    }
}