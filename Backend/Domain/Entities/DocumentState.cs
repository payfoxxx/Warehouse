using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public enum DocumentState
    {
        Signed = 1,
        NotSigned = 2,
        Revoked = 3
    }
}
