using Microsoft.AspNetCore.Mvc;
using FinesApplication.API.DAL.Models;
using Microsoft.EntityFrameworkCore;
using FinesApplication.API.BLL.Servises;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FinesApplication.API.DAL.Models.DTO.FineDTO;
using FinesApplication.API.BLL.Interfaces;
using System.Reflection;

namespace FinesApplication.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FineController : ControllerBase
    {
        private readonly FineService _fineServise;
        public FineController(ApplicationDbContext context, FineService fineServise)
        {
            _fineServise = fineServise;
        }
        [HttpPut("PayFine")]
        public async Task<IActionResult> PayFine(PayFineDTO FineDTO)
        {
            var result = await _fineServise.PayFine(FineDTO);
            if (result == "-1")
            {
                //штраф не найден
                return NotFound("Fine not found");
            }
            if(result == "0")
            {
                //штраф не найден
                return BadRequest("Fine already paid");
            }
            //успешно изменение
            return Ok();
        }
        [HttpGet("Get")]
        public async Task<IActionResult> GetAllFInes()
        {
            var fines = await _fineServise.ShowAll();
            return Ok(fines);
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteFine([FromBody] DeleteFineDTO FineID)
        {
            var result = await _fineServise.Delete(FineID);
            if (result == "0")
            {
                //штраф не найден
                return NotFound("Fine not found");
            }
            else if (result == "1") 
            {
                //успешно удаленно
                return Ok();
            }
            return BadRequest();

        }
        [HttpPost("Create/ByUserId")]
        public async Task<IActionResult> CreateFineByUserId([FromBody] CreateFineByUserIdDTO createFineDTO)
        {
            if (await _fineServise.CreateByUserId(createFineDTO))
            {
                return Ok();
            }
            return NotFound();
        }
        [HttpPost("Create/ByUserName")]
        public async Task<IActionResult> CreateFineByUserName([FromBody] CreateFineByUserNameDTO createFineDTO)
        {
            if (await _fineServise.CreateByUserName(createFineDTO))
            {
                return Ok();
            }
            return NotFound();
        }
        [HttpPut("EditFine")]
        public async Task<IActionResult> EditFine([FromBody] EditFineDTO FineDTO)
        {
            var result = await _fineServise.Edit(FineDTO);
            if (result == "0")
            {
                //штраф не найден
                return NotFound("Fine not found");
            }
            else
            {
                //успешно изменение
                return Ok();
            }
        }


        [HttpGet("{page?}")]
        public async Task<IActionResult> Index(
            int page,
            int pageSize,
            string? selectedSortOption,
            string? userId,
            string? carNumber,
            decimal? minFineAmount,
            decimal? maxFineAmount,
            string? selectedPaidOption,
            string? description,
            DateTime? fromDateTime,
            DateTime? toDateTime,
            string? selectedAscendingOption,
            string sender)
        {
            int a = 20;
            try
            {
                // Переносим все параметры в сервис для обработки
                var response = await _fineServise.Index(
                    page,
                    pageSize,
                    selectedSortOption,
                    userId,
                    carNumber,
                    minFineAmount,
                    maxFineAmount,
                    selectedPaidOption,
                    description,
                    fromDateTime,
                    toDateTime,
                    selectedAscendingOption,
                    sender
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка на сервере: {ex.Message}");
            }
        }
        [HttpGet("addInfo")]
        public async Task<IActionResult> FineLoad()
        {
            await _fineServise.LoadFineOnDB();
            return Ok();
        }
    }
}
