using System;

namespace Application.DTOs
{
    public class ResourceUpdateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int State { get; set; }
    }
}