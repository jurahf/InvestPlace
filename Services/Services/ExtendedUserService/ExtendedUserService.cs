using InvestPlaceDB;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            ExtendedUser user = db.Users
                .Include(u => u.Cash)
                .SingleOrDefault(x => x.Email == email);

            List<string> roles = GetRoles(user);

            return ExtendedUserDto.ConvertByUser(user, roles);
        }

        public ExtendedUserDto GetById(int id)
        {
            ExtendedUser user = db.Users
                .Include(u => u.Cash)
                .FirstOrDefault(x => x.Id == id);

            List<string> roles = GetRoles(user);

            return ExtendedUserDto.ConvertByUser(user, roles);
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


        public List<string> GetRoles(ExtendedUser user)
        {
            if (user == null)
                return new List<string>();

            List<int> rolesIds = db.UserRoles
                .Where(x => x.UserId == user.Id)
                .Select(x => x.RoleId)
                .ToList();

            List<ExtendedRole> roles = db.Roles
                .Where(x => rolesIds.Contains(x.Id))
                .ToList();

            return roles.Select(x => x.Name).ToList();
        }

        //private async Task<List<string>> GetRoleOfCurrentUser()
        //{
        //    var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        //    var u = authState.User;
        //    var claims = authState.User?.Claims;

        //    return claims.Where(x => x.Type == "role").Select(x => x.Value).ToList();
        //}


        public List<ExtendedUserDto> GetAll()
        {
            var result = db.Users
                .Include(u => u.Cash)
                .ToList(); 

            return result.Select(x => ExtendedUserDto.ConvertByUser(x, GetRoles(x))).ToList();
        }


    }
}
