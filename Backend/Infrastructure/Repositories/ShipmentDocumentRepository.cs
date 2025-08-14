using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ShipmentDocumentRepository : IShipmentDocumentRepository
    {
        private readonly AppDbContext _context;
        public ShipmentDocumentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ShipmentDocument entity)
        {
            await _context.ShipmentDocuments.AddAsync(entity);
        }

        public void Delete(ShipmentDocument entity)
        {
            foreach (var resource in entity.ShipmentResources)
                _context.ShipmentResources.Remove(resource);
            _context.ShipmentDocuments.Remove(entity);
        }

        public async Task<bool> ExistsByNumAsync(string num)
        {
            return await _context.ShipmentDocuments.AnyAsync(sd => sd.Num == num);
        }

        public async Task<IEnumerable<ShipmentDocument>> GetAllAsync()
        {
            return await _context.ShipmentDocuments.ToListAsync();
        }

        public async Task<IEnumerable<ShipmentDocument>> GetAllAsync(ShipmentFilter filters) 
        {
            var query = _context.ShipmentDocuments
                .Include(sd => sd.Client)
                .Include(rd => rd.ShipmentResources)
                    .ThenInclude(rr => rr.Resource)
                .Include(rd => rd.ShipmentResources)    
                    .ThenInclude(rr => rr.MeasureUnit)
                .AsQueryable();
            query = ApplyFilters(query, filters);
            
            return await query.ToListAsync();
        }

        private IQueryable<ShipmentDocument> ApplyFilters(
            IQueryable<ShipmentDocument> query,
            ShipmentFilter filters
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
                    rd.ShipmentResources.Any(rr => 
                        filters.ResourceId!.Contains(rr.ResourceId)));

            if (filters.MeasureUnitId?.Length != 0)
                query = query.Where(rd => 
                    rd.ShipmentResources.Any(rr => 
                        filters.MeasureUnitId!.Contains(rr.MeasureUnitId)));

            return query;
        }

        public async Task<ShipmentDocument?> GetByIdAsync(Guid id)
        {
            return await _context.ShipmentDocuments
                .Include(sd => sd.ShipmentResources)
                    .ThenInclude(sr => sr.Resource)
                .Include(sd => sd.ShipmentResources)
                    .ThenInclude(sr => sr.MeasureUnit)
                .FirstOrDefaultAsync(sd => sd.Id == id);
        }

        public async Task<ShipmentDocument?> GetByIdUntrackedAsync(Guid id)
        {
            return await _context.ShipmentDocuments
                .AsNoTracking()
                .Include(d => d.ShipmentResources)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public void Update(ShipmentDocument entity)
        {
           var existingDocument = _context.ShipmentDocuments
                .Include(d => d.ShipmentResources)
                .FirstOrDefault(d => d.Id == entity.Id);

            if (existingDocument == null) return;

            _context.Entry(existingDocument).CurrentValues.SetValues(entity);

            SyncResources(existingDocument, entity);

            _context.Entry(existingDocument).State = EntityState.Modified;
        }

        public void SyncDocumentResources(ShipmentDocument document, IEnumerable<ShipmentResource> newResources)
        {
            var newResourcesDict = newResources.ToDictionary(r => r.Id);
            var resourcesToRemove = new List<ShipmentResource>();

            foreach (var existingResource in document.ShipmentResources.ToList())
            {
                if (newResourcesDict.TryGetValue(existingResource.Id, out var newResource))
                {
                    existingResource.ResourceId = newResource.ResourceId;
                    existingResource.MeasureUnitId = newResource.MeasureUnitId;
                    existingResource.Count = newResource.Count;

                    newResourcesDict.Remove(existingResource.Id);
                }
                else
                {
                    resourcesToRemove.Add(existingResource);
                }
            }

            foreach (var resource in resourcesToRemove)
            {
                document.ShipmentResources.Remove(resource);
                _context.ShipmentResources.Remove(resource);
            }

            foreach (var newResource in newResourcesDict.Values)
            {
                var resourceToAdd = new ShipmentResource
                {
                    Id = newResource.Id == Guid.Empty ? Guid.NewGuid() : newResource.Id,
                    ResourceId = newResource.ResourceId,
                    MeasureUnitId = newResource.MeasureUnitId,
                    Count = newResource.Count,
                };

                document.ShipmentResources.Add(resourceToAdd);
            }
        }

        private void SyncResources(ShipmentDocument existingDocument, ShipmentDocument newDocument)
        {
            foreach (var existingResource in existingDocument.ShipmentResources.ToList())
            {
                if (!newDocument.ShipmentResources.Any(r => r.Id == existingResource.Id))
                {
                    _context.ShipmentResources.Remove(existingResource);
                }
            }
            foreach (var newResource in newDocument.ShipmentResources)
            {
                var existingResource = existingDocument.ShipmentResources
                    .FirstOrDefault(r => r.Id == newResource.Id);

                if (existingResource != null)
                {
                    _context.Entry(existingResource).CurrentValues.SetValues(newResource);
                }
                else
                {
                    existingDocument.ShipmentResources.Add(newResource);
                }
            }
        }
    }
}
