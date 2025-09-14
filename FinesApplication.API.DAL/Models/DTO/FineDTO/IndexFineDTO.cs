namespace FinesApplication.API.DAL.Models.DTO.FineDTO
{
    public class IndexFineDTO
    {
        public List<Fine>? fines { get; set; }
        public List<FineWithoutUserId>? fineWithoutUserId { get; set; }
        public int totalCount { get; set; }
        public int totalPages { get; set; }
    }
}
