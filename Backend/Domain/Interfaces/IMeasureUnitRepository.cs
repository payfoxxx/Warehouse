using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IMeasureUnitRepository : IRepository<MeasureUnit>
    {
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<MeasureUnit>> GetAllByStateAsync(State state);
        Task<bool> ExistsByNameAndIdAsync(string name, Guid id);
        Task<bool> IsMeasureUnitUsedAsync(Guid id);
        Task<IEnumerable<MeasureUnit>> GetByIdsAsync(IEnumerable<Guid> ids);
    }
}
