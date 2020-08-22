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
        private readonly InvestPlaceContext db;
        private readonly AuthenticationStateProvider authProvider;

        public ExtendedUserService(InvestPlaceContext db, AuthenticationStateProvider authProvider)
        {
            this.db = db;
            this.authProvider = authProvider;
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

        public ExtendedUser GetCurrentUser()
        {
            return GetCurrentUserAsync().Result;
        }

        private async Task<ExtendedUser> GetCurrentUserAsync()
        {
            ExtendedUser result = null;
            var authState = await authProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity.IsAuthenticated)
            {
                result = db.Users
                    .Include(u => u.Cash)
                    .SingleOrDefault(x => x.Email == user.Identity.Name);
            }

            return result;
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

        public List<ExtendedUserDto> GetAll()
        {
            var result = db.Users
                .Include(u => u.Cash)
                .ToList(); 

            return result.Select(x => ExtendedUserDto.ConvertByUser(x, GetRoles(x))).ToList();
        }


    }
}
