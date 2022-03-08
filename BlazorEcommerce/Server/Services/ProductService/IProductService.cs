namespace BlazorEcommerce.Server.Services.ProductService
{
    public interface IProductService
    {
        Task<ServiceResponse<List<Product>>> GetProductListAsync();
        Task<ServiceResponse<Product>> GetProductAsync(int Id);

        Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryUrl);
    }
}
