using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using InvestPlaceDB;

namespace Services.DTO
{
    public class CashDto
    {
        public decimal Summ { get; set; }
        //public List<CashOperationDto>

        public static CashDto ConvertByCash(Cash cash)
        {
            if (cash == null)
                return null;

            return new CashDto()
            {
                Summ = Math.Round(cash.Summ, 2, MidpointRounding.ToEven)
            };
        }
    }
}
