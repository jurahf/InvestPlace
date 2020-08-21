using InvestPlaceDB;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.ExtendedUserService
{
    public interface IExtendedUserService
    {
        ExtendedUserDto GetByEmail(string email);

        bool UpdateUser(ExtendedUserDto userDto);

        List<ExtendedUserDto> GetAll();

        List<string> GetRoles(ExtendedUser user);
    }
}
