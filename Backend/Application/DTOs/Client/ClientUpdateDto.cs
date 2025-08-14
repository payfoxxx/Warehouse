using System;

namespace Application.DTOs
{
    public class ClientUpdateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int State { get; set; } 
    }
}