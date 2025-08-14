using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IBalanceRepository Balances { get; }
        IClientRepository Clients { get; }
        IMeasureUnitRepository MeasureUnits { get; }
        IReceiptDocumentRepository ReceiptDocuments { get; }
        IResourceRepository Resources { get; }
        IShipmentDocumentRepository ShipmentDocuments { get; }
        IResourceUsageRepository ResourceUsage { get; }
        Task<bool> SaveEntitiesAsync();
        Task<ITransaction> BeginTransactionAsync();
        Task<int> CommitAsync();
    }
}
