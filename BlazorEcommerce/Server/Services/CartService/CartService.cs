namespace BlazorEcommerce.Server.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly DataContext _context;

        public CartService(DataContext context)
        {
            _context = context;
        }
        public async Task<ServiceResponse<List<CartProductResponeDTO>>> ConvertCartItemsToCartProductDTO(List<CartItem> cartItems)
        {
            var result = new ServiceResponse<List<CartProductResponeDTO>>
            {
                Data = new List<CartProductResponeDTO>()
            };

            //go through every single cartItem and get the corresponding product
            //along with its variant from context
            //create a new cartProductDto object from the info and return the list
            //of cartProductDTOs
            foreach (var item in cartItems)
            {
                var product = await _context.Products
                    .Where(p => p.Id == item.ProductId)
                    .FirstOrDefaultAsync();

                if (product == null) continue;

                var productVariant = await _context.ProductVariants
                    .Where(v => v.ProductId == item.ProductId
                           && v.ProductTypeId == item.ProductTypeId)
                    .Include(v => v.ProductType)
                    .FirstOrDefaultAsync();
                
                if(productVariant == null) continue;

                var cartProduct = new CartProductResponeDTO
                {
                    ProductId = product.Id,
                    Title = product.Title,
                    ProductTypeId = productVariant.ProductTypeId,
                    ProductType = productVariant.ProductType.Name,
                    ImageUrl = product.ImageUrl,
                    Price = productVariant.Price,
                    Quantity = item.Quantity
                };
                result.Data.Add(cartProduct);
            }
            return result;
        }
    }
}
