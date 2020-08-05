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
                db.Lot.Select(x => LotDto.ConvertFromLot(x, db)).ToList());
        }

        public LotDto GetById(int id)
        {
            return LotDto.ConvertFromLot(db.Lot.FirstOrDefault(x => x.Id == id), db);
        }

        public List<LotDto> GetByUser(ExtendedUserDto user)
        {
            return db.Lot.Where(x => x.SellerId == user.Id).Select(x => LotDto.ConvertFromLot(x, db)).ToList();
        }


    }
}
