using System;

namespace Application.DTOs
{
    public class MeasureUnitUpdateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int State { get; set; }
    }
}