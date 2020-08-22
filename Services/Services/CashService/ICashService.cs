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

        List<QueryForOperationDto> QueriesForModerate();

        QueryForOperationDto GetQueryForOperationById(int id);

        void Moderate(QueryForOperationDto query, ExtendedUserDto moderator, bool solution);
    }
}
