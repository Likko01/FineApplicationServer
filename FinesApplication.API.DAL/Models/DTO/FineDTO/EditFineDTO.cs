namespace FinesApplication.API.DAL.Models.DTO.FineDTO
{
    public class EditFineDTO
    {
        public int UserId { get; set; }
        public int FineId { get; set; }
        public string CarNumber { get; set; }
        public decimal FineAmount { get; set; }
        public bool IsPaid { get; set; }
        public string Description { get; set; }
    }
}
