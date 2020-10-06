using InvestPlaceDB;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Services.DTO;
using Services.Services.ExtendedUserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.LotService
{
    public class LotService : ILotService
    {
        private static object lockObject = new object();
        private readonly InvestPlaceContext db;
        private readonly IExtendedUserService userService;

        public LotService(InvestPlaceContext db, IExtendedUserService userService)
        {
            this.db = db;
            this.userService = userService;
        }

        public Task<List<LotDto>> GetAllAsync()
        {
            return Task.FromResult(
                db.Lot
                .Include(x => x.Seller)
                .Include(x => x.CreateModerator)
                .Include(x => x.PriceRange)
                .Include(x => x.Pazzle)
                .Select(x => LotDto.ConvertFromLot(x))
                .ToList());
        }


        /// <summary>
        /// Все лоты, проданные пользователем (продажа завершена)
        /// </summary>
        public List<LotDto> SelledLots(ExtendedUserDto user)
        {
            return db.Lot
                .Include(x => x.Seller)
                .Include(x => x.CreateModerator)
                .Include(x => x.PriceRange)
                .Include(x => x.Pazzle)
                .Where(x => x.SellerId == user.Id)
                .Where(x => x.CompleteNumber != null)
                .Select(x => LotDto.ConvertFromLot(x))
                .ToList();
        }

        /// <summary>
        /// Лоты, в которых пользователь оказался победителем
        /// </summary>
        public List<LotDto> BuyedLots(ExtendedUserDto user)
        {
            List<Pazzle> pazzles = db.Pazzle
                .Include(x => x.Lot)
                .Include(x => x.Buyer)
                .Where(x => x.Winner == true)
                .Where(x => x.BuyerId == user.Id)
                .ToList();

            List<int> lotIds = pazzles.Select(x => x.Lot.Id).ToList();

            return db.Lot
                .Include(x => x.Seller)
                .Include(x => x.CreateModerator)
                .Include(x => x.PriceRange)
                .Include(x => x.Pazzle)
                .Where(x => lotIds.Contains(x.Id))
                .Select(x => LotDto.ConvertFromLot(x))
                .ToList();
        }

        /// <summary>
        /// Все промодерированные и незавершенные
        /// </summary>
        public List<LotDto> GetActual(bool actual)  // используется в NewLot для показа лотов для участия
        {
            return db.Lot
                .Include(x => x.PriceRange)
                .Include(x => x.Pazzle)
                .Include(x => x.Seller)
                .Include(x => x.CreateModerator)
                .Include(x => x.LotCategory)
                .ThenInclude(x => x.Category)
                .ThenInclude(x => x.Parent)
                .Where(x => x.CreateModerate == actual)
                .Where(x => (x.CompleteDate == null) == actual)
                .Select(x => LotDto.ConvertFromLot(x))
                .ToList();
        }

        public LotDto GetById(int id)
        {
            return LotDto.ConvertFromLot(db.Lot
                .Include(x => x.PriceRange)
                .Include(x => x.Pazzle)
                .Include(x => x.Seller)
                .Include(x => x.CreateModerator)
                .Include(x => x.LotCategory)
                .ThenInclude(x => x.Category)
                .ThenInclude(x => x.Parent)
                .FirstOrDefault(x => x.Id == id));
        }

        public List<LotDto> GetByUserSell(ExtendedUserDto user)
        {
            return db.Lot
                .Include(x => x.PriceRange)
                .Include(x => x.Pazzle)
                .Include(x => x.Seller)
                .Include(x => x.CreateModerator)
                .Where(x => x.SellerId == user.Id)
                .Where(x => x.CompleteDate == null)
                .Select(x => LotDto.ConvertFromLot(x))
                .ToList();
        }

        public List<LotDto> GetByUserBuy(ExtendedUserDto user)
        {
            return db.Lot
                .Include(x => x.PriceRange)
                .Include(x => x.Pazzle)
                .Include(x => x.Seller)
                .Include(x => x.CreateModerator)
                .Where(x => x.Pazzle.Any(p => p.BuyerId == user.Id))
                .Where(x => x.CreateModerate == true)
                .Where(x => x.CompleteDate == null)
                .Select(x => LotDto.ConvertFromLot(x))
                .ToList();
        }


        public OperationResult CreateLot(LotDto lot, ExtendedUserDto creator, List<int> categoriesId)
        {
            try
            {
                lock (lockObject)
                {
                    if (lot == null)
                        return OperationResult.CreateFail("Не задан лот");
                    if (creator == null)
                        return OperationResult.CreateFail("Не задан пользователь");

                    ExtendedUser user = db.Users
                        .Include(x => x.Cash)
                        .Include(x => x.LotSeller)
                        .ThenInclude(x => x.PriceRange)
                        .FirstOrDefault(x => x.Id == creator.Id);

                    if (user == null)
                        return OperationResult.CreateFail("Пользователь не нейден в базе");

                    // подбираем диапазон цены (это цена пазла)
                    PriceRange priceRange = db.PriceRange.FirstOrDefault(x => x.Minimum <= lot.PuzzlePrice && lot.PuzzlePrice <= x.Maximum);
                    if (priceRange == null)
                        return OperationResult.CreateFail("Не найден диапазон цены для товара");

                    // проверить, нет ли лота с тем же названием
                    if (db.Lot.Any(x => x.Name.ToLower() == lot.Name.ToLower()))
                        return OperationResult.CreateFail("Товар с таким названием уже существует");

                    // нельзя выставлять больше 1 лота в каждом диапазоне
                    if (user.LotSeller.Any(x => x.CompleteDate == null 
                        && x.CreateModerate != false // промодерирован или на модерации
                        && x.PriceRange.Id == priceRange.Id))
                    {
                        throw new Exception("Нельзя иметь больше 1 активного лота в ценовом диапазоне");
                    }

                    // хватает ли накопленной помощи
                    decimal helpingNeed = lot.Price * EpicSettings.HelpingSummForNewLotPercent / 100m;
                    if (user.Cash == null || user.Cash.HelpingSumm < helpingNeed)
                    {
                        // TODO: проверить, а есть ли вообще пазлы, которые можно купить
                        throw new Exception($"Для размещения лота необходимо оказать помощь в сборе пазлов еще на {helpingNeed - user.Cash.HelpingSumm} руб.");
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Lot toSave = new Lot()
                        {
                            Name = lot.Name,
                            Description = lot.Description,
                            ImageLink = lot.ImageLink,
                            CreateDate = DateTime.Now,
                            Seller = user,
                            Price = lot.Price,
                            SourceLink = lot.SourceLink,
                            PriceRange = priceRange,
                        };

                        // подставляем категорию из выбранной
                        foreach (int catId in categoriesId)
                        {
                            Category cat = db.Category.Find(catId);

                            if (cat != null)
                            {
                                LotCategory lc = new LotCategory()
                                {
                                    Lot = toSave,
                                    Category = cat,
                                };

                                db.LotCategory.Add(lc);
                            }
                        }

                        user.Cash.HelpingSumm -= helpingNeed;

                        db.Lot.Add(toSave);
                        db.SaveChanges();
                        transaction.Commit();

                        return OperationResult.CreateSuccess();
                    }
                }
            }
            catch (Exception ex)
            {
                return OperationResult.CreateFail($"Непредвиденная ошибка: {ex.Message}");
            }
        }


        public OperationResult UpdateLot(int id, LotDto lot, ExtendedUserDto editor, List<int> categoriesId)
        {
            try
            {
                lock (lockObject)
                {
                    if (lot == null)
                        return OperationResult.CreateFail("Не задан лот");
                    if (editor == null)
                        return OperationResult.CreateFail("Не задан пользователь");

                    ExtendedUser user = db.Users
                        .Include(x => x.Cash)
                        .FirstOrDefault(x => x.Id == editor.Id);

                    if (user == null)
                        return OperationResult.CreateFail("Пользователь не нейден в базе");

                    List<string> roles = userService.GetRoles(user);
                    if (!roles.Contains(ExtendedRole.MODERATOR) && !roles.Contains(ExtendedRole.ADMIN))   // TODO: лучше проверить через разрешения, а не роли
                    {
                        return OperationResult.CreateFail("У пользователя нет разрешения на данное действие");
                    }

                    Lot oldLot = db.Lot
                        .Include(x => x.Seller)
                        .Include(x => x.PriceRange)
                        .Include(x => x.LotCategory)
                        .FirstOrDefault(x => x.Id == lot.Id);

                    if (oldLot == null)
                    {
                        return OperationResult.CreateFail("Не найден лот для редактирования");
                    }

                    // подбираем диапазон цены (это цена пазла)
                    PriceRange priceRange = db.PriceRange.FirstOrDefault(x => x.Minimum <= lot.PuzzlePrice && lot.PuzzlePrice <= x.Maximum);
                    if (priceRange == null)
                        return OperationResult.CreateFail("Не найден диапазон цены для товара");

                    // проверить, нет ли лота с тем же названием
                    if (db.Lot.Any(x => x.Name.ToLower() == lot.Name.ToLower() && x.Id != oldLot.Id))
                        return OperationResult.CreateFail("Товар с таким названием уже существует");

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        oldLot.Name = lot.Name;
                        oldLot.Description = lot.Description;
                        oldLot.ImageLink = lot.ImageLink;
                        oldLot.Price = lot.Price;
                        oldLot.SourceLink = lot.SourceLink;
                        oldLot.PriceRange = priceRange;
                        oldLot.LotCategory.Clear();

                        // подставляем категорию из выбранной
                        foreach (int catId in categoriesId)
                        {
                            Category cat = db.Category.Find(catId);

                            if (cat != null)
                            {
                                LotCategory lc = new LotCategory()
                                {
                                    Lot = oldLot,
                                    Category = cat,
                                };

                                db.LotCategory.Add(lc);
                            }
                        }

                        db.Lot.Update(oldLot);
                        db.SaveChanges();
                        transaction.Commit();

                        return OperationResult.CreateSuccess();
                    }
                }
            }
            catch (Exception ex)
            {
                return OperationResult.CreateFail($"Непредвиденная ошибка: {ex.Message}");
            }
        }


        public List<LotDto> GetBuyerField()
        {
            // завершенные на вчерашний день
            var allCompleted = db.Lot
                .Include(x => x.PriceRange)
                .Include(x => x.Pazzle)
                .Where(x => x.CompleteDate != null)
                .ToList()       // следующее предложение не транслируется в SQL
                .Where(x => x.CompleteDate.Value.Date < DateTime.Today); 

            // каждые 300 - одно поле покупателей
            int mod = allCompleted.Count() % EpicSettings.LotPerBuyerField;

            if (mod == 0)
                return new List<LotDto>();
            else
            {
                return allCompleted.OrderByDescending(x => x.CompleteDate)
                    .Take(mod)
                    .Select(x => LotDto.ConvertFromLot(x))
                    .OrderBy(x => x.CompleteDate)
                    .ToList();
            }
        }


        public int GetNextBuyerFieldNumber()
        {
            var allCompleted = db.Lot
                .Include(x => x.PriceRange)
                .Include(x => x.Pazzle)
                .Where(x => x.CompleteDate != null);

            // каждые 300 - одно поле покупателей
            int mod = allCompleted.Count() % EpicSettings.LotPerBuyerField;

            return mod + 1;
        }
    



        public int LotForModerateCount()
        {
            return db.Lot
                .Count(x => x.CreateModerate == null);
        }


        public List<LotDto> LotsForModerate()
        {
            return db.Lot
                .Include(x => x.PriceRange)
                .Include(x => x.Seller)
                .Include(x => x.CreateModerator)
                .Where(x => x.CreateModerate == null)
                .Select(x => LotDto.ConvertFromLot(x))
                .ToList();
        }


        public void CreateModerate(LotDto lot, bool solution)
        {
            if (lot == null)
                throw new ArgumentException("Товар не может быть пустым");

            Lot findedLot = db.Lot.Find(lot.Id);
            ExtendedUser findedModerator = userService.GetCurrentUser();

            if (findedLot == null)
                throw new ArgumentException("Товар не найден");
            if (findedModerator == null)
                throw new ArgumentException("Модератор не найден");

            List<string> roles = userService.GetRoles(findedModerator);
            if (!roles.Contains(ExtendedRole.MODERATOR) && !roles.Contains(ExtendedRole.ADMIN))   // TODO: лучше проверить через разрешения, а не роли
            {
                throw new ArgumentException("У пользователя нет разрешения на данное действие");
            }

            findedLot.CreateModerate = solution;
            findedLot.CreateModerator = findedModerator;
            findedLot.CreateModerateDate = DateTime.Now;

            db.Update(findedLot);
            db.SaveChanges();
        }


        /// <summary>
        /// Обмен проданного товара на деньги по текущему уровню обмена
        /// </summary>
        public void ExchangeBySellerOnMoney(ExtendedUserDto user, LotDto lot)
        {
            if (lot == null)
                throw new ArgumentNullException("Товар должен быть указан");

            if (user == null)
                throw new ArgumentNullException("Пользователь должен быть указан");

            ExtendedUser dbUser = db.Users
                .Include(x => x.Cash)
                .FirstOrDefault(x => x.Id == user.Id);

            Lot dbLot = db.Lot
                .Include(x => x.Seller)
                .FirstOrDefault(x => x.Id == lot.Id);

            if (dbUser == null)
                throw new ArgumentException("Пользователь не найден");

            if (dbLot == null)
                throw new ArgumentException("Товар не найден");

            if (dbLot.CompleteNumber == null)
                throw new ArgumentException("Покупка товара еще не завершена");

            if (dbLot.Seller.Id != dbUser.Id)
                throw new ArgumentException("Пользователь не является продавцом товара");

            lock (lockObject)
            {
                if (dbLot.ExchangeBySeller)
                    throw new ArgumentException("Товар уже обменян продавцом");

                using (var transaction = db.Database.BeginTransaction())
                {
                    dbLot.ExchangeBySeller = true;
                    if (dbUser.Cash == null)
                    {
                        Cash cash = new Cash()
                        {
                            Summ = 0,
                        };
                        dbUser.Cash = cash;
                        db.Cash.Add(cash);
                    }

                    decimal summ = (dbLot.Price ?? 0m) * EpicSettings.ExchangeLevelSeller;

                    CashOperation operation = new CashOperation()
                    {
                        Cash = dbUser.Cash,
                        Summ = summ,
                        Date = DateTime.Now,
                        Comment = $"Обмен проданного товара \"{dbLot.Name}\" (id = {dbLot.Id}) по уровню обмена {EpicSettings.ExchangeLevelSeller * 100}%"
                    };

                    db.CashOperation.Add(operation);
                    dbUser.Cash.Summ += summ;

                    db.Update(dbLot);
                    db.Update(dbUser);

                    db.SaveChanges();
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Обмен проданного товара на реальный товар
        /// </summary>
        public void ExchangeBySellerOnReal(ExtendedUserDto user, LotDto lot)
        {
            if (lot == null)
                throw new ArgumentNullException("Товар должен быть указан");

            if (user == null)
                throw new ArgumentNullException("Пользователь должен быть указан");


            lock (lockObject)
            {
                ExtendedUser dbUser = db.Users
                .Include(x => x.Cash)
                .FirstOrDefault(x => x.Id == user.Id);

                Lot dbLot = db.Lot
                    .Include(x => x.Pazzle)
                    .ThenInclude(x => x.QueryForExchange)
                    .FirstOrDefault(x => x.Id == lot.Id);

                if (dbUser == null)
                    throw new ArgumentException("Пользователь не найден");

                if (dbLot == null)
                    throw new ArgumentException("Товар не найден");

                if (dbLot.CompleteNumber == null)
                    throw new ArgumentException("Покупка товара еще не завершена");


                if (dbLot.SellerId != dbUser.Id)
                    throw new ArgumentException("Пользователь не является продавцом товара");

                if (dbLot.QueryForExchange.Any(x => x.ModerateDate == null))
                    throw new ArgumentException("Уже создана заявка на обмен этого товара");

                if (dbLot.ExchangeBySeller)
                    throw new ArgumentException("Товар уже обменян продавцом");

                using (var transaction = db.Database.BeginTransaction())
                {
                    QueryForExchange query = new QueryForExchange()
                    {
                        Lot = dbLot,
                        Date = DateTime.Now,
                    };

                    db.QueryForExchange.Add(query);

                    db.SaveChanges();
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Обмен купленного товара на деньги
        /// </summary>
        public void ExchangeByBuyerOnMoney(ExtendedUserDto user, LotDto lot)
        {
            if (lot == null)
                throw new ArgumentNullException("Товар должен быть указан");

            if (user == null)
                throw new ArgumentNullException("Пользователь должен быть указан");

            ExtendedUser dbUser = db.Users
                .Include(x => x.Cash)
                .FirstOrDefault(x => x.Id == user.Id);

            Lot dbLot = db.Lot
                .Include(x => x.Pazzle)
                .ThenInclude(x => x.QueryForExchange)
                .FirstOrDefault(x => x.Id == lot.Id);

            if (dbUser == null)
                throw new ArgumentException("Пользователь не найден");

            if (dbLot == null)
                throw new ArgumentException("Товар не найден");

            if (dbLot.CompleteNumber == null)
                throw new ArgumentException("Покупка товара еще не завершена");

            Pazzle pazzle = dbLot.Pazzle.FirstOrDefault(x => x.Winner == true);
            if (pazzle == null)
                throw new ArgumentException("Покупатель товара еще не определен");

            if (pazzle.BuyerId != dbUser.Id)
                throw new ArgumentException("Пользователь не является покупателем товара");

            if (pazzle.QueryForExchange.Any(x => x.ModerateDate == null))
                throw new ArgumentException("Уже создана заявка на обмен этого товара");

            lock (lockObject)
            {
                if (dbLot.ExchangeByBuyer)
                    throw new ArgumentException("Товар уже обменян покупателем");

                using (var transaction = db.Database.BeginTransaction())
                {
                    dbLot.ExchangeByBuyer = true;
                    if (dbUser.Cash == null)
                    {
                        Cash cash = new Cash()
                        {
                            Summ = 0,
                        };
                        dbUser.Cash = cash;
                        db.Cash.Add(cash);
                    }

                    decimal summ = (dbLot.Price ?? 0m) * EpicSettings.ExchangeLevelBuyer;

                    CashOperation operation = new CashOperation()
                    {
                        Cash = dbUser.Cash,
                        Summ = summ,
                        Date = DateTime.Now,
                        Comment = $"Обмен купленного товара \"{dbLot.Name}\" (id = {dbLot.Id}) на деньги ({EpicSettings.ExchangeLevelBuyer * 100}%)"
                    };

                    db.CashOperation.Add(operation);
                    dbUser.Cash.Summ += summ;

                    db.Update(dbLot);
                    db.Update(dbUser);

                    db.SaveChanges();
                    transaction.Commit();
                }
            }
        }


        /// <summary>
        /// Обмен купленного товара на реальный товар
        /// </summary>
        public void ExchangeByBuyerOnReal(ExtendedUserDto user, LotDto lot)
        {
            if (lot == null)
                throw new ArgumentNullException("Товар должен быть указан");

            if (user == null)
                throw new ArgumentNullException("Пользователь должен быть указан");


            lock (lockObject)
            {
                ExtendedUser dbUser = db.Users
                .Include(x => x.Cash)
                .FirstOrDefault(x => x.Id == user.Id);

                Lot dbLot = db.Lot
                    .Include(x => x.Pazzle)
                    .ThenInclude(x => x.QueryForExchange)
                    .FirstOrDefault(x => x.Id == lot.Id);

                if (dbUser == null)
                    throw new ArgumentException("Пользователь не найден");

                if (dbLot == null)
                    throw new ArgumentException("Товар не найден");

                if (dbLot.CompleteNumber == null)
                    throw new ArgumentException("Покупка товара еще не завершена");

                Pazzle pazzle = dbLot.Pazzle.FirstOrDefault(x => x.Winner == true);
                if (pazzle == null)
                    throw new ArgumentException("Покупатель товара еще не определен");

                if (pazzle.BuyerId != dbUser.Id)
                    throw new ArgumentException("Пользователь не является покупателем товара");

                if (pazzle.QueryForExchange.Any(x => x.ModerateDate == null))
                    throw new ArgumentException("Уже создана заявка на обмен этого товара");

                if (dbLot.ExchangeByBuyer)
                    throw new ArgumentException("Товар уже обменян покупателем");

                using (var transaction = db.Database.BeginTransaction())
                {
                    QueryForExchange query = new QueryForExchange()
                    {
                        Pazzle = pazzle,
                        Date = DateTime.Now,
                    };

                    db.QueryForExchange.Add(query);

                    db.SaveChanges();
                    transaction.Commit();
                }
            }
        }



        public void CompleteLot(Lot lot)
        {
            // лот полностью куплен, определяем победителя
            lot.CompleteDate = DateTime.Now;
            int completeNumber = GetNextBuyerFieldNumber();
            lot.CompleteNumber = completeNumber;

            Pazzle winnerPazzle = lot.Pazzle.OrderBy(x => x.BuyDate).ElementAt(completeNumber - 1);
            winnerPazzle.Winner = true;

            // всем начисляем скидки (кроме победителя и продавца)
            List<ExtendedUser> forBonus = lot.Pazzle.Select(x => x.Buyer)
                .Where(b => b.Id != winnerPazzle.BuyerId)
                .Where(x => x.Id != lot.SellerId)
                .ToList();

            decimal bonus = (lot.Price ?? 0m) * EpicSettings.BonusPercent / 100m;
            foreach (var bonusedUser in forBonus)
            {
                bonusedUser.Cash.BonusSumm += bonus;
            }
        }





    }
}
