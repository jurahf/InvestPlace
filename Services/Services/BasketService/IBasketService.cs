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

        BasketDto RemoveFromBasket(ExtendedUserDto user, LotDto lot);

        void Buy(ExtendedUserDto user);
    }
}
