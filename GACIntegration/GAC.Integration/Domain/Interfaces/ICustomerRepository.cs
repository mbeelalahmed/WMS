using GAC.Integration.Domain.Entities;

namespace GAC.Integration.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task AddAsync(Customer customer);
        Task<List<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(Guid id);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(Customer customer);
    }
}
