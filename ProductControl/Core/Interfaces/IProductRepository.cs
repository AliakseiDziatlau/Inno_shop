using ProductControl.Application.DTOs;
using ProductControl.Core.Entities;

namespace ProductControl.Core.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id, int userId);
    Task<IEnumerable<Product>> GetByFilterAsync(ProductFilterDto filterDto, int userId);
    Task AddAsync(Product product);
    Task DeleteAsync(Product product);
    Task SaveChangesAsync();
    Task<List<Product>> GetAllByUserIdAsync(int userId, bool includeDeleted = false);
    void RemoveRange(IEnumerable<Product> products);
}