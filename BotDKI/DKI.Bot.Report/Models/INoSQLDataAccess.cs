using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.DKI.Entities
{
    public interface INoSQLDataAccess
    {
        bool InsertBulkData<T>(IEnumerable<T> data);
        bool InsertData<T>(T data);
        bool DeleteAllData<T>();
        bool DeleteData<T>(long id);
        bool DeleteDataBulk<T>(IEnumerable<T> Ids);
        List<T> GetAllData<T>();
        T GetDataById<T>(long Id);
        List<T> GetDataByIds<T>(params long[] Ids);
        List<T> GetAllData<T>(int Limit);
        List<T> GetDataByStartId<T>(int Limit, long StartId);
        long GetSequence<T>();
    }
}
