using Services.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services.CategoryService
{
    public interface ICategoryService
    {
        List<CategoryDto> GetAll();
    }
}
