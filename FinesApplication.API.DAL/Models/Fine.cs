using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Runtime.CompilerServices;

namespace FinesApplication.API.DAL.Models
{    
    public class Fine
    {
        [Key]
        public int Id { get; set; }
        //Id


        [Required]
        public int UserId { get; set; }
        //UserId


        [Required]
        [MaxLength(20)]
        public string CarNumber { get; set; }
        //CarNumber


        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal FineAmount { get; set; }
        //FineAmount


        [Required]
        public bool IsPaid { get; set; }
        //IsPaid


        [Required]
        public string Description { get; set; }
        //Description


        [Required]
        public DateTime FineDate { get; set; }
        //FineDate


        [ForeignKey("UserId")]
        public User User { get; set; }
        //ForeignKey
    }
}

