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

        void Buy(ExtendedUserDto user);
    }
}
