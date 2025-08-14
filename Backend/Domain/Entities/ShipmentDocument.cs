using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ShipmentDocument
    {
        public Guid Id { get; set; }
        public string Num { get; set; } = null!;
        public Client Client { get; set; } = null!;
        public Guid ClientId { get; set; }
        public DateTime Date { get; set; }
        public List<ShipmentResource> ShipmentResources { get; set; }
        public DocumentState State { get; set; }
    }
}
