namespace Domain.Entities {
    public class BalanceFilter 
    {
        public Guid[]? ResourceId { get;  set; }
        public Guid[]? MeasureUnitId { get; set; }
    }
}