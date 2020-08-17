using InvestPlaceDB;
using Microsoft.EntityFrameworkCore;
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
            return ExtendedUserDto.ConvertByUser(
                db.Users
                .Include(u => u.Cash)
                .SingleOrDefault(x => x.Email == email));
        }

        public bool UpdateUser(ExtendedUserDto dto)
        {
            try
            {
                ExtendedUser user = db.Users.Find(dto.Id);

                user.InnerName = dto.InnerName;
                user.Surname = dto.Surname;
                user.Patronymic = dto.Patronymic;
                user.PhoneNumber = dto.PhoneNumber;

                user.Email = dto.Email;
                //user.UserName = dto.Email;  // blazor хочет входить по email. еще надо Normalizaed name и email
                user.Address = dto.Address;

                user.SchetNumber = dto.SchetNumber;
                user.CorrSchet = dto.CorrSchet;
                user.Bank = dto.Bank;
                user.INN = dto.INN;
                user.BIK = dto.BIK;
                user.KPP = dto.KPP;

                db.Update(user);
                db.SaveChanges();

                return true;
            }
            catch
            {
                // TODO: Log
                return false;
            }
        }



    }
}
