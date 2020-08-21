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

        void ChangeSumm(ExtendedUserDto userToChange, ExtendedUserDto moderator, decimal summDelta);
    }
}
