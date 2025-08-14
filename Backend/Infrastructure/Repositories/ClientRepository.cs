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
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _context;
        public ClientRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Client entity)
        {
            await _context.Clients.AddAsync(entity);
        }

        public void Delete(Client entity)
        {
            _context.Clients.Remove(entity);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Clients.AnyAsync(c => c.Name.Equals(name));
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            return await _context.Clients.ToListAsync();
        }

        public async Task<IEnumerable<Client>> GetAllByStateAsync(State state)
        {
            return await _context.Clients.Where(x => x.State == state).ToListAsync();
        }

        public async Task<Client?> GetByIdAsync(Guid id)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsByNameAndIdAsync(string name, Guid id)
        {
            return await _context.Clients.AnyAsync(r => r.Name.Equals(name) && r.Id != id);
        }

        public async Task<bool> IsClientUsedAsync(Guid id)
        {
            return await _context.ShipmentDocuments.AnyAsync(b => b.ClientId == id);
        }

        public void Update(Client entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
