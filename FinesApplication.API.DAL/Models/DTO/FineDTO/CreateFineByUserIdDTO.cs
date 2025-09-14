namespace FinesApplication.API.DAL.Models.DTO.FineDTO
{
    public class CreateFineByUserIdDTO
    {
        public int UserId { get; set; }
        public string CarNumber { get; set; }
        public decimal FineAmount { get; set; }
        public bool IsPaid { get; set; }
        public string Description { get; set; }
    }
}
