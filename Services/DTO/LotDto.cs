using InvestPlaceDB;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;

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

        public LotDto()
        {
        }

        public LotDto(Lot lot)
        {
            if (lot == null)
                return; // а не return null?

            this.Id = lot.Id;
            this.Name = lot.Name;
            this.Description = lot.Description;
            this.SourceLink = lot.SourceLink;
            this.ImageLink = lot.ImageLink;
            this.Price = lot.Price ?? 0;
            this.PuzzleCount = lot.Pazzle.Count();
        }
    }
}
