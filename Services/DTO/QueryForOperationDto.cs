using InvestPlaceDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.DTO
{
    public class QueryForOperationDto
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public decimal Summ { get; set; }

        public bool? Moderate { get; set; }

        public bool IsOutput { get; set; }

        public string Status { get; set; }
        public int StatusInt { get; set; }

        public ExtendedUserDto Moderator { get; set; }

        public ExtendedUserDto User { get; set; }

        public static QueryForOperationDto ConvertFromQueryForOperation(QueryForOperation query)
        {
            if (query == null)
                return null;

            return new QueryForOperationDto()
            {
                Id = query.Id,
                Date = query.Date,
                Summ = query.Summ,
                Moderate = query.OperationModerate,
                IsOutput = query.IsCashOutput,
                Moderator = ExtendedUserDto.ConvertByUser(query.CashQueryModerator),
                User = ExtendedUserDto.ConvertByUser(query.Cash.ExtendedUser.First()),
                Status = StatusToString(query.Status),
                StatusInt = (int)query.Status,  // TODO
            };
        }

        private static string StatusToString(CashQueryStatus status)
        {
            switch (status)
            {
                case CashQueryStatus.None:
                    return "Обрабатывается модератором";
                case CashQueryStatus.ProcessByUser:
                    return "Обрабатывается пользователем";
                case CashQueryStatus.SendConfirm:
                    return "Отправка денег подтверждена";
                case CashQueryStatus.GetConfirm:
                    return "Получение денег подтверждено";
                default:
                    return $"{status}";
            }
        }

    }


}
