using Microsoft.EntityFrameworkCore;
using ProductControl.Application.DTOs;
using ProductControl.Core.Entities;
using ProductControl.Core.Interfaces;
using ProductControl.Infrastracture.Persistence;

namespace ProductControl.Infrastracture.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;

    public ProductRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int id, int userId)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId && !p.IsDeleted);
    }

    public async Task<IEnumerable<Product>> GetByFilterAsync(ProductFilterDto filterDto, int userId)
    {
        var query = _context.Products
            .Where(p => p.UserId == userId && !p.IsDeleted);
        
        if (!string.IsNullOrEmpty(filterDto.Name))
        {
            query = query.Where(p => p.Name.Contains(filterDto.Name));
        }

        if (filterDto.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= filterDto.MinPrice.Value);
        }

        if (filterDto.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= filterDto.MaxPrice.Value);
        }

        if (filterDto.IsAvailable.HasValue)
        {
            query = query.Where(p => p.IsAvailable == filterDto.IsAvailable.Value);
        }
        
        return await query.ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    
    public async Task<List<Product>> GetAllByUserIdAsync(int userId, bool includeDeleted = false)
    {
        var query = _context.Products.AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return await query.Where(p => p.UserId == userId).ToListAsync();
    }
    
    public void RemoveRange(IEnumerable<Product> products)
    {
        _context.Products.RemoveRange(products);
    }
}