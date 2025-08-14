using System;

namespace Application.DTOs
{
    public class ClientDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string StateName { get; set; }
        public int State { get; set; }
    }
}