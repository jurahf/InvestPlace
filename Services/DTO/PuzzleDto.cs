using InvestPlaceDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTO
{
    public class PuzzleDto
    {
        public int Id { get; set; }

        public int LotId { get; set; }

        public LotDto Lot { get; set; }

        public static PuzzleDto ConvertFromPuzzle(Pazzle puzzle)
        {
            PuzzleDto dto = new PuzzleDto();

            if (puzzle == null)
                return dto;

            dto = new PuzzleDto()
            {
                Id = puzzle.Id,
                LotId = puzzle.LotId ?? 0,
                Lot = LotDto.ConvertFromLot(puzzle.Lot)
            };

            return dto;
        }
    }
}
