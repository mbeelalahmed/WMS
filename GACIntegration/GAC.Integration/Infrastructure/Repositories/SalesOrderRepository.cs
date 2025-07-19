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
            return await _context.SalesOrders.ToListAsync();
        }
    }
}
