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

        public async Task<ServiceResponse<List<Product>>> SearchProducts(string searchText)
        {
            //return products which contain searchText in either title or description
            var response = new ServiceResponse<List<Product>>
            {
                Data = await GetProductsBySearchText(searchText)
            };

            return response;
        }

        //util function to return products which contain searchText in either title or description
        private async Task<List<Product>> GetProductsBySearchText(string searchText)
        {
            return await _context.Products
                                .Where(p => p.Title.ToLower().Contains(searchText.ToLower())
                                || p.Description.ToLower().Contains(searchText.ToLower()))
                                .Include(p => p.ProductVariants)
                                .ToListAsync();
        }

        public async Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string searchText)
        {
            var products = await GetProductsBySearchText(searchText);
            List<string> result = new List<string>();
            foreach (var product in products)
            {
                //suggest product titles, whose titles contain searchText
                if (product.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(product.Title);
                }

                //suggest words in product description, which conatin searchText
                if (product.Description != null)
                {
                    var punctuations = product.Description.Where(char.IsPunctuation)
                        .Distinct().ToArray();
                    var words = product.Description.Split()
                        .Select(s => s.Trim(punctuations));
                    foreach (var word in words)
                    {
                        if (word.Contains(searchText, StringComparison.OrdinalIgnoreCase) &&
                            !result.Contains(word))
                        {
                            result.Add(word);
                        }
                    }
                }
            }
            return new ServiceResponse<List<string>> { Data = result };
        }
    }
}
