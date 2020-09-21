using InvestPlaceDB;
using Microsoft.EntityFrameworkCore;
using Services.DTO;
using Services.Services.ExtendedUserService;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Services.Services.CashService
{
    public class CashService : ICashService
    {
        private object lockObject = new object();
        private readonly InvestPlaceContext db;
        private readonly IExtendedUserService userService;
        private readonly IConfiguration Configuration;

        public CashService(InvestPlaceContext db, IExtendedUserService userService, IConfiguration configuration)
        {
            this.db = db;
            this.userService = userService;
            this.Configuration = configuration;
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

        public void ChangeSumm(ExtendedUserDto userToChange, decimal summDelta)
        {
            if (userToChange == null)
                throw new ArgumentNullException("Пользователь не может быть пустым");

            if (summDelta == 0)
                return;
            
            ExtendedUser findedUser = db.Users
                .Include(x => x.Cash)
                .FirstOrDefault(x => x.Id == userToChange.Id);
            ExtendedUser findedModerator = userService.GetCurrentUser();

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
                        Comment = $"Операция выполнена модератором (id = {findedModerator.Id})",
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

            List<QueryForOperation> existedQueries = db.QueryForOperation
                .Include(x => x.CashQueryModerator)
                .Include(x => x.Cash)
                .ThenInclude(x => x.ExtendedUser)
                .Where(x => x.IsCashOutput)
                .Where(x => x.OperationModerate == null)
                .Where(x => x.CashId == findedUser.CashId)
                .ToList();

            decimal existedQueriesSumm = existedQueries.Sum(x => x.Summ);

            if (findedUser.Cash.Summ < summ + existedQueriesSumm)
                throw new ArgumentException("На кошельке пользователя недостаточно средств, с учетом уже созданных запросов на вывод средств");

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


        public int OutputCashRequestCount()
        {
            return db.QueryForOperation.Count(x => x.OperationModerate == null);
        }

        public List<QueryForOperationDto> QueriesForModerate()
        {
            return db.QueryForOperation
                .Include(x => x.CashQueryModerator)
                .Include(x => x.Cash)
                .ThenInclude(x => x.ExtendedUser)
                .Where(x => x.OperationModerate == null)
                .Select(x => QueryForOperationDto.ConvertFromQueryForOperation(x))
                .ToList();
        }

        public List<QueryForOperationDto> QueriesCashByUser(ExtendedUserDto user)
        {
            if (user == null)
                throw new ArgumentNullException("Пользователь не задан");

            return db.QueryForOperation
                .Include(x => x.CashQueryModerator)
                .Include(x => x.Cash)
                .ThenInclude(x => x.ExtendedUser)
                .Where(x => x.Cash.ExtendedUser.Any(u => u.Id == user.Id))
                .Select(x => QueryForOperationDto.ConvertFromQueryForOperation(x))
                .ToList();
        }

        public QueryForOperationDto GetQueryForOperationById(int id)
        {
            return QueryForOperationDto.ConvertFromQueryForOperation(
                db.QueryForOperation
                .Include(x => x.CashQueryModerator)
                .Include(x => x.Cash)
                .ThenInclude(x => x.ExtendedUser)
                .FirstOrDefault(x => x.Id == id));
        }


        public void Moderate(QueryForOperationDto query, bool solution)
        {
            if (query == null)
                throw new ArgumentNullException("Запрос не может быть пустым");

            QueryForOperation findedQuery = db.QueryForOperation
                .Include(x => x.CashQueryModerator)
                .Include(x => x.Cash)
                .ThenInclude(x => x.ExtendedUser)
                .FirstOrDefault(x => x.Id == query.Id);

            ExtendedUser findedModerator = userService.GetCurrentUser();

            if (findedQuery == null)
                throw new ArgumentException("Запрос на операцию не найден");

            if (findedQuery.Summ <= 0 || findedQuery.Summ > 2_000_000)
                throw new ArgumentException("Сумма вывода денег должна быть больше 0 и меньше 2 000 000");

            if (findedModerator == null)
                throw new ArgumentException("Модератор не найден");

            List<string> roles = userService.GetRoles(findedModerator);
            if (!roles.Contains(ExtendedRole.MODERATOR) && !roles.Contains(ExtendedRole.ADMIN))
                throw new ArgumentException("Нет прав для выполнения данной операции");

            lock (lockObject)
            {
                if (findedQuery.IsCashOutput && (findedQuery.Cash == null || findedQuery.Cash.Summ < findedQuery.Summ))
                    throw new ArgumentException("На кошельке недостаточно средств");

                using (var transaction = db.Database.BeginTransaction())
                {
                    findedQuery.OperationModerate = solution;
                    findedQuery.CashQueryModerator = findedModerator;

                    db.Update(findedQuery);

                    string comment = $"В операции отказано (id запроса = {findedQuery.Id})";
                    if (solution)
                    {
                        if (findedQuery.IsCashOutput)
                        {
                            findedQuery.Cash.Summ -= findedQuery.Summ;
                            comment = $"Вывод денег (id запроса = {findedQuery.Id})";
                        }
                        else
                        {
                            findedQuery.Cash.Summ += findedQuery.Summ;
                            comment = $"Пополнение кошелька (id запроса = {findedQuery.Id})";
                        }
                    }

                    CashOperation cashOperation = new CashOperation()
                    {
                        Date = DateTime.Now,
                        Cash = findedQuery.Cash,
                        Summ = findedQuery.IsCashOutput ? -findedQuery.Summ : findedQuery.Summ,
                        Comment = comment 
                    };
                    db.CashOperation.Add(cashOperation);

                    db.SaveChanges();
                    transaction.Commit();
                }
            }
        }


        public SendMoneyToUserParams GetPhoneForInputMoney(ExtendedUserDto user, decimal summ)
        {
            if (user == null)
                throw new ArgumentNullException("Пользователь не задан");

            ExtendedUser findedUser = db.Users.FirstOrDefault(x => x.Id == user.Id);
            if (findedUser == null)
                throw new ArgumentException("Пользователь не найден в базе");

            lock (lockObject)
            {
                List<QueryForOperation> goodQueries = db.QueryForOperation
                    .Include(x => x.Cash)
                    .ThenInclude(x => x.ExtendedUser)
                    .Where(x => x.OperationModerate == null)
                    .Where(x => x.Status == CashQueryStatus.None)
                    .Where(x => x.IsCashOutput)
                    .Where(x => x.Summ == summ)
                    .Where(x => !string.IsNullOrEmpty(x.Cash.ExtendedUser.First().PhoneNumber))
                    .ToList();

                if (goodQueries.Any())
                {
                    var query = goodQueries.OrderBy(x => x.Date).First();
                    query.CashQueryClientProcessor = findedUser;        // в заявку на вывод денег подставляем обработчиком чувака, который хочет пополнить счет
                    query.Status = CashQueryStatus.ProcessByUser;
                    db.SaveChanges();
                    return new SendMoneyToUserParams()
                    {
                        Phone = query.Cash.ExtendedUser.First().PhoneNumber,
                        UserId = query.Cash.ExtendedUser.First().Id
                    };
                }
                else
                {
                    return new SendMoneyToUserParams()
                    {
                        Phone = Configuration.GetValue<string>("contacts:phone"),
                        UserId = null
                    };
                }
            }
        }


        public void ConfirmSendMoney(ExtendedUserDto fromUser, int? toUserId, string phone, decimal summ)
        {
            if (fromUser == null)
                throw new ArgumentNullException("Пользователь не задан");

            ExtendedUser findedUser = db.Users.FirstOrDefault(x => x.Id == fromUser.Id);
            if (findedUser == null)
                throw new ArgumentException("Пользователь не найден в базе");

            lock (lockObject)
            {
                if (toUserId.HasValue)
                {
                    QueryForOperation relatedQuery = db.QueryForOperation
                        .Include(x => x.CashQueryClientProcessor)
                        .Include(x => x.Cash)
                        .ThenInclude(x => x.ExtendedUser)
                        .Where(x => x.OperationModerate == null)
                        .Where(x => x.Status == CashQueryStatus.ProcessByUser)
                        .Where(x => x.IsCashOutput)
                        .Where(x => x.Summ == summ)
                        .Where(x => x.Cash.ExtendedUser.First().Id == toUserId)     // кому перевести - владелец кошелька и автор запроса на ВЫВОД денег (у него спишутся деньги с кошелька и придут в реальности)
                        .Where(x => x.CashQueryClientProcessor.Id == fromUser.Id)   // от кого перевести - ему надо запрос на пополнение кошелька, а свои деньги в реальности он отправит (уже подтвердил, что отправил)
                        .FirstOrDefault();

                    if (relatedQuery == null)
                        throw new ArgumentException("Не найдена связанная заявка. Обратитесь к администароту.");

                    // создадим запрос на пополнение кошелька
                    relatedQuery.Status = CashQueryStatus.SendConfirm;
                    QueryForOperation query = new QueryForOperation()
                    {
                        Status = CashQueryStatus.ProcessByUser,
                        Cash = findedUser.Cash,
                        Date = DateTime.Now,
                        IsCashOutput = false,
                        CashQueryClientProcessor = relatedQuery.Cash.ExtendedUser.First(),
                        Summ = summ
                    };
                    db.Add(query);
                    db.SaveChanges();
                }
                else
                {
                    // перевел сайту по номеру телефона
                    // создаем запрос на пополнение кошелька, и ждем, когда обработает модератор
                    QueryForOperation query = new QueryForOperation()
                    {
                        Status = CashQueryStatus.None,
                        Cash = findedUser.Cash,
                        Date = DateTime.Now,
                        IsCashOutput = false,
                        CashQueryClientProcessor = null,
                        Summ = summ,
                    };
                    db.Add(query);
                    db.SaveChanges();
                }
            }
        }


        public void ConfirmGetMoney(ExtendedUserDto user, QueryForOperationDto queryForOutputDto)
        {
            // пользователь говорит: деньги получил, вывод с кошелька состоялся

            if (user == null)
                throw new ArgumentNullException("Пользователь не задан");

            ExtendedUser findedUser = db.Users.FirstOrDefault(x => x.Id == user.Id);
            if (findedUser == null)
                throw new ArgumentException("Пользователь не найден в базе");

            if (queryForOutputDto == null)
                throw new ArgumentNullException("Запрос не задан");

            lock (lockObject)
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    QueryForOperation queryForOutput = db.QueryForOperation
                        .Include(x => x.CashQueryClientProcessor)
                        .Include(x => x.Cash)
                        .ThenInclude(x => x.ExtendedUser)
                        .Where(x => x.IsCashOutput == true)
                        .Where(x => x.Id == queryForOutputDto.Id)
                        .FirstOrDefault();
                    if (queryForOutput == null)
                        throw new ArgumentException("Запрос не найден в базе");
                    if (queryForOutput.OperationModerate == true)
                        throw new ArgumentException("Операция уже была проведена ранее");

                    // заявка на вывод и получение денег подтверждается
                    // деньги вычитаются из кошелька
                    if (queryForOutput.Cash.Summ < queryForOutput.Summ)
                        throw new ArgumentException("На кошельке недостаточно средств");

                    queryForOutput.Status = CashQueryStatus.SendConfirm;
                    queryForOutput.OperationModerate = true;
                    queryForOutput.Cash.Summ -= queryForOutput.Summ;
                    CashOperation cashOutputOperation = new CashOperation()
                    {
                        Date = DateTime.Now,
                        Cash = queryForOutput.Cash,
                        Summ = -queryForOutput.Summ,
                        Comment = $"Вывод денег (id запроса = {queryForOutput.Id})"
                    };
                    db.CashOperation.Add(cashOutputOperation);

                    // связанная завяка на пополнение кошелька подтверждается
                    // деньги добавляются на кошелек
                    QueryForOperation queryForInput = db.QueryForOperation
                        .Include(x => x.CashQueryClientProcessor)
                        .Include(x => x.Cash)
                        .ThenInclude(x => x.ExtendedUser)
                        .Where(x => x.OperationModerate == null)
                        .Where(x => x.CashQueryClientProcessor.Id == queryForOutput.Cash.ExtendedUser.First().Id)
                        .Where(x => x.Status == CashQueryStatus.ProcessByUser)
                        .Where(x => x.Summ == queryForOutput.Summ)
                        .Where(x => x.IsCashOutput == false)
                        .Where(x => x.Cash.ExtendedUser.First().Id == queryForOutput.CashQueryClientProcessorId)
                        .FirstOrDefault();

                    if (queryForInput == null)
                        throw new ArgumentException("Не найдена связанная операция пополнения");

                    queryForInput.Status = CashQueryStatus.SendConfirm;
                    queryForInput.OperationModerate = true;
                    queryForInput.Cash.Summ += queryForInput.Summ;
                    CashOperation cashInputOperation = new CashOperation()
                    {
                        Date = DateTime.Now,
                        Cash = queryForInput.Cash,
                        Summ = queryForInput.Summ,
                        Comment = $"Пополнение кошелька (id запроса = {queryForInput.Id})"
                    };
                    db.CashOperation.Add(cashInputOperation);

                    db.SaveChanges();
                    tran.Commit();
                }
            }
        }


        public List<QueryForOperationDto> ProcessOutputsByUser(ExtendedUserDto user)
        {
            if (user == null)
                throw new ArgumentNullException("Пользователь не задан");

            return db.QueryForOperation
                .Include(x => x.CashQueryModerator)
                .Include(x => x.CashQueryClientProcessor)
                .Include(x => x.Cash)
                .ThenInclude(x => x.ExtendedUser)
                .Where(x => x.CashQueryClientProcessor.Id == user.Id)
                .Where(x => x.OperationModerate == null)
                .Where(x => x.Status == CashQueryStatus.ProcessByUser)
                .Where(x => x.IsCashOutput == true)
                .Select(x => QueryForOperationDto.ConvertFromQueryForOperation(x))
                .ToList();
        }


        public void DeclainProcessOutput(ExtendedUserDto user, QueryForOperationDto queryForOutputDto)
        {
            if (user == null)
                throw new ArgumentNullException("Пользователь не задан");

            ExtendedUser findedUser = db.Users.FirstOrDefault(x => x.Id == user.Id);
            if (findedUser == null)
                throw new ArgumentException("Пользователь не найден в базе");

            if (queryForOutputDto == null)
                throw new ArgumentNullException("Запрос не задан");

            lock (lockObject)
            {
                QueryForOperation queryForOutput = db.QueryForOperation
                    .Include(x => x.CashQueryClientProcessor)
                    .Include(x => x.Cash)
                    .ThenInclude(x => x.ExtendedUser)
                    .Where(x => x.IsCashOutput == true)
                    .Where(x => x.Id == queryForOutputDto.Id)
                    .Where(x => x.CashQueryClientProcessor.Id == findedUser.Id)
                    .FirstOrDefault();
                if (queryForOutput == null)
                    throw new ArgumentException("Запрос не найден в базе");

                queryForOutput.Status = CashQueryStatus.None;
                db.SaveChanges();
            }
        }



    }
}
