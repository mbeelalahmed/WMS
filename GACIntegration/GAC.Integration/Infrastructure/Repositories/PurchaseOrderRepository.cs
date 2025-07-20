using GAC.Integration.Domain.Entities;
using GAC.Integration.Domain.Interfaces;
using GAC.Integration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GAC.Integration.Infrastructure.Repositories
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly AppDbContext _context;
        public PurchaseOrderRepository(AppDbContext context) => _context = context;

        public async Task AddAsync(PurchaseOrder po)
        {
            _context.PurchaseOrders.Add(po);
            await _context.SaveChangesAsync();
        }
        public async Task<List<PurchaseOrder>> GetAllAsync()
        {
            return await _context.PurchaseOrders
                .Include(po => po.Lines)
                .ToListAsync();
        }

        public async Task<PurchaseOrder?> GetByIdAsync(Guid id)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Lines)
                .FirstOrDefaultAsync(po => po.Id == id);
        }

        public async Task UpdateAsync(PurchaseOrder po)
        {
            _context.PurchaseOrders.Update(po);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var po = await _context.PurchaseOrders.FindAsync(id);
            if (po != null)
            {
                _context.PurchaseOrders.Remove(po);
                await _context.SaveChangesAsync();
            }
        }
    }
}
