using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorEcommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("products")]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponeDTO>>>>
            ConvertCartItemsToCartProductDTO(List<CartItem> cartItems)
        {
            //cartItems will be sent in body, since its a post method
            var response = await _cartService.ConvertCartItemsToCartProductDTO(cartItems);
            return Ok(response);
        }
    }
}
