using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IReceiptDocumentRepository : IRepository<ReceiptDocument>
    {
        Task<bool> ExistsByNumAsync(string num);
        Task<IEnumerable<ReceiptDocument>> GetAllAsync(ReceiptFilter filters);
        Task<ReceiptDocument?> GetByIdUntrackedAsync(Guid id);
    }
}
