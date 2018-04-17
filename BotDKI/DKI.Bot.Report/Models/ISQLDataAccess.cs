using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.DKI.Entities
{
    public interface ISQLDataAccess
    {
        bool InsertBulkData<T>(IEnumerable<T> data) where T : class;
        bool InsertData<T>(T data) where T : class;
        bool DeleteAllData<T>(string TableName) where T:class;
        bool DeleteData<T>(string TableName, long id) where T : class;
        bool DeleteDataBulk<T>(string TableName, IEnumerable<T> Ids) where T : class;
        List<T> GetAllData<T>() where T : class;
        T GetDataById<T>(long Id) where T : class;
        List<T> GetDataByIds<T>(params long[] Ids) where T : class;
        List<T> GetAllData<T>(int Limit) where T : class;
        List<T> GetDataByStartId<T>(int Limit, long StartId) where T : class;
        long GetSequence<T>();
    }
}
