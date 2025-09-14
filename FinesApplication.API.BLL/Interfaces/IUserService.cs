using FinesApplication.API.DAL.Models.DTO;
using FinesApplication.API.DAL.Models.DTO.UserDTO;
using FinesApplication.API.DAL.Models.Responce;

namespace FinesApplication.API.BLL.Interfaces
{
    public interface IUserService
    {
        Task<LoginResponce> RegisterUser(RegistrationDTO registrationUserDTO);
        Task<LoginResponce> RegisterAdmin(RegistrationDTO registrationUserDTO);
        Task<LoginResponce> Login(AuthorizationDTO authorizationUserDTO);
        Task<string> DeleteUser(DeleteUserDTO deleteUserDTO );
        

    }
}
