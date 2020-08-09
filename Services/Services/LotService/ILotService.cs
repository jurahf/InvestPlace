using Services.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.LotService
{
    public interface ILotService
    {
        Task<List<LotDto>> GetAllAsync();

        List<LotDto> GetActual(bool actual);

        LotDto GetById(int id);

        List<LotDto> GetByUser(ExtendedUserDto user);

        OperationResult CreateLot(LotDto lot, ExtendedUserDto creator, List<int> categoriesId);
    }
}
