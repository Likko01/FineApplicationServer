using FinesApplication.API.DAL.Models;
using FinesApplication.API.DAL.Models.DTO.FineDTO;

namespace FinesApplication.API.BLL.Interfaces
{
    public interface IFineService
    {
        Task<bool> CreateByUserId(CreateFineByUserIdDTO createFineDTO);
        Task<string> Edit(EditFineDTO editFineDTO);
        Task<bool> CreateByUserName(CreateFineByUserNameDTO createFineDTO);
        Task<string> Delete(DeleteFineDTO FineDTO);
        Task<List<Fine>> ShowAll();
        Task<string> PayFine(PayFineDTO FineDTO);
    }
}
