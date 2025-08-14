using System;

namespace Application.DTOs
{
    public class MeasureUnitDto
    {
        public Guid Id { get; set; } 
        public string Name { get; set; }
        public string StateName { get; set; }
        public int State { get; set; }
    }
}