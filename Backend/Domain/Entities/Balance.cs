using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Balance
    {
        public Guid Id { get; set; }
        public int Count { get; set; }
        public Resource Resource { get; set; } = null!;
        public Guid ResourceId { get; set; }
        public MeasureUnit MeasureUnit { get; set; } = null!;
        public Guid MeasureUnitId { get; set; }
    }
}
