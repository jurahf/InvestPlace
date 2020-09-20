using InvestPlaceDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services;
using Services.Services.LotService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvestPlace.Scheduler
{
    public interface IOverfloweredLotsHostedService
    {
        Task DoWork();
    }

    /// <summary>
    /// Костыль на случай если в каком-то лоте смогли купить больше 300 пазлов
    /// </summary>
    public class OverfloweredLotsHostedService : IOverfloweredLotsHostedService
    {
        private readonly InvestPlaceContext db;
        private readonly ILotService lotService;
        private readonly ILogger<UnusedBasketHostedService> logger;


        public OverfloweredLotsHostedService(
            ILogger<UnusedBasketHostedService> logger, 
            InvestPlaceContext db,
            ILotService lotService)
        {
            this.logger = logger;
            this.db = db;
            this.lotService = lotService;
        }

        public Task DoWork()
        {
            logger.LogInformation("OverfloweredLotsHostedService is working.");

            try
            {
                List<Lot> lotsToComplete = db.Lot
                    .Include(x => x.Pazzle)
                    .ThenInclude(x => x.Buyer)
                    .ThenInclude(x => x.Cash)
                    .Where(x => x.CompleteDate == null)
                    .Where(x => x.Pazzle.Count >= EpicSettings.PuzzlePerLot)
                    .Where(x => x.Pazzle.All(y => y.BuyDate != null))
                    .ToList();

                foreach (var lot in lotsToComplete)
                {
                    // лишние купленные пазлы надо вернуть
                    foreach (var puzzle in lot.Pazzle.OrderBy(x => x.BuyDate).Skip(EpicSettings.PuzzlePerLot))
                    {
                        decimal summ = (lot.Price ?? 0) / EpicSettings.PuzzleCostDelimeter;
                        ExtendedUser user = puzzle.Buyer;
                        user.Cash.Summ += summ;

                        CashOperation operation = new CashOperation()
                        {
                            Cash = user.Cash,
                            Comment = "Возврат пазлов, кто-то опередил с покупкой в последний момент",
                            Summ = -summ,
                            Date = DateTime.Now
                        };

                        db.Add(operation);
                        db.Remove(puzzle); // а это можно в foreach?
                    }

                    // лот собран
                    lotService.CompleteLot(lot);
                }

                db.SaveChanges();

                logger.LogInformation("OverfloweredLotsHostedService end working.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "OverfloweredLotsHostedService");
            }

            return Task.CompletedTask;
        }
    }
}
