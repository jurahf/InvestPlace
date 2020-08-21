
using InvestPlaceDB;
using Microsoft.EntityFrameworkCore;
using Services.DTO;
using Services.Services.ExtendedUserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Services.Services.CashService
{
    public class CashService : ICashService
    {
        private object lockObject = new object();
        private readonly InvestPlaceContext db;
        private readonly IExtendedUserService userService;

        public CashService(InvestPlaceContext db, IExtendedUserService userService)
        {
            this.db = db;
            this.userService = userService;
        }

        public List<CashOperationDto> GetHistoryByUser(ExtendedUserDto user)
        {
            if (user == null)
                throw new ArgumentException("Пользователь не задан");

            Cash cash = db.Cash
                .Include(c => c.CashOperation)
                .FirstOrDefault(x => x.ExtendedUser.Single().Id == user.Id);

            if (cash == null)
                return new List<CashOperationDto>();
            else
                return cash.CashOperation.Select(x => CashOperationDto.ConvertFromCashOperation(x)).ToList();
        }

        public void ChangeSumm(ExtendedUserDto userToChange, ExtendedUserDto moderator, decimal summDelta)
        {
            if (userToChange == null)
                throw new ArgumentNullException("Пользователь не может быть пустым");
            if (moderator == null)
                throw new ArgumentNullException("Модератор не может быть пустым");

            if (summDelta == 0)
                return;

            ExtendedUser findedUser = db.Users
                .Include(x => x.Cash)
                .FirstOrDefault(x => x.Id == userToChange.Id);
            ExtendedUser findedModerator = db.Users.Find(moderator.Id);

            if (findedUser == null)
                throw new ArgumentException("Пользователь не найден");
            if (findedModerator == null)
                throw new ArgumentException("Модератор не найден");

            List<string> roles = userService.GetRoles(findedModerator);
            if (!roles.Contains(ExtendedRole.MODERATOR) && !roles.Contains(ExtendedRole.ADMIN))
                throw new ArgumentException("У пользователя нет прав на выполнение операции");


            lock (lockObject)   // TODO: проверить время жизни этого объекта
            {
                if (findedUser.Cash == null)
                {
                    Cash cash = new Cash()
                    {
                        Summ = 0,
                    };

                    findedUser.Cash = cash;

                    db.Cash.Add(cash);
                    db.SaveChanges();
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    findedUser.Cash.Summ += summDelta;
                    CashOperation operation = new CashOperation()
                    {
                        Cash = findedUser.Cash,
                        Date = DateTime.Now,
                        Summ = summDelta,
                        Comment = $"Операция выполнена модератором (id = {moderator.Id})",
                    };
                    db.CashOperation.Add(operation);
                    db.Update(findedUser.Cash);

                    db.SaveChanges();
                    transaction.Commit();
                }
            }
        }



        public void CreateOutputOperationRequest(ExtendedUserDto user, decimal summ)
        {
            if (user == null)
                throw new ArgumentNullException("Пользователь не может быть пустым");

            if (summ <= 0)
                throw new ArgumentException("Сумма для вывода должна быть больше ноля");

            ExtendedUser findedUser = db.Users
                .Include(x => x.Cash)
                .FirstOrDefault(x => x.Id == user.Id);

            if (findedUser == null)
                throw new ArgumentException("Пользователь не найден");

            if (findedUser.Cash == null || findedUser.Cash.Summ < summ)
                throw new ArgumentException("На кошельке пользователя недостаточно средств");

            // тут без транзакции и без блокировки - просто создаем заявку на вывод
            // а вот когда заявка будет подтверждена, тогда нужны будут проверки и транзакции

            QueryForOperation query = new QueryForOperation()
            {
                Summ = summ,
                IsCashOutput = true,
                Date = DateTime.Now,
                Cash = findedUser.Cash,
            };

            db.QueryForOperation.Add(query);
            db.SaveChanges();
        }


    }
}
