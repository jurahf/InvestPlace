using InvestPlaceDB;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services.ExtendedUserService
{
    public interface IExtendedUserService
    {
        ExtendedUserDto GetByEmail(string email);

        bool UpdateUser(ExtendedUserDto userDto);
    }
}
