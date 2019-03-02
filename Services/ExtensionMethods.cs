using Microsoft.Data.Sqlite;
using System;

namespace UniversalKeepTheRhythm.Services
{
    
        public static class ExtensionMethods
        {
            public static SqliteParameter AddWithNullable<T>(this SqliteParameterCollection parms,
                    string parameterName, T? nullable) where T : struct
            {
                if (nullable.HasValue)
                    return parms.AddWithValue(parameterName, nullable.Value);
                else
                    return parms.AddWithValue(parameterName, DBNull.Value);
            }

        public static SqliteParameter AddWithNullable(this SqliteParameterCollection parms,
         string parameterName, object nullable) 
        {
            if (nullable != null)
                return parms.AddWithValue(parameterName, nullable);
            else
                return parms.AddWithValue(parameterName, DBNull.Value);
        }

        public static double? GetDoubleWithNull(this SqliteDataReader parms,
                int index)
        {
            if (parms.IsDBNull(index))
                return null;
            else
                return parms.GetDouble(index);
        }
        public static double GetDouble(this SqliteDataReader parms,
                int index, double de)
        {
            if (parms.IsDBNull(index))
                return de;
            else
                return parms.GetDouble(index);
        }
        public static int? GetIntWithNull(this SqliteDataReader parms,
                int index)
        {
            if (parms.IsDBNull(index))
                return null;
            else
                return parms.GetInt32(index);
        }
        public static int GetInt(this SqliteDataReader parms,
                int index, int de)
        {
            if (parms.IsDBNull(index))
                return de;
            else
                return parms.GetInt32(index);
        }
        public static long? GetLongWithNull(this SqliteDataReader parms,
                int index)
        {
            if (parms.IsDBNull(index))
                return null;
            else
                return parms.GetInt64(index);
        }
        public static long GetLong(this SqliteDataReader parms,
                int index, long de)
        {
            if (parms.IsDBNull(index))
                return de;
            else
                return parms.GetInt64(index);
        }
        public static string GetString(this SqliteDataReader parms,
                int index, string de)
        {
            if (parms.IsDBNull(index))
                return de;
            else
                return parms.GetString(index);
        }
    }


}
