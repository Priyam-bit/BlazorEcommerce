

namespace BlazorEcommerce.Client.Services.ProductService
{
    public interface IProductService
    {
        event Action ProductsChanged;
        List<Product> Products { get; set; }
        //to display search result msg (eg- loading products, no product found)
        string Message { get; set; }
        //current search results page
        int CurrentPage { get; set; }
        //total search result page
        int PageCount { get; set; }
        string LastSearchText { get; set; }
        Task GetProducts(string? categoryUrl = null);
        Task<ServiceResponse<Product>> GetProductById(int productId);
        Task SearchProducts(string searchText, int page);
        Task<List<string>> GetProductSearchSuggestions(string searchText);
        Task<List<Product>> GetFeaturedProducts();
    }
}
