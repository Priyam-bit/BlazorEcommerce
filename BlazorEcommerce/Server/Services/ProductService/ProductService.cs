namespace BlazorEcommerce.Server.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly DataContext _context;

        public ProductService(DataContext dataContext)
        {
            _context = dataContext;
        }

        public async Task<ServiceResponse<List<Product>>> GetProductListAsync()
        {
            //get all products with their variants
            var products = await _context.Products.Include(p => p.ProductVariants).ToListAsync();
            var response = new ServiceResponse<List<Product>>()
            {
                Data = products
            };
            return response;
        }

        public async Task<ServiceResponse<Product>> GetProductAsync(int Id)
        {
            //get single product by Id, along with all its variants and its type
            var response = new ServiceResponse<Product>();
            var product = await _context.Products
                .Include(p => p.ProductVariants)
                .ThenInclude(pv => pv.ProductType)
                .FirstOrDefaultAsync(p => p.Id == Id);
            if(product == null)
            {
                response.Success = false;
                response.Message = "Sorry product not found";
            }
            else {
                response.Data = product;
            };
            return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryUrl)
        {
            //get all the products with their variants by category
            var response = new ServiceResponse<List<Product>>();
            var products =
                await _context.Products
                .Where(p => p.Category.Url.ToLower().Equals(categoryUrl.ToLower()))
                .Include(p => p.ProductVariants)
                .ToListAsync();
            if (products.Count == 0)
            {
                response.Success = false;
                response.Message = "Sorry no product found";
            }
            else
            {
                response.Data = products;
            };
            return response;
        }
    }
}
