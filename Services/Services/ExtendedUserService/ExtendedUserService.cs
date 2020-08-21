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
        private AuthenticationStateProvider authenticationStateProvider;
        private UserManager<ExtendedUser> userManager;

        public ExtendedUserService(
            InvestPlaceContext db,
            AuthenticationStateProvider authenticationStateProvider,
            UserManager<ExtendedUser> userManager)
        {
            this.db = db;
            this.authenticationStateProvider = authenticationStateProvider;
            this.userManager = userManager;
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


        public async Task<IList<string>> GetRoles(ExtendedUser user)
        {
            if (user == null)
                return new List<string>();

            IList<string> result = new List<string>();
            Task<IList<string>> task = userManager.GetRolesAsync(user);

            try
            {
                result = task.GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
            }

            return result;
        }

        private async Task<List<string>> GetRoleOfCurrentUser()
        {
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var u = authState.User;
            var claims = authState.User?.Claims;

            return claims.Where(x => x.Type == "role").Select(x => x.Value).ToList();
        }


        public List<ExtendedUserDto> GetAll()
        {
            var result = db.Users
                .Include(u => u.Cash)
                .ToList(); 

            return result.Select(x => ExtendedUserDto.ConvertByUser(x/*, GetRoles(x).Result*/)).ToList();
        }


    }
}
