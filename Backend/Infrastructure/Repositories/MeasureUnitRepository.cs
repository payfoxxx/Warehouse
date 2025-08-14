using Domain.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MeasureUnitRepository : IMeasureUnitRepository
    {
        private readonly AppDbContext _context;
        public MeasureUnitRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MeasureUnit entity)
        {
            await _context.MeasureUnits.AddAsync(entity);
        }

        public void Delete(MeasureUnit entity)
        {
            _context.MeasureUnits.Remove(entity);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.MeasureUnits.AnyAsync(c => c.Name.Equals(name));
        }

        public async Task<IEnumerable<MeasureUnit>> GetAllAsync()
        {
            return await _context.MeasureUnits.ToListAsync();
        }

        public async Task<IEnumerable<MeasureUnit>> GetAllByStateAsync(State state)
        {
            return await _context.MeasureUnits.Where(x => x.State == state).ToListAsync();
        }

        public async Task<MeasureUnit?> GetByIdAsync(Guid id)
        {
            return await _context.MeasureUnits.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsByNameAndIdAsync(string name, Guid id)
        {
            return await _context.MeasureUnits.AnyAsync(r => r.Name.Equals(name) && r.Id != id);
        }

        public async Task<bool> IsMeasureUnitUsedAsync(Guid id)
        {
            return await _context.Balances.AnyAsync(b => b.MeasureUnitId == id);
        }

        public void Update(MeasureUnit entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task<IEnumerable<MeasureUnit>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _context.MeasureUnits
                .Where(u => ids.Contains(u.Id))
                .ToListAsync();
        }
    }
}
