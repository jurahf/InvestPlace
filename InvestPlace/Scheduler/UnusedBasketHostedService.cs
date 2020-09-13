using InvestPlaceDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InvestPlace.Scheduler
{
    public interface IUnusedBasketHostedService
    {
        Task DoWork();
    }

    public class UnusedBasketHostedService : IUnusedBasketHostedService
    {
        private readonly InvestPlaceContext db;
        private readonly ILogger<UnusedBasketHostedService> logger;

        public UnusedBasketHostedService(ILogger<UnusedBasketHostedService> logger, InvestPlaceContext db)
        {
            this.logger = logger;
            this.db = db;
        }

        public Task DoWork()
        {
            logger.LogInformation("UnusedBasketHostedService is working.");

            try
            {
                // найдем все забронированные и не купленные после этого паззлы
                List<Basket> basketForClear = db.Basket
                    .Include(x => x.Pazzle)
                    .ThenInclude(x => x.Lot)
                    .Where(x => x.LastOperationDate == null || x.LastOperationDate < DateTime.Now.AddHours(-12))
                    .Where(x => x.Pazzle.Any())
                    .ToList();

                while (basketForClear.Any())
                {
                    Pazzle puzzleForDel = null;
                    Basket basket = basketForClear.First();
                    do
                    {
                        puzzleForDel = basket.Pazzle.FirstOrDefault(); // все, которые в корзине - не куплены

                        if (puzzleForDel != null)
                        {
                            Lot lot = puzzleForDel.Lot;
                            basket.Pazzle.Remove(puzzleForDel);
                            lot.Pazzle.Remove(puzzleForDel);
                            db.Pazzle.Remove(puzzleForDel);     // может быть только этого достаточно
                        }
                    } while (puzzleForDel != null);

                    basket.LastOperationDate = DateTime.Now;
                    db.SaveChanges();

                    basketForClear.Remove(basket);
                }

                logger.LogInformation("UnusedBasketHostedService end working.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UnusedBasketHostedService");
            }

            return Task.CompletedTask;
        }
    }
}
