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

        LotDto GetById(int id);
    }
}
