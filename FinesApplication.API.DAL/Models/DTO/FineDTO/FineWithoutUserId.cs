using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FinesApplication.API.DAL.Models.DTO.FineDTO
{
    public class FineWithoutUserId
    {

        [Key]
        public int Id { get; set; }
        //Id

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
    }
}
