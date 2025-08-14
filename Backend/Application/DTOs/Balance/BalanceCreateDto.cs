using System;

namespace Application.DTOs
{
    public class BalanceCreateDto
    {
        public Guid ResourceId { get; set; }
        public Guid MeasureUnitId { get; set; }
        public int Count { get; set; }
    }
}