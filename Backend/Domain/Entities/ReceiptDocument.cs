using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ReceiptDocument
    {
        public Guid Id { get; set; }
        public string Num { get; set; } = null!;
        public DateTime Date { get; set; }
        public List<ReceiptResource>? ReceiptResources { get; set; }
    }
}
