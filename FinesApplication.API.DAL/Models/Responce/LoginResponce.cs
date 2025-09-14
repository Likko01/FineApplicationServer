namespace FinesApplication.API.DAL.Models.Responce
{
    public class LoginResponce
    {
        public string Token { get; set; }
        public User? User { get; set; }
    }
}
