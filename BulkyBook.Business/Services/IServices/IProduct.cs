using BulkyBook.Models;


namespace BulkyBook.Business.Services.IServices
{
    public interface IProductService
    {

      Task<Product?> GetProductByIdAsync(int id);
      Task<IEnumerable<Product>> GetAllProductsAsync();
      Task<Product> CreateProductAsync(Product product);
      Task UpdateProductAsync(Product product);
      Task DeleteProductAsync(int id);

      Task<bool> IsProductNameUniqueAsync(string name, int? id = null);
    }
}
