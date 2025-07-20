using GAC.Integration.Domain.Entities;
using GAC.Integration.Domain.Interfaces;
using GAC.Integration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GAC.Integration.Infrastructure.Repositories
{
    public class SalesOrderRepository : ISalesOrderRepository
    {
        private readonly AppDbContext _context;
        public SalesOrderRepository(AppDbContext context) => _context = context;

        public async Task AddAsync(SalesOrder so)
        {
            _context.SalesOrders.Add(so);
            await _context.SaveChangesAsync();
        }
        public async Task<List<SalesOrder>> GetAllAsync()
        {
            return await _context.SalesOrders
                .Include(po => po.Lines)
                .ToListAsync();
        }

        public async Task<SalesOrder?> GetByIdAsync(Guid id)
        {
            return await _context.SalesOrders
                .Include(po => po.Lines)
                .FirstOrDefaultAsync(po => po.Id == id);
        }

        public async Task<bool> UpdateAsync(SalesOrder so)
        {
            _context.SalesOrders.Update(so);
            var result = await _context.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var so = await _context.SalesOrders.FindAsync(id);
            if (so != null)
            {
                _context.SalesOrders.Remove(so);
                var result = await _context.SaveChangesAsync();

                return result > 0;
            }
            return false;
        }
    }
}
