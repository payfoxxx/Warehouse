using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IShipmentDocumentRepository : IRepository<ShipmentDocument>
    {
        Task<bool> ExistsByNumAsync(string num);
        Task<IEnumerable<ShipmentDocument>> GetAllAsync(ShipmentFilter filters);
        Task<ShipmentDocument?> GetByIdUntrackedAsync(Guid id);
        void SyncDocumentResources(ShipmentDocument document, IEnumerable<ShipmentResource> resources);
    }
}
