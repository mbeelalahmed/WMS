using GAC.Integration.Domain.Entities;

namespace GAC.Integration.Domain.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        Task AddAsync(PurchaseOrder po);
        Task<List<PurchaseOrder>> GetAllAsync();
        Task<PurchaseOrder?> GetByIdAsync(Guid id);
        Task UpdateAsync(PurchaseOrder po);
        Task DeleteAsync(Guid id);
    }
}
