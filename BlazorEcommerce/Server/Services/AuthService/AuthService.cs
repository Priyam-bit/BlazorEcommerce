using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace BlazorEcommerce.Server.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<string>> Login(string email, string password)
        {
            var response = new ServiceResponse<string>();
            //check if user exits
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                response.Success = false;
                response.Message = "Either email or password wrong";
            }
            else if (!VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Either email or password wrong";
            }
            else
            {
                response.Data = CreateJWT(user);
            }

            return response;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            //sets the password hash and salt for the new user, and adds th new user to table
            if (await UserExists(user.Email))
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "User already exists"
                };
            }

            GeneratePasswordHash(password, out byte[] passwordHash,
                out byte[] passwordSalt);
            //assign the salt and hash to user object
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;

            _context.Users.Add(user);
            await _context.SaveChangesAsync(); 

            return new ServiceResponse<int> { 
                Data = user.Id,
                Message = "Registration Successful!"
            };
        }

        public async Task<bool> UserExists(string email)
        { 
            //check if any user with same email exists in Users table
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            return existingUser == null ? false : true;
        }

        private void GeneratePasswordHash(string password, out byte[] passwordHash,
            out byte[] passwordSalt)
        {
            //util function to generate password hash using hmac SHA512 algo

            //The out is a keyword in C# which is used for passing
            //the arguments to methods as a reference
            //sets the salt and hash to the input byte arrays
            
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = 
                    hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPassword(string password, byte[] passwordHash, 
            byte[] passwordSalt)
        {
            //util function to verify password hash
            //get the hmac instance used to create password hash by passing the key (salt)
            //get the computed hash by using this hmac instance and password input

            //compare the computedHash and the passwordHash

            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash =
                    hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateJWT(User user)
        {
            //attach a list of claims to user's created token
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username.ToString()),
                new Claim(ClaimTypes.Name, user.Email.ToString())
            };

            //generate symmetric key using token value (unique key for our app that
            //we specified) from appSettings appsettings.json
            var key =
                new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                    _configuration.GetSection("AppSettings:Token").Value)
                );

            //create signingCredentials with this key
            var creds = 
                new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //create a new JWT with the list of claims, credentials
            var token = new JwtSecurityToken(
                    claims : claims,
                    expires : DateTime.Now.AddMonths(1),
                    signingCredentials : creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
