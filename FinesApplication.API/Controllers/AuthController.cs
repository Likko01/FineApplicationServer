using FinesApplication.API.BLL.Interfaces;
using FinesApplication.API.DAL.Models.DTO;
using FinesApplication.API.DAL.Models.DTO.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.AccessControl;

namespace FinesApplication.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("register/user")]
        public async Task<IActionResult> RegisterUser (RegistrationDTO registrationDTO)
        {
            var searchResult = await _userService.RegisterUser(registrationDTO);
            if (searchResult.Token == "-1") //Пользователь с таким именем уже существует
            {
                return Conflict(new { message = "Пользователь с таким именем уже существует." }); // Возвращаем статус 409 (Conflict) и сообщение об ошибке
            }
            return Ok(new { token = searchResult.Token, user = searchResult.User }); // Возвращаем 200 OK с токеном;
        }
        [HttpPost("register/admin")]
        public async Task<IActionResult> RegisterAdmin(RegistrationDTO registrationDTO)
        {
            var searchResult = await _userService.RegisterAdmin(registrationDTO);
            if (searchResult.Token == "-1") //Пользователь с таким именем уже существует
            {
                return Conflict(new { message = "Пользователь с таким именем уже существует." }); // Возвращаем статус 409 (Conflict) и сообщение об ошибке
            }
            return Ok(new { token = searchResult.Token, user = searchResult.User }); // Возвращаем 200 OK с токеном;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthorizationDTO authorizationDTO)
         {
            var searchResult = await _userService.Login(authorizationDTO); // Получаем токен и роль
            // Пользователь не найден
            if (searchResult.Token == "-1")
            {
                return NotFound(new { message = "Пользователь не найден" }); // Возвращаем 404 NotFound
            }
            // Неверный пароль
            if (searchResult.Token == "0")
            {
                return Unauthorized(new { message = "Неверный пароль" }); // Возвращаем 401 Unauthorized
            }
            // Успешная авторизация
            return Ok(new { token = searchResult.Token, user = searchResult.User});
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(DeleteUserDTO deleteUserDTO)
        {
            var searchResult = await _userService.DeleteUser(deleteUserDTO);
            if (searchResult == "-1") //Пользователь не найден
            {
                return NotFound(new { message = "Пользователь не найден." }); // Возвращаем статус 404 (NotFound) и сообщение об ошибке
            }
            return Ok(); // Возвращаем 200 OK;
        }
    }
}
