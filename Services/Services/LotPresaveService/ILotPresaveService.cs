using Services.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services.LotPresaveService
{
    public interface ILotPresaveService
    {
        LotDto GetPresaveLot(ExtendedUserDto user);

        void SavePresaveLot(ExtendedUserDto user, LotDto lot);

        void ClearPresaveLot(ExtendedUserDto user);
    }


}
