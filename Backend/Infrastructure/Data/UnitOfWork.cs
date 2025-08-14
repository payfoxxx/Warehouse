using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private bool _disposed;
        private IDbContextTransaction? _transaction;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }
        public IBalanceRepository Balances => new BalanceRepository(_context);

        public IClientRepository Clients => new ClientRepository(_context);

        public IMeasureUnitRepository MeasureUnits => new MeasureUnitRepository(_context);

        public IReceiptDocumentRepository ReceiptDocuments => new ReceiptDocumentRepository(_context);

        public IResourceRepository Resources => new ResourceRepository(_context);

        public IShipmentDocumentRepository ShipmentDocuments => new ShipmentDocumentRepository(_context);
        public IResourceUsageRepository ResourceUsage => new ResourceUsageRepository(_context);

        public async Task<bool> SaveEntitiesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ITransaction> BeginTransactionAsync()
        {
            return new EfTransaction(await _context.Database.BeginTransactionAsync());
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose() 
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}
