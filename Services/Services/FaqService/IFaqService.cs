using Services.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services.FaqService
{
    public interface IFaqService
    {
        List<FaqDto> GetAll();
    }
}
