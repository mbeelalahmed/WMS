using GAC.Integration.Domain.Entities;

namespace GAC.Integration.Domain.Interfaces
{
    public interface ISalesOrderRepository
    {
        Task AddAsync(SalesOrder so);
        Task<List<SalesOrder>> GetAllAsync();
        Task<SalesOrder?> GetByIdAsync(Guid id);
        Task<bool> UpdateAsync(SalesOrder so);
        Task<bool> DeleteAsync(Guid id);
    }
}
