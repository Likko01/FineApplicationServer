using Azure;
using FinesApplication.API.BLL.Interfaces;
using FinesApplication.API.DAL.Models;
using FinesApplication.API.DAL.Models.DTO.FineDTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FinesApplication.API.BLL.Servises
{
    public class FineService : IFineService
    {
        public readonly ApplicationDbContext _context;
        public FineService (ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<string> PayFine(PayFineDTO FineDTO) // возвращаем true при успехе и false если не нашли штраф
        {
            var fine = await _context.Fines.FindAsync(FineDTO.FineId);
            if (fine == null)
            {
                return "-1"; // штраф не найден
            }
            if(fine.IsPaid == true)
            {
                return "0";
            }
            fine.IsPaid = true;

            await _context.SaveChangesAsync();
            return "1"; // всё успешно
        }


        public async Task<bool> CreateByUserId(CreateFineByUserIdDTO createFineDTO)
        {
            var user = await _context.Users.FindAsync(createFineDTO.UserId);
            if (user == null)
            {
                return false;
            }
            var newFine = new Fine
            {
                CarNumber = createFineDTO.CarNumber,
                Description = createFineDTO.Description,
                FineAmount = createFineDTO.FineAmount,
                IsPaid = createFineDTO.IsPaid,
                FineDate = DateTime.Now,
                UserId = user.Id
            };

            _context.Fines.Add(newFine);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CreateByUserName(CreateFineByUserNameDTO createFineDTO)
        {
            var user = await _context.Users
                   .FirstOrDefaultAsync(u => u.Name == createFineDTO.UserName);
            if (user == null)
            {
                return false;
            }

            var newFine = new Fine
            {
                CarNumber = createFineDTO.CarNumber,
                Description = createFineDTO.Description,
                FineAmount = createFineDTO.FineAmount,
                IsPaid = createFineDTO.IsPaid,
                FineDate = DateTime.Now,
                UserId = user.Id
            };

            _context.Fines.Add(newFine);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<string> Delete(DeleteFineDTO FineDTO) // возвращаем true при успехе и false если не нашли штраф
        {
            var fine = await _context.Fines.FindAsync(FineDTO.FineId);
            if (fine == null)
            {
                return "0"; //штраф не найден
            }
            _context.Fines.Remove(fine);
            await _context.SaveChangesAsync();
            return "1"; // успешно удаленно
        }
        public async Task<string> Edit(EditFineDTO FineDTO) // возвращаем true при успехе и false если не нашли штраф
        {
            var user = await _context.Users.FindAsync(FineDTO.UserId); // в будующем будем записывать какой админ изменил штраф в логи
            var fine = await _context.Fines.FindAsync(FineDTO.FineId);
            if (fine == null)
            {
                return "0"; // штраф не найден
            }
            fine.CarNumber = FineDTO.CarNumber;
            fine.FineAmount = FineDTO.FineAmount;
            fine.IsPaid = FineDTO.IsPaid;
            fine.Description = FineDTO.Description;

            await _context.SaveChangesAsync();
            return "1"; // всё успешно
        }


        public async Task<List<Fine>> ShowAll() // после поиска нужно отправить весь штраф а не только данные что мы выводим. нужно ибо метод pay принимает id штрафа и мы должны его хранить
        {
            List<Fine> fines = await _context.Fines.ToListAsync();
            return fines;
        }

        public async Task LoadFineOnDB()
        {
            int pageSize = 10;
            List<Fine> fines = new List<Fine>();
            Random random = new Random();

            for (int i = 0; i <= 32; i++)
            {
                fines.Add(new Fine()
                {
                    CarNumber = "A" + i,
                    Description = "description " + i,
                    FineAmount = random.Next(1, 101) * 100,
                    IsPaid = false,
                    FineDate = DateTime.Now,
                    UserId = 5
                });
            }
            _context.Fines.AddRange(fines);
            await _context.SaveChangesAsync();
        }
        public async Task<IndexFineDTO> Index(
            int page,
            int pageSize,
            string? selectedSortOption,
            string? userId, //+
            string? carNumber, //+
            decimal? minFineAmount, //+
            decimal? maxFineAmount, //+
            string? selectedPaidOption, //+
            string? description, //+
            DateTime? fromDateTime, //+
            DateTime? toDateTime, //+
            string? selectedAscendingOption,
            string sender)
        {
            // Создаем базовый запрос с фильтрацией
            var finesQuery = _context.Fines.AsQueryable();

            // Фильтрация
            if (userId != null)
            {
                finesQuery = finesQuery.Where(f => f.UserId == int.Parse(userId));
            }
            if (carNumber != null)
            {
                finesQuery = finesQuery.Where(f => f.CarNumber == carNumber);
            }
            if (minFineAmount != null)
            {
                finesQuery = finesQuery.Where(f => f.FineAmount > minFineAmount);
            }
            if (maxFineAmount != null)
            {
                finesQuery = finesQuery.Where(f => f.FineAmount < maxFineAmount);
            }
            if (selectedPaidOption != null)
            {
                finesQuery = selectedPaidOption == "paid" ? finesQuery.Where(f => f.IsPaid == true)
                        : selectedPaidOption == "not paid" ? finesQuery.Where(f => f.IsPaid == false) : finesQuery;
            }
            if (description != null)
            {
                finesQuery = finesQuery.Where(f => f.Description == description);
            }
            if (fromDateTime != null)
            {
                finesQuery = finesQuery.Where(f => f.FineDate > fromDateTime);
            }
            if (toDateTime != null)
            {
                finesQuery = finesQuery.Where(f => f.FineDate < toDateTime);
            }



            //Сортировка 
            bool ascending = selectedAscendingOption?.ToLower() == "ascending";
            if (!string.IsNullOrEmpty(selectedSortOption))
            {
                finesQuery = selectedSortOption switch
                {
                    "FineAmount" => ascending ? finesQuery.OrderBy(f => f.FineAmount) : finesQuery.OrderByDescending(f => f.FineAmount),
                    "DateSearch" => ascending ? finesQuery.OrderBy(f => f.FineDate) : finesQuery.OrderByDescending(f => f.FineDate),
                    "UserId" => ascending ? finesQuery.OrderBy(f => f.UserId) : finesQuery.OrderByDescending(f => f.UserId),
                    "CarNumber" => ascending ? finesQuery.OrderBy(f => f.CarNumber) : finesQuery.OrderByDescending(f => f.CarNumber),
                    "IsPaid" => ascending ? finesQuery.OrderBy(f => f.IsPaid) : finesQuery.OrderByDescending(f => f.IsPaid),
                    "Discription" => ascending ? finesQuery.OrderBy(f => f.Description) : finesQuery.OrderByDescending(f => f.Description),
                    _ => finesQuery // Если поле сортировки не распознано, сортировка не применяется
                };
            }
            else
            {
                // Значение сортировки по умолчанию (например, по дате)
                finesQuery = finesQuery.OrderByDescending(f => f.FineDate);
            }


            // Подсчитываем общее количество записей, удовлетворяющих условиям
            int totalCount = await finesQuery.CountAsync();
            // Применяем пагинацию
            var paginatedQuery = finesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize);


            /*
            // Выполняем запрос для получения записей текущей страницы
            if (sender == "User")
            {
                // Возвращаем штрафы без колонки UserId
                List<FineWithoutUserId> fines = await paginatedQuery.Select(f => new FineWithoutUserId
                {
                    CarNumber = f.CarNumber,
                    Description = f.Description,
                    FineAmount = f.FineAmount,
                    IsPaid = f.IsPaid,
                    FineDate = f.FineDate
                }).ToListAsync();

                // Рассчитываем общее количество страниц
                int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                return new IndexFineDTO
                {
                    fineWithoutUserId = fines,
                    totalCount = totalCount,
                    totalPages = totalPages
                };
            }
            else
            {
            */
                // Возвращаем штрафы без изменений
                List<Fine> fines = await paginatedQuery.ToListAsync();

                // Рассчитываем общее количество страниц
                int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                return new IndexFineDTO
                {
                    fines = fines,
                    totalCount = totalCount,
                    totalPages = totalPages
                };
            
            //}
            
        }
    }
}
