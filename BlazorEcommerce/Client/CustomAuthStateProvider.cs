using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace BlazorEcommerce.Client
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _http;

        public CustomAuthStateProvider(ILocalStorageService localStorageService, HttpClient http)
        {
            _localStorageService = localStorageService;
            _http = http;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //gets the auth token from local storage
            //parses the claims and creates a new claims identity
            //then it notifies the components that want to be notified of this new
            //claims identity
            //with that info the application will know that whether the user is 
            //authenticated or not authenticated

            string authToken = await _localStorageService.GetItemAsStringAsync("authToken");
            var identity = new ClaimsIdentity();
            //we want to set a defaut request header for the authorization header
            //using http client
            _http.DefaultRequestHeaders.Authorization = null; //unauthorized

            //check the auth token and parse the claims and then set the authorization
            //header to the new auth token we found in the local storage
            if(!string.IsNullOrEmpty(authToken))
            {
                try
                {
                    //ParseClaimsFromJwt taken from
                    //https://github.com/SteveSandersonMS/presentation-2019-06-NDCOslo/blob/master/demos/MissionControl/MissionControl.Client/Util/ServiceExtensions.cs
                    identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");
                    //set header, and set the Bearer token to auth token without quotation marks
                    _http.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", authToken.Replace("\"", ""));
                }
                catch 
                {
                    //if it fails, remove the token from local storage and 
                    //set the identity to an empty claims identity to make the user
                    //unauthorized
                    await _localStorageService.RemoveItemAsync("authToken");
                    identity = new ClaimsIdentity();
                }
            }

            //set the user as bearer of this identity
            var user = new ClaimsPrincipal(identity);
            //get user's state
            var state = new AuthenticationState(user);

            NotifyAuthenticationStateChanged(Task.FromResult(state));

            return state;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch(base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }
    }
}
