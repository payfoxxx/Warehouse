using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class MeasureUnit
    {
        public Guid Id{ get; set; }
        public string Name { get; set; } = null!;
        public State State{ get; set; }
    }
}
