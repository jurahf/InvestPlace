using InvestPlaceDB;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.ComponentModel.DataAnnotations;

namespace Services.DTO
{
    public class LotDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Название обязательно для заполнения")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageLink { get; set; }

        [Required(ErrorMessage = "Адрес интернет-магазина обязателен для заполнения")]
        [Url(ErrorMessage = "Введите url-адрес")]
        public string SourceLink { get; set; }

        [Required(ErrorMessage = "Цена обязательна для заполнения")]
        [Range(1.0, 2_000_000.0, ErrorMessage = "Цена должна быть числом от 1 до 2 000 000")]
        public decimal Price { get; set; }

        public decimal PuzzlePrice { get { return Math.Round(Price / EpicSettings.PuzzleCostDelimeter, 2, MidpointRounding.ToEven); } }

        public int PuzzleCount { get; set; }

        public List<CategoryDto> Categories { get; set; }

        public PriceRangeDto PriceRange { get; set; }

        public LotDto()
        {
            Categories = new List<CategoryDto>();
        }

        public static LotDto ConvertFromLot(Lot lot)
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
            result.PuzzleCount = lot.Pazzle.Count(x => x.BuyDate != null);
            result.Categories = lot.LotCategory.Select(x => CategoryDto.ConvertFromCategory(x.Category)).ToList();
            result.PriceRange = PriceRangeDto.ConvertFromPriceRange(lot.PriceRange);

            return result;
        }


        public override bool Equals(object obj)
        {
            LotDto other = obj as LotDto;

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
