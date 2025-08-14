using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IResourceRepository : IRepository<Resource>
    {
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<Resource>> GetAllByStateAsync(State state);
        Task<bool> ExistsByNameAndIdAsync(string name, Guid id);
        Task<bool> IsResourceUsedAsync(Guid id);
        Task<IEnumerable<Resource>> GetByIdsAsync(IEnumerable<Guid> ids);
    }
}
