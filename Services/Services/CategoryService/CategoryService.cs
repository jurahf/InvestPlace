using InvestPlaceDB;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private InvestPlaceContext db;


        public CategoryService(InvestPlaceContext db)
        {
            this.db = db;
        }

        public List<CategoryDto> GetAll()
        {
            return db.Category.Select(x => CategoryDto.ConvertFromCategory(x)).ToList();
        }
    }
}
