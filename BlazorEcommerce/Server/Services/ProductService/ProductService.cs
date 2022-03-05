﻿namespace BlazorEcommerce.Server.Services.ProductService
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
            var products = await _context.Products.ToListAsync();
            var response = new ServiceResponse<List<Product>>()
            {
                Data = products
            };
            return response;
        }

        public async Task<ServiceResponse<Product>> GetProductAsync(int Id)
        {
            var product = await _context.Products.FindAsync(Id);
            var response = new ServiceResponse<Product>();
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
    }
}