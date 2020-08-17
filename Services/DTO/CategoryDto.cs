using InvestPlaceDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTO
{
    public class CategoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public CategoryDto Parent { get; set; }

        public static CategoryDto ConvertFromCategory(Category category)
        {
            CategoryDto dto = new CategoryDto();

            if (category == null)
                return null;        // !

            dto.Id = category.Id;
            dto.Name = category.Name;
            dto.Parent = ConvertFromCategory(category.Parent);

            return dto;
        }
    }
}
