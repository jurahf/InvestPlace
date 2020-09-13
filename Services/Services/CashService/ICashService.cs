using InvestPlaceDB;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services.CashService
{
    public interface ICashService
    {
        List<CashOperationDto> GetHistoryByUser(ExtendedUserDto user);

        void ChangeSumm(ExtendedUserDto userToChange, decimal summDelta);

        void CreateOutputOperationRequest(ExtendedUserDto user, decimal summ);

        int OutputCashRequestCount();

        List<QueryForOperationDto> QueriesCashByUser(ExtendedUserDto user);

        List<QueryForOperationDto> QueriesForModerate();

        QueryForOperationDto GetQueryForOperationById(int id);

        void Moderate(QueryForOperationDto query, bool solution);


        /// <summary>
        /// Выдать номер телефона пользователя для человека, который хочет пополнить свой кошелёк
        /// </summary>
        SendMoneyToUserParams GetPhoneForInputMoney(ExtendedUserDto user, decimal summ);

        /// <summary>
        /// Пользователь заявляет, что он отправил деньги другому пользователю
        /// </summary>
        void ConfirmSendMoney(ExtendedUserDto fromUser, int? toUserId, string phone, decimal summ);

        /// <summary>
        /// Пользователь заявляет, что он получил деньги от другого пользователя
        /// </summary>
        void ConfirmGetMoney(ExtendedUserDto user, QueryForOperationDto queryForOutputDto);

        /// <summary>
        /// Пользователь заявляет, что отказывается переводить деньги по операции вывода денег другого пользователя (своего пополнения кошелька)
        /// </summary>
        void DeclainProcessOutput(ExtendedUserDto user, QueryForOperationDto queryForOperationDto);

        /// <summary>
        /// Запросы на вывод денег от других пользователей, чей телефон был показан текущему пользователю
        /// </summary>
        List<QueryForOperationDto> ProcessOutputsByUser(ExtendedUserDto user);
    }
}
