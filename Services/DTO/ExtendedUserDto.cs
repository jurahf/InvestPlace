using InvestPlaceDB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Services.DTO
{
    public class ExtendedUserDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Имя обязательно для заполнения")]
        public string InnerName { get; set; }

        public string Surname { get; set; }

        public string Patronymic { get; set; }

        [Required(ErrorMessage = "Email-адрес обязателен для заполнения")]
        [EmailAddress(ErrorMessage = "Неверный email-адрес")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Неверный номер телефона")]
        [RegularExpression(@"^\+79\d{9}$", ErrorMessage = "Номер телефона должен иметь формат +7 9XX XXX XX XX")]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string Bank { get; set; }

        public string INN { get; set; }

        public string KPP { get; set; }

        public string BIK { get; set; }

        public string SchetNumber { get; set; }

        public string CorrSchet { get; set; }

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
                Id = user.Id,
                //this.Role = user.R
                InnerName = user.InnerName,
                Surname = user.Surname,
                Patronymic = user.Patronymic,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                
                SchetNumber = user.SchetNumber,
                CorrSchet = user.CorrSchet,
                Bank = user.Bank,
                INN = user.INN,
                BIK = user.BIK,
                KPP = user.KPP,                

                Cash = CashDto.ConvertByCash(user.Cash),
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
