using InvestPlaceDB;
using Microsoft.EntityFrameworkCore;
using Services.DTO;
using Services.Services.ExtendedUserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Services.Services.QueryExchangeService
{
    public class QueryExchangeService : IQueryExchangeService
    {
        private readonly InvestPlaceContext db;
        private readonly IExtendedUserService userService;

        public QueryExchangeService(InvestPlaceContext db, IExtendedUserService userService)
        {
            this.db = db;
            this.userService = userService;
        }

        public int GetActiveQueryCount()
        {
            return db.QueryForExchange
                .Where(x => x.Moderate == null)
                .Count();
        }

        public List<QueryForExchangeDto> QueriesForModerate()
        {
            return db.QueryForExchange
                .Include(x => x.Lot)
                .ThenInclude(x => x.Seller)
                .Include(x => x.Pazzle)
                .ThenInclude(x => x.Buyer)
                .Include(x => x.Pazzle)
                .ThenInclude(x => x.Lot)
                .Where(x => x.Moderate == null)
                .Select(x => QueryForExchangeDto.ConvertFromQueryForExchange(x))
                .ToList();
        }

        public QueryForExchangeDto GetById(int id)
        {
            return QueryForExchangeDto.ConvertFromQueryForExchange(
                db.QueryForExchange
                .Include(x => x.Pazzle)
                .ThenInclude(x => x.Buyer)
                .Include(x => x.Pazzle)
                .ThenInclude(x => x.Lot)
                .FirstOrDefault(x => x.Id == id)
                );
        }


        public void Moderate(QueryForExchangeDto query, bool solution)
        {
            if (query == null)
                throw new ArgumentNullException("Запрос не может быть пустым");

            QueryForExchange findedQuery = db.QueryForExchange
                .Include(x => x.Lot)
                .ThenInclude(x => x.Seller)
                .Include(x => x.Pazzle)
                .ThenInclude(x => x.Buyer)
                .Include(x => x.Pazzle)
                .ThenInclude(x => x.Lot)
                .FirstOrDefault(x => x.Id == query.Id);


            if (findedQuery == null)
                throw new ArgumentException("Запрос на операцию не найден");

            ExtendedUser findedModerator = userService.GetCurrentUser();
            if (findedModerator == null)
                throw new ArgumentException("Модератор не найден");

            List<string> roles = userService.GetRoles(findedModerator);
            if (!roles.Contains(ExtendedRole.MODERATOR) && !roles.Contains(ExtendedRole.ADMIN))
                throw new ArgumentException("Нет прав для выполнения данной операции");

            if (findedQuery.Pazzle != null)
            {
                Lot lot = findedQuery.Pazzle.Lot;

                if (lot.CompleteNumber == null)
                    throw new ArgumentException("Покупка товара еще не завершена");


                findedQuery.Moderate = solution;
                findedQuery.ModerateDate = DateTime.Now;
                findedQuery.ExchangeModerator = findedModerator;
                findedQuery.Pazzle.Lot.ExchangeByBuyer = (solution == true);
            }
            else if (findedQuery.Lot != null)
            {
                Lot lot = findedQuery.Lot;

                if (lot.CompleteNumber == null)
                    throw new ArgumentException("Покупка товара еще не завершена");

                findedQuery.Moderate = solution;
                findedQuery.ModerateDate = DateTime.Now;
                findedQuery.ExchangeModerator = findedModerator;
                findedQuery.Lot.ExchangeBySeller = (solution == true);
            }

            db.SaveChanges();
        }


    }
}
