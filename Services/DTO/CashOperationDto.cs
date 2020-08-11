using InvestPlaceDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTO
{
    public class CashOperationDto
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public decimal Summ { get; set; }

        public string Comment { get; set; }


        public static CashOperationDto ConvertFromCashOperation(CashOperation operation)
        {
            if (operation == null)
                return null;

            return new CashOperationDto()
            {
                Id = operation.Id,
                Date = operation.Date,
                Summ = operation.Summ,
                Comment = operation.Comment
            };
        }

    }
}
