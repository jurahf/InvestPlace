using Services.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services.QueryExchangeService
{
    public interface IQueryExchangeService
    {
        int GetActiveQueryCount();

        List<QueryForExchangeDto> QueriesForModerate();

        QueryForExchangeDto GetById(int id);

        void Moderate(QueryForExchangeDto query, bool solution);
    }
}
