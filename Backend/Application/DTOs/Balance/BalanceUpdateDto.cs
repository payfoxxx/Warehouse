using System;

namespace Application.DTOs
{
    public class BalanceUpdateDto
    {
        public Guid Id { get; set; }
        public Guid ResourceId { get; set; }
        public Guid MeasureUnitId { get; set; }
        public int Count { get; set; }
    }
}