using MediPulses.DAL.Entities;

namespace MediPulses.BLL.Interfaces;

public interface ISupplierService
{
    Task<List<Supplier>> GetAllAsync();
    Task<Supplier?> GetByIdAsync(int id);
    Task CreateAsync(Supplier supplier);
    Task<bool> UpdateAsync(Supplier supplier);
    Task<bool> DeleteAsync(int id);
}
