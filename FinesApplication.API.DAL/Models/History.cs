using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinesApplication.API.DAL.Models
{
    public class History
    {
        [Key]
        public int Id { get; set; }
        //Id


        [Required]
        public int UserId { get; set; }
        //UserId


        [Required]
        public string EventDescription { get; set; }
        //EventDescription


        [Required]
        public DateTime EventDate { get; set; }
        //EventDate


        [ForeignKey("UserId")]
        public User User { get; set; }
        //ForeignKey
    }
}
