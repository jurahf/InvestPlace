using InvestPlaceDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTO
{
    public class QueryForExchangeDto
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public DateTime? ModerateDate { get; set; }

        public bool Moderate { get; set; }

        public ExchangeOnRealType ExchangeType { get; set; }

        public PuzzleDto Puzzle { get; set; }

        public LotDto Lot { get; set; }

        public ExtendedUserDto Moderator { get; set; }

        public static QueryForExchangeDto ConvertFromQueryForExchange(QueryForExchange query)
        {
            if (query == null)
                return null;

            return new QueryForExchangeDto()
            {
                Id = query.Id,
                Date = query.Date,
                Moderate = query.Moderate == true,
                ModerateDate = query.ModerateDate,
                Moderator = ExtendedUserDto.ConvertByUser(query.ExchangeModerator),
                Puzzle = PuzzleDto.ConvertFromPuzzle(query.Pazzle),
                Lot = LotDto.ConvertFromLot(query.Lot),
                ExchangeType = query.Pazzle == null ? ExchangeOnRealType.SellerLot : ExchangeOnRealType.BuyerPuzzle
            };
        }
    }


    public enum ExchangeOnRealType
    {
        BuyerPuzzle,
        SellerLot
    }

}
