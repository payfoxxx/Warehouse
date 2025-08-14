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
    public class ResourceRepository : IResourceRepository
    {
        private readonly AppDbContext _context;
        public ResourceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Resource entity)
        {
            await _context.Resources.AddAsync(entity);
        }

        public void Delete(Resource entity)
        {
            _context.Resources.Remove(entity);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Resources.AnyAsync(c => c.Name.Equals(name));
        }

        public async Task<IEnumerable<Resource>> GetAllAsync()
        {
            return await _context.Resources.ToListAsync();
        }

        public async Task<IEnumerable<Resource>> GetAllByStateAsync(State state)
        {
            return await _context.Resources.Where(x => x.State == state).ToListAsync();
        }

        public async Task<Resource?> GetByIdAsync(Guid id)
        {
            return await _context.Resources.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsByNameAndIdAsync(string name, Guid id)
        {
            return await _context.Resources.AnyAsync(r => r.Name.Equals(name) && r.Id != id);
        }

        public async Task<bool> IsResourceUsedAsync(Guid id)
        {
            return await _context.Balances.AnyAsync(b => b.ResourceId == id);
        }

        public void Update(Resource entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task<IEnumerable<Resource>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _context.Resources
                .Where(r => ids.Contains(r.Id))
                .ToListAsync();
        }
    }
}
