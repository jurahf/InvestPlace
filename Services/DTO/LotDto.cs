using InvestPlaceDB;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace Services.DTO
{
    public class LotDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageLink { get; set; }
        public string SourceLink { get; set; }
        public decimal Price { get; set; }
        public decimal PuzzlePrice { get { return Math.Round(Price / EpicSettings.PuzzlePerLot, 2, MidpointRounding.ToEven); } }
        public int PuzzleCount { get; set; }

        public PriceRangeDto PriceRange { get; set; }

        public LotDto()
        {
        }

        public static LotDto ConvertFromLot(Lot lot, InvestPlaceContext db)
        {
            LotDto result = new LotDto();

            if (lot == null)
                return result; // а не return null?

            result.Id = lot.Id;
            result.Name = lot.Name;
            result.Description = lot.Description;
            result.SourceLink = lot.SourceLink;
            result.ImageLink = lot.ImageLink;
            result.Price = lot.Price ?? 0;
            result.PuzzleCount = lot.Pazzle.Count();

            // TODO: разобраться и удалить этот костыль
            if (lot.PriceRange == null && lot.PriceRangeId != null)
            {
                lot.PriceRange = db.PriceRange.Find(lot.PriceRangeId);
                db.Update(lot);
                //db.SaveChanges();
            }

            result.PriceRange = PriceRangeDto.ConvertFromPriceRange(lot.PriceRange);

            return result;
        }


        public override bool Equals(object obj)
        {
            PriceRangeDto other = obj as PriceRangeDto;

            if (other == null)
                return false;

            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

    }
}
