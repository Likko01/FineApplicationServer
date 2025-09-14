using FinesApplication.API.BLL.Interfaces;
using FinesApplication.API.DAL.Models;
using FinesApplication.API.DAL.Models.Responce;
using FinesApplication.API.DAL.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FinesApplication.API.DAL.Models.Enam;
using FinesApplication.API.DAL.Models.DTO.UserDTO;

namespace FinesApplication.API.BLL.Servises
{
    public class UserService : IUserService
    {
        public readonly ApplicationDbContext _context;
        public readonly IConfiguration _configuration;
        public UserService (ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<LoginResponce> Login(AuthorizationDTO authorizationDTO) // -1 не найден // 0 неверный пароль // JWT token все верно
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == authorizationDTO.Name);
            if (user == null)
            {
                return new LoginResponce { Token = "-1", User = new User { } }; // пользователь не найден
            }
            if (VerifyPasswordHash(authorizationDTO.Password, user.PasswordHash, user.PasswordSalt))
            {
                var jwtToken = GenerateJwtToken(user);
                 return new LoginResponce { Token = jwtToken, User = user }; // пользователь найден
            }
            return new LoginResponce { Token = "0", User = new User { } }; // неверный пароль
        }

        public async Task<LoginResponce> RegisterUser(RegistrationDTO registrationDTO)
        {
            // Проверка, существует ли пользователь с таким именем
            if (await _context.Users.AnyAsync(u => u.Name == registrationDTO.Name))
            {
                return new LoginResponce { Token = "-1"};  //Пользователь с таким именем уже существует
            }

            using var hmac = new HMACSHA512();

            var user = new User
            {
                Name = registrationDTO.Name,
                PasswordSalt = hmac.Key,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registrationDTO.Password)),
                Role =  DAL.Models.Enam.Role.User.ToString()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var jwtToken = GenerateJwtToken(user);
            return new LoginResponce { Token = jwtToken, User = user }; // пользователь найден
        }
        public async Task<LoginResponce> RegisterAdmin(RegistrationDTO registrationDTO)
        {
            // Проверка, существует ли пользователь с таким именем
            if (await _context.Users.AnyAsync(u => u.Name == registrationDTO.Name))
            {
                return new LoginResponce { Token = "-1" };  //Пользователь с таким именем уже существует
            }

            using var hmac = new HMACSHA512();

            var user = new User
            {
                Name = registrationDTO.Name,
                PasswordSalt = hmac.Key,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registrationDTO.Password)),
                Role = DAL.Models.Enam.Role.Admin.ToString()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var jwtToken = GenerateJwtToken(user);
            return new LoginResponce { Token = jwtToken, User = user }; // пользователь найден
        }
        public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }
        private string GenerateJwtToken(User user)
        {
            //обработчик токена
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

            var issuer = _configuration.GetSection("Jwt:Issuer").Value;
            var audience = _configuration.GetSection("Jwt:Audience").Value;
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("Jwt:Key").Value);

            //Дескриптор токена - объект, который содержит настройки и данные для создания JWT-токена
            //Он определяет:
            // Утверждения (Claims): информация, которую токен будет содержать (например, имя пользователя)
            // Срок действия (Expires): когда токен истекает
            //Подпись (SigningCredentials): ключ и алгоритм, используемые для подписания токена
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Name) }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = issuer,
                Audience = audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<string> DeleteUser(DeleteUserDTO deleteUserDTO)
        {
            var user = await _context.Users.FindAsync(deleteUserDTO.userID);
            // Проверка, существует ли пользователь с таким именем
            if (user != null)
            {
                //удаляем пользователя
                _context.Remove(user);
                await _context.SaveChangesAsync();
                return "";
            }
            return "-1";
        }
    }
}
