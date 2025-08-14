using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IClientRepository : IRepository<Client>
    {
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<Client>> GetAllByStateAsync(State state);
        Task<bool> ExistsByNameAndIdAsync(string name, Guid id);
        Task<bool> IsClientUsedAsync(Guid id);
    }
}
