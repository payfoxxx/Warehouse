using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IBalanceRepository : IRepository<Balance>
    {
        Task<IEnumerable<Balance>> GetAllAsync(BalanceFilter filters);
        Task<Balance?> GetByResourceAndMeasureUnitAsync(Guid resourceId, Guid measureUnitId);
    }
}
