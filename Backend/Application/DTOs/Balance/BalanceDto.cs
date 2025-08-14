using System;

namespace Application.DTOs
{
    public class BalanceDto
    {
        public Guid Id { get; set; }
        public string ResourceName { get; set; }
        public Guid ResourceId { get; set; }
        public string MeasureUnitName { get; set; }
        public Guid MeasureUnitId { get; set; }
        public int Count { get; set; }
    }
}