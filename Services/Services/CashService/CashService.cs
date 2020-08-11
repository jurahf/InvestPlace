
using InvestPlaceDB;
using Microsoft.EntityFrameworkCore;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.Services.CashService
{
    public class CashService : ICashService
    {
        private readonly InvestPlaceContext db;

        public CashService(InvestPlaceContext db)
        {
            this.db = db;
        }

        public List<CashOperationDto> GetHistoryByUser(ExtendedUserDto user)
        {
            if (user == null)
                throw new ArgumentException("Пользователь не задан");

            Cash cash = db.Cash
                .Include(c => c.CashOperation)
                .FirstOrDefault(x => x.ExtendedUser.Single().Id == user.Id);

            if (cash == null)
                return new List<CashOperationDto>();
            else
                return cash.CashOperation.Select(x => CashOperationDto.ConvertFromCashOperation(x)).ToList();
        }
    }
}
