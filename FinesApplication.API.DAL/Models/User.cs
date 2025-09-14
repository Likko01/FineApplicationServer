using System.ComponentModel.DataAnnotations;
namespace FinesApplication.API.DAL.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        //Id


        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        //Name


        [Required]
        public byte[] PasswordHash { get; set; }
        //PasswordHash


        [Required]
        public byte[] PasswordSalt { get; set; }
        //PasswordSalt


        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }
        //Email


        [StringLength(100)]
        public string? Telegram { get; set; }
        //Telegram


        public string? NotificationMethod { get; set; }
        //NotificationMethod


        [Required]
        public string Role { get; set; }
        //Role


        public ICollection<Fine> fines { get; set; }
        public ICollection<History> users { get; set; }
        //Навигационные свойства
    }
}
