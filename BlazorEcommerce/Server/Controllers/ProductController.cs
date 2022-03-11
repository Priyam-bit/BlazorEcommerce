using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorEcommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        //constructor, inject IProductService to pass on the client request to the service
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProducts()
        {
            //forward gthe request to service, and return its response to client
            var response = await _productService.GetProductListAsync();
            return Ok(response);
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProduct(int productId)
        {
            //forward the request to service, and return its response to client
            var response = await _productService.GetProductAsync(productId);
            return Ok(response);
        }

        [HttpGet("category/{categoryUrl}")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProductsByCategory(string categoryUrl)
        {
            var response = await _productService.GetProductsByCategory(categoryUrl);
            return Ok(response);
        }

        [HttpGet("search/{searchText}")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> SearchProducts(string searchText)
        {
            var response = await _productService.SearchProducts(searchText);
            return Ok(response);
        }

        [HttpGet("searchSuggestions/{searchText}")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProductSearchSuggestions(string searchText)
        {
            var response = await _productService.GetProductSearchSuggestions(searchText);
            return Ok(response);
        }
    }
}
