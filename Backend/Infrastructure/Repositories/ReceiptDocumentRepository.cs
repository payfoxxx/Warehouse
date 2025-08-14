using Domain.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ReceiptDocumentRepository : IReceiptDocumentRepository
    {
        private readonly AppDbContext _context;
        public ReceiptDocumentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ReceiptDocument entity)
        {
            await _context.ReceiptDocuments.AddAsync(entity);
        }

        public void Delete(ReceiptDocument entity)
        {
            foreach (var resource in entity.ReceiptResources)
                _context.ReceiptResources.Remove(resource);
            _context.ReceiptDocuments.Remove(entity);
        }

        public async Task<bool> ExistsByNumAsync(string num)
        {
            return await _context.ReceiptDocuments.AnyAsync(rd => rd.Num == num);
        }

        public async Task<IEnumerable<ReceiptDocument>> GetAllAsync()
        {
            return await _context.ReceiptDocuments
                .Include(rd => rd.ReceiptResources)
                    .ThenInclude(rr => rr.Resource)
                .Include(rd => rd.ReceiptResources)    
                    .ThenInclude(rr => rr.MeasureUnit)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReceiptDocument>> GetAllAsync(ReceiptFilter filters) 
        {
            var query = _context.ReceiptDocuments
                .Include(rd => rd.ReceiptResources)
                    .ThenInclude(rr => rr.Resource)
                .Include(rd => rd.ReceiptResources)    
                    .ThenInclude(rr => rr.MeasureUnit)
                .AsQueryable();
            query = ApplyFilters(query, filters);
            
            return await query.ToListAsync();
        }

        private IQueryable<ReceiptDocument> ApplyFilters(
            IQueryable<ReceiptDocument> query,
            ReceiptFilter filters
        ) 
        {
            if (filters.DateFrom.HasValue)
                query = query.Where(rd => rd.Date >= filters.DateFrom.Value);
            
            if (filters.DateTo.HasValue)
                query = query.Where(rd => rd.Date <= filters.DateTo.Value.ToUniversalTime());

            if (filters.NumId?.Length != 0)
                query = query.Where(rd => filters.NumId!.Contains(rd.Id));
            
            if (filters.ResourceId?.Length != 0)
                query = query.Where(rd => 
                    rd.ReceiptResources.Any(rr => 
                        filters.ResourceId!.Contains(rr.ResourceId)));

            if (filters.MeasureUnitId?.Length != 0)
                query = query.Where(rd => 
                    rd.ReceiptResources.Any(rr => 
                        filters.MeasureUnitId!.Contains(rr.MeasureUnitId)));

            return query;
        }

        public async Task<ReceiptDocument?> GetByIdAsync(Guid id)
        {
            return await _context.ReceiptDocuments
                .Include(rd => rd.ReceiptResources)
                    .ThenInclude(rr => rr.Resource)
                .Include(rd => rd.ReceiptResources)    
                    .ThenInclude(rr => rr.MeasureUnit)
                .FirstOrDefaultAsync(rd => rd.Id == id);
        }

        public async Task<ReceiptDocument?> GetByIdUntrackedAsync(Guid id)
        {
            return await _context.ReceiptDocuments
                .AsNoTracking()
                .Include(d => d.ReceiptResources)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public void Update(ReceiptDocument entity)
        {
            var existingDocument = _context.ReceiptDocuments
            .Include(d => d.ReceiptResources)
            .FirstOrDefault(d => d.Id == entity.Id);

            if (existingDocument == null) return;

            _context.Entry(existingDocument).CurrentValues.SetValues(entity);

            SyncResources(existingDocument, entity);

            _context.Entry(existingDocument).State = EntityState.Modified;
        }

        private void SyncResources(ReceiptDocument existingDocument, ReceiptDocument newDocument)
        {
            foreach (var existingResource in existingDocument.ReceiptResources.ToList())
            {
                if (!newDocument.ReceiptResources.Any(r => r.Id == existingResource.Id))
                {
                    _context.ReceiptResources.Remove(existingResource);
                }
            }

            foreach (var newResource in newDocument.ReceiptResources)
            {
                var existingResource = existingDocument.ReceiptResources
                    .FirstOrDefault(r => r.Id == newResource.Id);

                if (existingResource != null)
                {
                    _context.Entry(existingResource).CurrentValues.SetValues(newResource);
                }
                else
                {
                    existingDocument.ReceiptResources.Add(newResource);
                }
            }
        }

    }
}
