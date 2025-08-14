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
    public class BalanceRepository : IBalanceRepository
    {
        private readonly AppDbContext _context;
        public BalanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Balance entity)
        {
            await _context.AddAsync(entity);
        }

        public void Delete(Balance entity)
        {
            _context.Balances.Remove(entity);
        }

        public async Task<IEnumerable<Balance>> GetAllAsync()
        {
            return await _context.Balances.ToListAsync();
        }

        public async Task<IEnumerable<Balance>> GetAllAsync(BalanceFilter filters)
        {
            var query = _context.Balances
                .Include(b => b.Resource)
                .Include(b => b.MeasureUnit)
                .AsQueryable();
            query = ApplyFilters(query, filters);
            return await query.ToListAsync();
        }

        private IQueryable<Balance> ApplyFilters(
            IQueryable<Balance> query,
            BalanceFilter filters
        ) 
        {
            if (filters.ResourceId?.Length != 0)
                query = query.Where(b => filters.ResourceId!.Contains(b.ResourceId));
            
            if (filters.MeasureUnitId?.Length != 0)
                query = query.Where(b => filters.MeasureUnitId!.Contains(b.MeasureUnitId));

            return query;
        }

        public async Task<Balance?> GetByIdAsync(Guid id)
        {
            Balance? balance = await _context.Balances.FirstOrDefaultAsync(x => x.Id == id);
            return balance ?? null;
        }

        public async Task<Balance?> GetByResourceAndMeasureUnitAsync(Guid resourceId, Guid measureUnitId)
        {
            return await _context.Balances
                .Where(b => b.ResourceId == resourceId && b.MeasureUnitId == measureUnitId)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public void Update(Balance entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
