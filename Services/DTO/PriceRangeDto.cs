using InvestPlaceDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTO
{
    public class PriceRangeDto
    {
        public int Id { get; set; }

        public decimal Minimum { get; set; }

        public decimal Maximum { get; set; }

        public static PriceRangeDto ConvertFromPriceRange(PriceRange range)
        {
            PriceRangeDto result = new PriceRangeDto();
            if (range == null)
                return result;

            result.Id = range.Id;
            result.Minimum = range.Minimum;
            result.Maximum = range.Maximum;

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
