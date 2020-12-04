using InvestPlaceDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Services.DTO;
using Services.Services.LotService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Services.Services.BasketService
{
    public class BasketService : IBasketService
    {
        private readonly InvestPlaceContext db;
        private readonly ILotService lotService;
        private static object lockObject = new object();

        public BasketService(InvestPlaceContext db, ILotService lotService)
        {
            this.db = db;
            this.lotService = lotService;
        }

        public BasketDto GetBasketByUser(ExtendedUserDto user)
        {
            if (user == null || user.Id <= 0)
                throw new ArgumentException("Пользователь задан неверно");

            Basket basket = db.Basket
                .Include(x => x.ExtendedUser)
                .Include(x => x.Pazzle)
                .ThenInclude(x => x.Lot)
                .FirstOrDefault(x => x.ExtendedUser.Single().Id == user.Id);

            return BasketDto.ConvertFromBasket(basket);
        }

        public BasketDto ChangeLotCount(ExtendedUserDto user, LotDto lot, int changeNumber)
        {
            try
            {
                // TODO: это расточительно, так как в методах каждый раз ищутся пользователь и лот в базе.
                // Плюс, блокировка только внутри каждого добавления, а не на все сразу
                BasketDto temp = null;
                if (changeNumber > 0)
                {
                    for (int i = 0; i < changeNumber; i++)
                        temp = AddToBasket(user, lot);
                }
                else
                {
                    for (int i = 0; i < -changeNumber; i++)
                        temp = RemoveFromBasket(user, lot, false);
                }

                return temp;
            }
            catch
            {
                throw;
            }
        }

        public BasketDto AddToBasket(ExtendedUserDto userDto, LotDto lotDto)
        {
            lock (lockObject)
            {
                ExtendedUser user = db.Find<ExtendedUser>(userDto.Id);
                if (user == null)
                    throw new ArgumentException("Пользователь не найден");

                Lot lot = db.Find<Lot>(lotDto.Id);
                if (lot == null)
                    throw new ArgumentException("Лот не найден");

                if (lot.SellerId == user.Id)
                    throw new ArgumentException("Нельзя покупать пазлы лота, который сам разместил");

                Basket basket = db.Basket.FirstOrDefault(x => x.ExtendedUser.Single().Id == userDto.Id);

                if (basket == null)
                {
                    basket = new Basket();
                    basket.ExtendedUser.Add(user);
                    //user.Basket = basket;

                    db.Basket.Add(basket);
                }


                if (lot.Pazzle.Count >= EpicSettings.PuzzlePerLot)
                    throw new OverflowException("Все пазлы данного лота уже куплены");

                Pazzle puzzle = new Pazzle();
                // X, Y
                bool exitLoop = false;
                for (int i = 0; i < EpicSettings.MaxPuzzleX; i++)
                {
                    if (exitLoop)
                        break;

                    for (int j = 0; j < EpicSettings.MaxPuzzleY; j++)
                    {
                        if (!lot.Pazzle.Any(x => x.X == i && x.Y == j))
                        {
                            puzzle.X = i;
                            puzzle.Y = j;
                            exitLoop = true;
                        }

                        if (exitLoop)
                            break;
                    }
                }

                lot.Pazzle.Add(puzzle);
                db.Pazzle.Add(puzzle);

                basket.Pazzle.Add(puzzle);

                basket.LastOperationDate = DateTime.Now;
                db.SaveChanges();

                return BasketDto.ConvertFromBasket(basket);
            }
        }


        public BasketDto RemoveFromBasket(ExtendedUserDto userDto, LotDto lotDto, bool allByLot)
        {
            lock (lockObject)
            {
                ExtendedUser user = db.Find<ExtendedUser>(userDto.Id);
                if (user == null)
                    throw new ArgumentException("Пользователь не найден");

                Lot lot = db.Find<Lot>(lotDto.Id);
                if (lot == null)
                    throw new ArgumentException("Лот не найден");

                Basket basket = db.Basket.FirstOrDefault(x => x.ExtendedUser.Single().Id == userDto.Id);

                if (basket == null)
                    throw new ArgumentException("У пользователя не найдена корзина");


                Pazzle puzzleForDel = null;
                do
                {
                    puzzleForDel = basket.Pazzle.FirstOrDefault(x => x.LotId == lot.Id/* && x.BuyDate == null*/); // все, которые в корзине - не куплены

                    if (puzzleForDel != null)
                    {
                        basket.Pazzle.Remove(puzzleForDel);
                        lot.Pazzle.Remove(puzzleForDel);
                        db.Pazzle.Remove(puzzleForDel);     // может быть только этого достаточно
                    }
                } while (allByLot && puzzleForDel != null);

                basket.LastOperationDate = DateTime.Now;
                db.SaveChanges();

                return BasketDto.ConvertFromBasket(basket);
            }
        }


        /// <summary>
        /// Все что было в корзине становится купленным
        /// </summary>
        public void Buy(ExtendedUserDto userDto)
        {
            lock (lockObject)
            {
                ExtendedUser user = db.Users
                .Include(x => x.Cash)
                .Include(x => x.Basket)
                .ThenInclude(y => y.Pazzle)
                .ThenInclude(z => z.Lot)
                .ThenInclude(й => й.Pazzle)
                .ThenInclude(q => q.Buyer)
                .ThenInclude(q => q.Cash)
                .FirstOrDefault(x => x.Id == userDto.Id);

                if (user == null)
                    throw new ArgumentException("Пользователь не найден");
                if (user.Basket == null)
                    throw new ArgumentException("У пользователя нет корзины");
                if (user.Cash == null)
                    throw new ArgumentException("У пользователя нет кошелька");

                using (var transaction = db.Database.BeginTransaction())
                {
                    decimal summ = user.Basket.Pazzle.Sum(x => (x.Lot.Price ?? 0) / EpicSettings.PuzzleCostDelimeter);
                    if (summ > user.Cash.Summ)
                        throw new Exception("Недостаточно средств");

                    foreach (var pz in user.Basket.Pazzle)
                    {
                        if (pz.Lot.CreateModerate != true)
                        {
                            throw new Exception($"Размещение товара \"{pz.Lot.Name}\" не подтверждено модератором");
                        }

                        if (pz.Lot.CompleteDate != null)
                        {
                            throw new Exception($"Продажа товара \"{pz.Lot.Name}\" уже завершена");
                        }
                    }

                    CashOperation operation = new CashOperation()
                    {
                        Cash = user.Cash,
                        Comment = "Покупка пазлов",
                        Summ = -summ,
                        Date = DateTime.Now
                    };

                    db.CashOperation.Add(operation);
                    user.Cash.Summ -= summ;
                    user.Cash.HelpingSumm += summ;

                    foreach (var puzzle in user.Basket.Pazzle)
                    {
                        puzzle.Buyer = user;
                        puzzle.BuyDate = DateTime.Now;

                        if (puzzle.Lot.Pazzle.Count >= EpicSettings.PuzzlePerLot
                            && puzzle.Lot.Pazzle.All(x => x.BuyDate != null))
                        {
                            lotService.CompleteLot(puzzle.Lot);
                        }
                    }

                    user.Basket.Pazzle.Clear();
                    db.Basket.Remove(user.Basket);

                    db.SaveChanges();

                    transaction.Commit();
                }
            }
        }





    }



}
