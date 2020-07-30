using InvestPlaceDB;
using Microsoft.EntityFrameworkCore;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.LotService
{
    public class LotService : ILotService
    {
        private InvestPlaceContext db;

        public LotService(InvestPlaceContext db)
        {
            this.db = db;
        }

        public Task<List<LotDto>> GetAllAsync()
        {
            return Task.FromResult(
                db.Lot.Select(x => new LotDto(x)).ToList());
        }

        public LotDto GetById(int id)
        {
            return new LotDto(db.Lot.FirstOrDefault(x => x.Id == id));
        }
    }
}
