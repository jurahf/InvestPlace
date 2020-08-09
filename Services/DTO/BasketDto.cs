using InvestPlaceDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.DTO
{
    public class BasketDto
    {
        public int Id { get; set; }

        public int ExtendedUserId { get; set; }

        public DateTime LastOperationDate { get; set; }

        public List<PuzzleDto> Puzzles { get; set; }

        public BasketDto()
        {
            Puzzles = new List<PuzzleDto>();
        }

        public static BasketDto ConvertFromBasket(Basket basket)
        {
            BasketDto dto = new BasketDto();

            if (basket == null)
                return dto;

            dto.Id = basket.Id;
            dto.ExtendedUserId = basket.ExtendedUser.Single().Id;
            dto.LastOperationDate = basket.LastOperationDate ?? DateTime.Now;
            dto.Puzzles = basket.Pazzle.Select(x => PuzzleDto.ConvertFromPuzzle(x)).ToList();

            return dto;
        }

    }
}
