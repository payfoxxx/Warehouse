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
    public class ResourceUsageRepository : IResourceUsageRepository
    {
        private readonly AppDbContext _context;
        public ResourceUsageRepository(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<bool> IsResourceUsedAsync(Guid resourceId, Guid measureUnitId)
        {
            bool inReceipts = await _context.ReceiptResources
                .AnyAsync(r => 
                    r.ResourceId == resourceId &&
                    r.MeasureUnitId == measureUnitId);
            
            bool inShipments = await _context.ShipmentResources
                .AnyAsync(r => 
                    r.ResourceId == resourceId &&
                    r.MeasureUnitId == measureUnitId);
            
            return inReceipts || inShipments;
        }
    }
}
