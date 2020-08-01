using InvestPlaceDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTO
{
    public class ExtendedUserDto
    {
        public string Email { get; set; }
        public string InnerName { get; set; }
        public UserRole Role { get; set; }
        public CashDto Cash { get; set; }

        public ExtendedUserDto()
        {
        }

        public static ExtendedUserDto ConvertByUser(ExtendedUser user/*, ExtendedRole role*/)
        {
            if (user == null)
                return null;

            return new ExtendedUserDto()
            {
                Email = user.Email,
                InnerName = user.InnerName,
                //this.Role = user.R
                Cash = CashDto.ConvertByCash(user.Cash)
            };
        }
    }

    public enum UserRole
    {
        Usual,
        Moderator,
        Admin
    }
}
