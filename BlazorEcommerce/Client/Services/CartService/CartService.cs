using Blazored.LocalStorage;

namespace BlazorEcommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _http;

        public CartService(ILocalStorageService localStorage, HttpClient http)
        {
            _localStorage = localStorage;
            _http = http;
        }
        public event Action OnChange;

        public async Task AddToCart(CartItem cartItem)
        {
            //get cart from localstorage
            //GetItemAsync<type of item>(item_name)
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if(cart == null)
            {
                //if no cart exists in local storage
                //initialize cart as a list of empty cart items
                cart = new List<CartItem>();
            }
            //add item(with its quantity) to cart, check if there exists same item in cart already
            //if so update its quantity
            var sameItem = cart.Find(x => x.ProductId == cartItem.ProductId &&
                x.ProductTypeId == cartItem.ProductTypeId);
            if(sameItem == null)
            {
                cart.Add(cartItem);
            } 
            else
            {
                sameItem.Quantity += cartItem.Quantity;
            }

            //save the cart object as variable cart in localstorage
            await _localStorage.SetItemAsync("cart", cart);
            //notify all subscribers cart has changed
            OnChange.Invoke();
        }


        public async Task<List<CartItem>> GetCartItems()
        {
            //get cart from localstorage
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart == null)
            {
                return new List<CartItem>();
            }
            return cart;
        }
        public async Task<List<CartProductResponeDTO>> GetCartProducts()
        {
            var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            var response = await _http.PostAsJsonAsync("api/cart/products", cartItems);
            //http response
            var cartProducts =
                await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponeDTO>>>();
            return cartProducts.Data;
        }

        public async Task RemoveProductFromCart(int productId, int productTypeId)
        {
            //check if cart exists in local storage
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart == null) return;
            //retrive item from cart
            var cartItem = cart.FirstOrDefault(x => x.ProductId == productId &&
                x.ProductTypeId == productTypeId);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
                await _localStorage.SetItemAsync("cart", cart);
                OnChange.Invoke();
            }
        }

        public async Task UpdateQuantiy(CartProductResponeDTO product)
        {
            //check if cart exists in local storage
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart == null) return;
            //retrive item from cart
            var cartItem = cart.FirstOrDefault(x => x.ProductId == product.ProductId &&
                x.ProductTypeId == product.ProductTypeId);
            if (cartItem != null)
            {
                cartItem.Quantity = product.Quantity;
                await _localStorage.SetItemAsync("cart", cart);
                OnChange.Invoke();
            }
        }
    }
}
