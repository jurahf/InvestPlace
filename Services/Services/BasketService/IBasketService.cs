using Services.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services.BasketService
{
    public interface IBasketService
    {
        BasketDto GetBasketByUser(ExtendedUserDto user);

        BasketDto AddToBasket(ExtendedUserDto user, LotDto lot);

        BasketDto RemoveFromBasket(ExtendedUserDto user, LotDto lot, bool removeAllPuzzleOfLot);

        BasketDto ChangeLotCount(ExtendedUserDto user, LotDto lot, int changeNumber);

        void Buy(ExtendedUserDto user);
    }
}
