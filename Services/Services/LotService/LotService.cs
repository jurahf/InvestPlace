using InvestPlaceDB;
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
        /// Все промодерированные и незавершенные
        /// </summary>
        public List<LotDto> GetActual(bool actual)  // используется в NewLot для показа лотов для участия
        {
            return db.Lot
                .Include(x => x.PriceRange)
                .Include(x => x.Pazzle)
                .Include(x => x.Seller)
                .Include(x => x.CreateModerator)
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
                .Select(x => LotDto.ConvertFromLot(x))
                .ToList();
        }


        public OperationResult CreateLot(LotDto lot, ExtendedUserDto creator, List<int> categoriesId)
        {
            try
            {
                if (lot == null)
                    return OperationResult.CreateFail("Не задан лот");
                if (creator == null)
                    return OperationResult.CreateFail("Не задан пользователь");

                ExtendedUser user = db.Find<ExtendedUser>(creator.Id);

                if (user == null)
                    return OperationResult.CreateFail("Пользователь не нейден в базе");

                // подбираем диапазон цены (это цена пазла)
                PriceRange priceRange = db.PriceRange.FirstOrDefault(x => x.Minimum <= lot.PuzzlePrice && lot.PuzzlePrice <= x.Maximum);
                if (priceRange == null)
                    return OperationResult.CreateFail("Не найден диапазон цены для товара");


                // проверяем, а купил ли он другие лоты
                int anotherPuzzleCount = user.PazzleBuyer.Count(x =>
                    x.BuyDate >= DateTime.Now.AddDays(-1)
                    && x.Lot.PriceRangeId == priceRange.Id); // сейчас не проверяем, это 3 пазла одного товара или разных

                // а есть ли вообще пазлы, которые он мог бы купить
                int allAvaliablePuzzle = db.Lot.Count(x =>  // а здесь наоборот смотрим на лоты и считаем их, а не их оставшиеся пазлы
                    x.PriceRangeId == priceRange.Id
                    && x.CompleteDate == null
                );

                // ВНИМАНИЕ! Если поменяем условия по проверке пазлов, то надо и поменять сообщение об ошибке ниже:

                if (anotherPuzzleCount < EpicSettings.AnotherPuzzlesForNewLot && allAvaliablePuzzle >= EpicSettings.AnotherPuzzlesForNewLot)
                    return OperationResult.CreateFail($"Для размещения товара необходимо приобрести как минимум {EpicSettings.AnotherPuzzlesForNewLot} пазла за последние сутки");


                // TODO: проверяем, а оплатил ли он рекламный сбор

                // проверить, нет ли лота с тем же названием
                if (db.Lot.Any(x => x.Name.ToLower() == lot.Name.ToLower()))
                    return OperationResult.CreateFail("Товар с таким названием уже существует");


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

                db.Lot.Add(toSave);
                db.SaveChanges();

                return OperationResult.CreateSuccess();
            }
            catch (Exception ex)
            {
                return OperationResult.CreateFail($"Непредвиденная ошибка: {ex.Message}");
            }
        }


        public List<LotDto> GetBuyerField()
        {
            var allCompleted = db.Lot
                .Include(x => x.PriceRange)
                .Include(x => x.Pazzle)
                .Where(x => x.CompleteDate != null && x.CompleteDate.Value.Date < DateTime.Today); // завершенные на вчерашний день

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


    }
}
