using InvestPlaceDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Services.DTO;
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
        private InvestPlaceContext db;

        public LotService(InvestPlaceContext db)
        {
            this.db = db;
        }

        public Task<List<LotDto>> GetAllAsync()
        {
            return Task.FromResult(
                db.Lot
                .Include(x => x.PriceRange)
                .Include(x => x.Pazzle)
                .Select(x => LotDto.ConvertFromLot(x))
                .ToList());
        }

        public List<LotDto> GetActual(bool actual)
        {
            return db.Lot
                .Include(x => x.PriceRange)
                .Include(x => x.Pazzle)
                .Where(x => (x.CompleteDate == null) == actual)
                .Select(x => LotDto.ConvertFromLot(x))
                .ToList();
        }

        public LotDto GetById(int id)
        {
            return LotDto.ConvertFromLot(db.Lot
                .Include(x => x.PriceRange)
                .Include(x => x.Pazzle)
                .FirstOrDefault(x => x.Id == id));
        }

        public List<LotDto> GetByUserSell(ExtendedUserDto user)
        {
            return db.Lot
                .Include(x => x.PriceRange)
                .Include(x => x.Pazzle)
                .Where(x => x.SellerId == user.Id)
                .Select(x => LotDto.ConvertFromLot(x))
                .ToList();
        }

        public List<LotDto> GetByUserBuy(ExtendedUserDto user)
        {
            return db.Lot
                .Include(x => x.PriceRange)
                .Include(x => x.Pazzle)
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


    }
}
