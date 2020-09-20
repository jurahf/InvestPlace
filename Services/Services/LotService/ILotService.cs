using InvestPlaceDB;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.LotService
{
    public interface ILotService
    {
        //Task<List<LotDto>> GetAllAsync();

        List<LotDto> GetActual(bool actual);

        LotDto GetById(int id);

        List<LotDto> GetByUserSell(ExtendedUserDto user);

        List<LotDto> GetByUserBuy(ExtendedUserDto user);

        OperationResult CreateLot(LotDto lot, ExtendedUserDto creator, List<int> categoriesId);

        List<LotDto> GetBuyerField();

        int GetNextBuyerFieldNumber();

        int LotForModerateCount();

        List<LotDto> LotsForModerate();

        void CreateModerate(LotDto lot, bool solution);


        List<LotDto> SelledLots(ExtendedUserDto dto);

        List<LotDto> BuyedLots(ExtendedUserDto dto);


        void ExchangeBySeller(ExtendedUserDto user, LotDto lot);

        void ExchangeByBuyerOnMoney(ExtendedUserDto user, LotDto lot);

        void ExchangeByBuyerOnReal(ExtendedUserDto user, LotDto lot);


        void CompleteLot(Lot lot);
    }
}
