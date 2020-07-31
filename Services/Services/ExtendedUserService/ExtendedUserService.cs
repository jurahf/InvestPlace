using InvestPlaceDB;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.Services.ExtendedUserService
{
    public class ExtendedUserService : IExtendedUserService
    {
        private InvestPlaceContext db;

        public ExtendedUserService(InvestPlaceContext db)
        {
            this.db = db;
        }

        public ExtendedUserDto GetByEmail(string email)
        {
            return ExtendedUserDto.ConvertByUser(db.Users.SingleOrDefault(x => x.Email == email));
        }
    }
}
