using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using UniversalKeepTheRhythm.model;
using Windows.Devices.Geolocation;
using Windows.Storage;
//using System.Data.SqlClient;

namespace UniversalKeepTheRhythm.Services
{
    public class DummyResult
    {
        public string type { get; set; }
        public double? maxSpeed { get; set; }
        public long? maxDuration { get; set; }
        public double? maxDistance { get; set; }
        public double? maxPace { get; set; }
        public long? totalTime { get; set; }
        public double? totalDistance { get; set; }
        public int totalNumber { get; set; }
    }
    public class DummyResult2
    {
        public string day { get; set; }
        public double? maxDistance { get; set; }
        public double? totalDistance { get; set; }
        public int totalNumber { get; set; }
        public long? totalTime { get; set; }
    }

    public class DataBaseManager
    {
        private static readonly object SyncRoot = new object();

        private static DataBaseManager _instance = null;
        public static DataBaseManager instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                lock (SyncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new DataBaseManager("ktr");
                    }
                }
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        public readonly System.Object lockPoint = new System.Object();
        private SqliteConnection db;
        private string fullpath;
        private string path;
        private IStorageFolder folder;
        public DataBaseManager(string name, IStorageFolder _folder = null)
        {

            path = name + ".db";

            if (_folder == null)
            {
                folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            }
            else
            {
                folder = _folder;
            }

            fullpath = Path.Combine(folder.Path, path);

            db = new SqliteConnection("FileName=" + fullpath);
            db.Open();
            create();
        }


        //void openOld()
        //{
        //    var localDatabaseConn = new SqlCeConnection("Data Source = " + name + ";");
        //    localDatabaseConn.Open();
        //    localDatabaseCmd = localDatabaseConn.CreateCommand();

        //}

        public void create()
        {

            var c = new SqliteCommand("create table if not exists Paths (id integer primary key, description varchar(50) not null, typePath char(1) not null)", db);
            c.ExecuteReader();

            c = new SqliteCommand("create table if not exists Properties (name varchar(250) primary key, value varchar(250))", db);
            c.ExecuteReader();

            c = new SqliteCommand("create table if not exists Sessions " +
            "(id integer primary key, " +
            "comment varchar(250) NOT NULL, " +
            "dayOfSession varchar(50) NOT NULL, " +

            "minLatitude double NULL, " +
            "minLongitude double NULL, " +
            "maxLatitude double NULL, " +
            "maxLongitude double NULL, " +

            "distance double NULL, " +
            "duration double NULL, " +

            "minSpeed double NULL, " +
            "maxSpeed double NULL, " +
            "avgSpeed double NULL, " +
            "minAltitude double NULL, " +
            "maxAltitude double NULL, " +
            "avgAltitude double NULL, " +

            "centerLatitude double NULL, " +
            "centerLongitude double NULL, " +

            "ascendent double NULL, " +
            "descendent double NULL, " +

            "avgPace double NULL, " +
            "startLongitude double NULL, " +
            "startLatitude double NULL, " +
            "endLongitude double NULL, " +

            "endLatitude double NULL, " +
            "objective double NULL, " +
            "objectiveDistance double NULL, " +
            "objectiveTime double NULL, " +

            "idPath integer NOT NULL, " +
            "FOREIGN KEY(idPath) REFERENCES Paths(id))", db);
            c.ExecuteReader();


            c = new SqliteCommand("create table if not exists Points " +
                "(id integer primary key, " +
                "longitude double NOT NULL, " +
                "latitude double NOT NULL, " +
                "speed double, " +
                "altitude double, " +
                "name varchar(500) NOT NULL, " +
                "type char(1) NOT NULL, " +
                "idPath integer, " +
                "FOREIGN KEY(idPath) REFERENCES Paths(id))", db);
            c.ExecuteReader();


            c = new SqliteCommand("create table if not exists PointInterest " +
                "(id integer primary key, " +
                "latitude double NOT NULL, " +
                "longitude double NOT NULL, " +
                "distance double NOT NULL, " +
                "time double NOT NULL, " +
                "rhythm double, " +
                "type char(1) NOT NULL, " +
                "message varchar(500) NOT NULL, " +
                "idSession integer, " +
                "FOREIGN KEY(idSession) REFERENCES Sessions(id))", db);
            c.ExecuteReader();


            c = new SqliteCommand("create table if not exists Mesures " +
            "( idSession integer, id integer," +
            "latitude double NOT NULL, " +
            "longitude double NOT NULL, " +
            "speed double, " +
            "altitude double, " +
            "action char(1), " +
            "PRIMARY KEY(idSession, id), " +
            "FOREIGN KEY(idSession) REFERENCES Sessions(id))", db);
            c.ExecuteReader();
        }
        public void Close()
        {
            db.Close();
            db = null;
        }

        public async Task<bool> DatabaseExistsAsync()
        {
            try
            {
                var file = await folder.GetFileAsync(path);
                var f = await file.OpenReadAsync();
                f.Dispose();

                var c = new SqliteCommand("SELECT count(1) FROM sqlite_master WHERE type='table'", db);
                SqliteDataReader query = c.ExecuteReader();

                var N = 0;
                while (query.Read())
                {
                    N = query.GetInt16(0);
                }
                return N == 6 ? true : false;
            }
            catch (System.IO.FileNotFoundException)
            {
                return false;
            }
        }

        public bool CheckDataBase()
        {
            try
            {
                var c = new SqliteCommand("SELECT count(1) FROM sqlite_master WHERE type='table'", db);
                SqliteDataReader query = c.ExecuteReader();

                var N = 0;
                while (query.Read())
                {
                    N = query.GetInt16(0);
                }
                return N == 6 ? true : false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteDatabaseAsync()
        {
            db.Dispose();

            //SQLite.SQLiteConnectionPool.ApplicationSuspended();

            var file = await folder.GetFileAsync(path);

            await file.DeleteAsync();

            return true;
        }

        internal void InsertSession(Sessions sessionData)
        {
            var c = new SqliteCommand("insert into Sessions (comment,dayOfSession, idPath,minLatitude,minLongitude,maxLatitude,maxLongitude,distance,duration,minSpeed,maxSpeed,avgSpeed," +
            "minAltitude,maxAltitude,avgAltitude,centerLatitude,centerLongitude,ascendent,descendent,avgPace,startLongitude,startLatitude,endLongitude,endLatitude,objective," +
            "objectiveDistance,objectiveTime) values (@comment,@dayOfSession, @idPath,@minLatitude,@minLongitude,@maxLatitude,@maxLongitude,@distance,@duration,@minSpeed,@maxSpeed,@avgSpeed," +
            "@minAltitude,@maxAltitude,@avgAltitude,@centerLatitude,@centerLongitude,@ascendent,@descendent,@avgPace,@startLongitude,@startLatitude,@endLongitude,@endLatitude,@objective," +
            "@objectiveDistance,@objectiveTime)", db);

            c.Parameters.AddWithValue("@comment", sessionData.Comment);
            c.Parameters.AddWithValue("@dayOfSession", sessionData.DayOfSession);
            c.Parameters.AddWithValue("@idPath", sessionData.IdPath);
            c.Parameters.AddWithNullable("@minLatitude", sessionData.MinLat);
            c.Parameters.AddWithNullable("@minLongitude", sessionData.MinLon);
            c.Parameters.AddWithNullable("@maxLatitude", sessionData.MaxLat);
            c.Parameters.AddWithNullable("@maxLongitude", sessionData.MaxLon);
            c.Parameters.AddWithNullable("@distance", sessionData.Distance);
            c.Parameters.AddWithNullable("@duration", sessionData.Duration);
            c.Parameters.AddWithNullable("@minSpeed", sessionData.MinSpeed);
            c.Parameters.AddWithNullable("@maxSpeed", sessionData.MaxSpeed);
            c.Parameters.AddWithNullable("@avgSpeed", sessionData.AvgSpeed);
            c.Parameters.AddWithNullable("@minAltitude", sessionData.MinAltitude);
            c.Parameters.AddWithNullable("@maxAltitude", sessionData.MaxAltitude);
            c.Parameters.AddWithNullable("@avgAltitude", sessionData.AvgAltitude);
            c.Parameters.AddWithNullable("@centerLatitude", sessionData.CenterLat);
            c.Parameters.AddWithNullable("@centerLongitude", sessionData.CenterLon);
            c.Parameters.AddWithNullable("@ascendent", sessionData.Ascendent);
            c.Parameters.AddWithNullable("@descendent", sessionData.Descendent);
            c.Parameters.AddWithNullable("@avgPace", sessionData.AvgPace);
            c.Parameters.AddWithNullable("@startLongitude", sessionData.StartLon);
            c.Parameters.AddWithNullable("@startLatitude", sessionData.StartLat);
            c.Parameters.AddWithNullable("@endLongitude", sessionData.EndLon);
            c.Parameters.AddWithNullable("@endLatitude", sessionData.EndLat);
            c.Parameters.AddWithNullable("@objective", sessionData.Objective);
            c.Parameters.AddWithNullable("@objectiveDistance", sessionData.ObjDistance);
            c.Parameters.AddWithNullable("@objectiveTime", sessionData.ObjTime);
            try
            {
                var r = c.ExecuteReader();

                sessionData.Id = LastInsertRowid();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        internal Sessions getLastSession()
        {
            var c = new SqliteCommand("select id as Id,comment as Comment,dayOfSession as DayOfSession, idPath as IdPath, minLatitude as MinLat, minLongitude as MinLon," +
                "maxLatitude,maxLongitude,distance,duration,minSpeed,maxSpeed,avgSpeed," +
            "minAltitude,maxAltitude,avgAltitude,centerLatitude,centerLongitude,ascendent,descendent,avgPace,startLongitude,startLatitude,endLongitude,endLatitude,objective," +
            "objectiveDistance,objectiveTime from Sessions order by dayOfSession desc", db);

            Sessions s = null;
            var query = c.ExecuteReader();
            while (query.Read())
            {
                s = new Sessions();
                s.Id = query.GetInt32(0);
                s.Comment = query.GetString(1);
                s.DayOfSession = query.GetString(2);
                s.IdPath = query.GetInt32(3);
                s.MinLat = query.GetDoubleWithNull(4);
                s.MinLon = query.GetDoubleWithNull(5);
                s.MaxLat = query.GetDoubleWithNull(6);
                s.MaxLon = query.GetDoubleWithNull(7);
                s.Distance = query.GetDoubleWithNull(8);
                s.Duration = query.GetLongWithNull(9);
                s.MinSpeed = query.GetDoubleWithNull(10);
                s.MaxSpeed = query.GetDoubleWithNull(11);
                s.AvgSpeed = query.GetDoubleWithNull(12);
                s.MinAltitude = query.GetDoubleWithNull(13);
                s.MaxAltitude = query.GetDoubleWithNull(14);
                s.AvgAltitude = query.GetDoubleWithNull(15);
                s.CenterLat = query.GetDoubleWithNull(16);
                s.CenterLon = query.GetDoubleWithNull(17);
                s.Ascendent = query.GetDoubleWithNull(18);
                s.Descendent = query.GetDoubleWithNull(19);
                s.AvgPace = query.GetDoubleWithNull(20);
                s.StartLon = query.GetDoubleWithNull(21);
                s.StartLat = query.GetDoubleWithNull(22);
                s.EndLon = query.GetDoubleWithNull(23);
                s.EndLat = query.GetDoubleWithNull(24);
                s.Objective = query.GetDoubleWithNull(25);
                s.ObjDistance = query.GetDoubleWithNull(26);
                s.ObjTime = query.GetLongWithNull(27);

                return s;
            }

            return s;
        }

        internal IEnumerable<Sessions> GetSessions(int idPath)
        {
            IList<Sessions> result = new List<Sessions>();

            var c = new SqliteCommand("select id as Id,comment as Comment,dayOfSession as DayOfSession, idPath as IdPath, minLatitude as MinLat, minLongitude as MinLon," +
    "maxLatitude,maxLongitude,distance,duration,minSpeed,maxSpeed,avgSpeed," +
"minAltitude,maxAltitude,avgAltitude,centerLatitude,centerLongitude,ascendent,descendent,avgPace,startLongitude,startLatitude,endLongitude,endLatitude,objective," +
"objectiveDistance,objectiveTime from Sessions where idPath=@idPath order by idPath,dayOfSession desc ", db);
            c.Parameters.AddWithValue("@idPath", idPath);

            Sessions s = null;
            var query = c.ExecuteReader();
            while (query.Read())
            {
                s = new Sessions();
                s.Id = query.GetInt32(0);
                s.Comment = query.GetString(1);
                s.DayOfSession = query.GetString(2);
                s.IdPath = query.GetInt32(3);
                s.MinLat = query.GetDoubleWithNull(4);
                s.MinLon = query.GetDoubleWithNull(5);
                s.MaxLat = query.GetDoubleWithNull(6);
                s.MaxLon = query.GetDoubleWithNull(7);
                s.Distance = query.GetDoubleWithNull(8);
                s.Duration = query.GetLongWithNull(9);
                s.MinSpeed = query.GetDoubleWithNull(10);
                s.MaxSpeed = query.GetDoubleWithNull(11);
                s.AvgSpeed = query.GetDoubleWithNull(12);
                s.MinAltitude = query.GetDoubleWithNull(13);
                s.MaxAltitude = query.GetDoubleWithNull(14);
                s.AvgAltitude = query.GetDoubleWithNull(15);
                s.CenterLat = query.GetDoubleWithNull(16);
                s.CenterLon = query.GetDoubleWithNull(17);
                s.Ascendent = query.GetDoubleWithNull(18);
                s.Descendent = query.GetDoubleWithNull(19);
                s.AvgPace = query.GetDoubleWithNull(20);
                s.StartLon = query.GetDoubleWithNull(21);
                s.StartLat = query.GetDoubleWithNull(22);
                s.EndLon = query.GetDoubleWithNull(23);
                s.EndLat = query.GetDoubleWithNull(24);
                s.Objective = query.GetDoubleWithNull(25);
                s.ObjDistance = query.GetDoubleWithNull(26);
                s.ObjTime = query.GetLongWithNull(27);

                result.Add(s);
            }

            return result;
        }

        internal IEnumerable<Sessions> GetSessions()
        {
            IList<Sessions> result = new List<Sessions>();

            var c = new SqliteCommand("select id as Id,comment as Comment,dayOfSession as DayOfSession, idPath as IdPath, minLatitude as MinLat, minLongitude as MinLon," +
    "maxLatitude,maxLongitude,distance,duration,minSpeed,maxSpeed,avgSpeed," +
"minAltitude,maxAltitude,avgAltitude,centerLatitude,centerLongitude,ascendent,descendent,avgPace,startLongitude,startLatitude,endLongitude,endLatitude,objective," +
"objectiveDistance,objectiveTime from Sessions order by dayOfSession desc ", db);

            Sessions s = null;
            var query = c.ExecuteReader();
            while (query.Read())
            {
                s = new Sessions();
                s.Id = query.GetInt32(0);
                s.Comment = query.GetString(1);
                s.DayOfSession = query.GetString(2);
                s.IdPath = query.GetInt32(3);
                s.MinLat = query.GetDoubleWithNull(4);
                s.MinLon = query.GetDoubleWithNull(5);
                s.MaxLat = query.GetDoubleWithNull(6);
                s.MaxLon = query.GetDoubleWithNull(7);
                s.Distance = query.GetDoubleWithNull(8);
                s.Duration = query.GetLongWithNull(9);
                s.MinSpeed = query.GetDoubleWithNull(10);
                s.MaxSpeed = query.GetDoubleWithNull(11);
                s.AvgSpeed = query.GetDoubleWithNull(12);
                s.MinAltitude = query.GetDoubleWithNull(13);
                s.MaxAltitude = query.GetDoubleWithNull(14);
                s.AvgAltitude = query.GetDoubleWithNull(15);
                s.CenterLat = query.GetDoubleWithNull(16);
                s.CenterLon = query.GetDoubleWithNull(17);
                s.Ascendent = query.GetDoubleWithNull(18);
                s.Descendent = query.GetDoubleWithNull(19);
                s.AvgPace = query.GetDoubleWithNull(20);
                s.StartLon = query.GetDoubleWithNull(21);
                s.StartLat = query.GetDoubleWithNull(22);
                s.EndLon = query.GetDoubleWithNull(23);
                s.EndLat = query.GetDoubleWithNull(24);
                s.Objective = query.GetDoubleWithNull(25);
                s.ObjDistance = query.GetDoubleWithNull(26);
                s.ObjTime = query.GetLongWithNull(27);

                result.Add(s);
            }

            return result;
        }

        internal Sessions GetSession(int idSession)
        {
            var c = new SqliteCommand("select id as Id,comment as Comment,dayOfSession as DayOfSession, idPath as IdPath, minLatitude as MinLat, minLongitude as MinLon," +
                "maxLatitude,maxLongitude,distance,duration,minSpeed,maxSpeed,avgSpeed," +
            "minAltitude,maxAltitude,avgAltitude,centerLatitude,centerLongitude,ascendent,descendent,avgPace,startLongitude,startLatitude,endLongitude,endLatitude,objective," +
            "objectiveDistance,objectiveTime from Sessions where id =@id", db);
            c.Parameters.AddWithValue("@id", idSession);

            Sessions s = null;
            var query = c.ExecuteReader();
            while (query.Read())
            {
                s = new Sessions();
                s.Id = query.GetInt32(0);
                s.Comment = query.GetString(1);
                s.DayOfSession = query.GetString(2);
                s.IdPath = query.GetInt32(3);
                s.MinLat = query.GetDoubleWithNull(4);
                s.MinLon = query.GetDoubleWithNull(5);
                s.MaxLat = query.GetDoubleWithNull(6);
                s.MaxLon = query.GetDoubleWithNull(7);
                s.Distance = query.GetDoubleWithNull(8);
                s.Duration = query.GetLongWithNull(9);
                s.MinSpeed = query.GetDoubleWithNull(10);
                s.MaxSpeed = query.GetDoubleWithNull(11);
                s.AvgSpeed = query.GetDoubleWithNull(12);
                s.MinAltitude = query.GetDoubleWithNull(13);
                s.MaxAltitude = query.GetDoubleWithNull(14);
                s.AvgAltitude = query.GetDoubleWithNull(15);
                s.CenterLat = query.GetDoubleWithNull(16);
                s.CenterLon = query.GetDoubleWithNull(17);
                s.Ascendent = query.GetDoubleWithNull(18);
                s.Descendent = query.GetDoubleWithNull(19);
                s.AvgPace = query.GetDoubleWithNull(20);
                s.StartLon = query.GetDoubleWithNull(21);
                s.StartLat = query.GetDoubleWithNull(22);
                s.EndLon = query.GetDoubleWithNull(23);
                s.EndLat = query.GetDoubleWithNull(24);
                s.Objective = query.GetDoubleWithNull(25);
                s.ObjDistance = query.GetDoubleWithNull(26);
                s.ObjTime = query.GetLongWithNull(27);

            }

            return s;
        }

        internal Paths GetPath(int idPath)
        {
            var c = new SqliteCommand("select id,description, typePath from Paths where id=@id", db);
            c.Parameters.AddWithValue("@id", idPath);

            Paths p = null;
            var query = c.ExecuteReader();
            while (query.Read())
            {
                p = new Paths();
                p.Id = query.GetInt32(0);
                p.Description = query.GetString(1);
                p.Type = query.GetString(2);
            }

            return p;
        }

        private int LastInsertRowid()
        {
            var c = new SqliteCommand(@"select last_insert_rowid()", db);
            long lastId = ((Int64)c.ExecuteScalar());

            return (int)lastId;
        }

        internal void InsertPath(Paths path)
        {
            var c = new SqliteCommand("insert into Paths (description, typePath) values (@description,@type)", db);
            c.Parameters.AddWithValue("@description", path.Description);
            c.Parameters.AddWithValue("@type", path.Type);

            var r = c.ExecuteReader();

            path.Id = (int)LastInsertRowid();
        }


        internal void InsertPoint(Sessions sessionData, string v, Geocoordinate t, double currentDistance, long currentTime, string type)
        {
            lock (lockPoint)
            {
                var c = new SqliteCommand("insert into PointInterest (latitude,longitude,distance,time,rhythm,message,idSession, type) values (@latitude,@longitude,@distance,@time,@rhythm,@message,@idSession, @type)", db);
                c.Parameters.AddWithValue("@latitude", t.Point.Position.Latitude);
                c.Parameters.AddWithValue("@longitude", t.Point.Position.Longitude);
                c.Parameters.AddWithValue("@distance", currentDistance);
                c.Parameters.AddWithValue("@time", currentTime);
                c.Parameters.AddWithNullable("@rhythm", null);
                c.Parameters.AddWithNullable("@message", v);
                c.Parameters.AddWithValue("@idSession", sessionData.Id);
                c.Parameters.AddWithNullable("@type", type);

                var r = c.ExecuteReader();
            }
        }


        internal void InsertPoint(PointInterest lastPoint)
        {
            lock (lockPoint)
            {
                var c = new SqliteCommand("insert into PointInterest (latitude,longitude,distance,time,rhythm,message,idSession, type) values (@latitude,@longitude,@distance,@time,@rhythm,@message,@idSession, @type)", db);
                c.Parameters.AddWithValue("@latitude", lastPoint.Latitude);
                c.Parameters.AddWithValue("@longitude", lastPoint.Longitude);
                c.Parameters.AddWithValue("@distance", lastPoint.Distance);
                c.Parameters.AddWithValue("@time", lastPoint.Time);
                c.Parameters.AddWithNullable("@rhythm", lastPoint.Rhythm);
                c.Parameters.AddWithNullable("@message", lastPoint.Message);
                c.Parameters.AddWithValue("@idSession", lastPoint.IdSession);
                c.Parameters.AddWithNullable("@type", lastPoint.Type);

                var r = c.ExecuteReader();

                lastPoint.Id = LastInsertRowid();
            }
        }

        internal void RenameSession(Sessions sessionData, string text)
        {
            var c = new SqliteCommand("update Sessions set comment=@comment where id = @id", db);

            c.Parameters.AddWithValue("@id", sessionData.Id);
            c.Parameters.AddWithValue("@comment", text);
            var r = c.ExecuteReader();
            sessionData.Comment = text;
        }


        internal void UpdateSession(Sessions sessionData)
        {
            var c = new SqliteCommand("update Sessions set comment=@comment,dayOfSession=@dayOfSession, idPath=@idPath,minLatitude=@minLatitude,minLongitude=@minLongitude,maxLatitude=@maxLatitude,maxLongitude=@maxLongitude,distance=@distance,duration=@duration,minSpeed=@minSpeed,maxSpeed=@maxSpeed,avgSpeed=@avgSpeed," +
            "minAltitude=@minAltitude,maxAltitude=@maxAltitude,avgAltitude=@avgAltitude,centerLatitude=@centerLatitude,centerLongitude=@centerLongitude,ascendent=@ascendent,descendent=@descendent,avgPace=@avgPace,startLongitude=@startLongitude,startLatitude=@startLatitude,endLongitude=@endLongitude,endLatitude=@endLatitude,objective=@objective," +
            "objectiveDistance=@objectiveDistance,objectiveTime=@objectiveTime  where id = @id", db);

            c.Parameters.AddWithValue("@id", sessionData.Id);
            c.Parameters.AddWithValue("@comment", sessionData.Comment);
            c.Parameters.AddWithValue("@dayOfSession", sessionData.DayOfSession);
            c.Parameters.AddWithValue("@idPath", sessionData.IdPath);
            c.Parameters.AddWithNullable("@minLatitude", sessionData.MinLat);
            c.Parameters.AddWithNullable("@minLongitude", sessionData.MinLon);
            c.Parameters.AddWithNullable("@maxLatitude", sessionData.MaxLat);
            c.Parameters.AddWithNullable("@maxLongitude", sessionData.MaxLon);
            c.Parameters.AddWithNullable("@distance", sessionData.Distance);
            c.Parameters.AddWithNullable("@duration", sessionData.Duration);
            c.Parameters.AddWithNullable("@minSpeed", sessionData.MinSpeed);
            c.Parameters.AddWithNullable("@maxSpeed", sessionData.MaxSpeed);
            c.Parameters.AddWithNullable("@avgSpeed", sessionData.AvgSpeed);
            c.Parameters.AddWithNullable("@minAltitude", sessionData.MinAltitude);
            c.Parameters.AddWithNullable("@maxAltitude", sessionData.MaxAltitude);
            c.Parameters.AddWithNullable("@avgAltitude", sessionData.AvgAltitude);
            c.Parameters.AddWithNullable("@centerLatitude", sessionData.CenterLat);
            c.Parameters.AddWithNullable("@centerLongitude", sessionData.CenterLon);
            c.Parameters.AddWithNullable("@ascendent", sessionData.Ascendent);
            c.Parameters.AddWithNullable("@descendent", sessionData.Descendent);
            c.Parameters.AddWithNullable("@avgPace", sessionData.AvgPace);
            c.Parameters.AddWithNullable("@startLongitude", sessionData.StartLon);
            c.Parameters.AddWithNullable("@startLatitude", sessionData.StartLat);
            c.Parameters.AddWithNullable("@endLongitude", sessionData.EndLon);
            c.Parameters.AddWithNullable("@endLatitude", sessionData.EndLat);
            c.Parameters.AddWithNullable("@objective", sessionData.Objective);
            c.Parameters.AddWithNullable("@objectiveDistance", sessionData.ObjDistance);
            c.Parameters.AddWithNullable("@objectiveTime", sessionData.ObjTime);

            var r = c.ExecuteReader();

        }
        internal IEnumerable<Paths> getPaths(string mode)
        {
            IList<Paths> result = new List<Paths>();

            SqliteCommand c;
            if (mode != null)
            {
                c = new SqliteCommand("select id,description, typePath from Paths where typePath=@type", db);
                c.Parameters.AddWithValue("@type", mode);
            }
            else
            {
                c = new SqliteCommand("select id,description, typePath from Paths", db);
            }

            var query = c.ExecuteReader();
            while (query.Read())
            {
                var p = new Paths();
                p.Id = query.GetInt32(0);
                p.Description = query.GetString(1);
                p.Type = query.GetString(2);

                result.Add(p);
            }

            return result;
        }

        public IDictionary<string, List<object>> getSummary()
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;

            string type = nowhereman.Properties.getProperty("graphType", "D");

            string format = "yyyy'-'MM'-'dd 00:00";
            int N = 30;

            switch (type)
            {
                case "D":
                    format = "yyyy'-'MM'-'dd 00:00";
                    N = 45;
                    break;
                case "M":
                    format = "yyyy'-'MM'-'01 00:00";
                    N = 24;
                    break;
                case "Y":
                    format = "yyyy'-'00'-'01 00:00";
                    N = 15;
                    break;
                case "W":
                    format = "yyyy'-'MM'-'dd 00:00";
                    N = 30;
                    break;
            }

            IDictionary<string, List<object>> res = new Dictionary<string, List<object>>();
            string start; //, end;
            DateTime _start = DateTime.Now;

            //start = _start.ToString(format);
            //_start = DateTime.ParseExact(start, "yyyy'-'MM'-'dd HH':'mm", CultureInfo.InvariantCulture);
            switch (type)
            {
                case "D":
                    _start = cal.AddDays(_start, -N + 1);
                    break;
                case "M":
                    _start = cal.AddMonths(_start, -N + 1);
                    break;
                case "W":
                    _start = cal.AddWeeks(_start, -N + 1);
                    break;
                case "Y":
                    _start = cal.AddYears(_start, -N + 1);
                    break;
            }

            start = _start.ToString("yyyy'-'MM'-'dd");

            List<object> days = new List<object>();
            List<object> time = new List<object>();
            List<object> distance = new List<object>();
            List<object> sub = new List<object>();

            res.Add("days", days);
            res.Add("time", time);
            res.Add("distance", distance);
            res.Add("sub", sub);

            //IDictionary<string, string> d = new Dictionary<string, string>();
            for (int i = 0; i < N; i++)
            {
                string tmp;
                switch (type)
                {
                    case "D":
                        tmp = _start.ToString(format);
                        days.Add(tmp);
                        time.Add((long)0);
                        distance.Add((double)0);
                        if (cal.GetDayOfWeek(_start) == dfi.FirstDayOfWeek)
                        {
                            sub.Add(true);
                        }
                        else
                        {
                            sub.Add(false);
                        }
                        _start = cal.AddDays(_start, 1);
                        break;
                    case "Y":
                        tmp = _start.ToString(format);
                        days.Add(tmp);
                        time.Add((long)0);
                        distance.Add((double)0);
                        if (i % 4 == 0)
                        {
                            sub.Add(true);
                        }
                        else
                        {
                            sub.Add(false);
                        }
                        _start = cal.AddYears(_start, 1);
                        break;
                    case "M":
                        tmp = _start.ToString(format);
                        days.Add(tmp);
                        time.Add((long)0);
                        distance.Add((double)0);
                        if (_start.Month == 1)
                        {
                            sub.Add(true);
                        }
                        else
                        {
                            sub.Add(false);
                        }
                        _start = cal.AddMonths(_start, 1);
                        break;
                    case "W":
                        int n = cal.GetWeekOfYear(_start, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                        tmp = cal.GetYear(_start).ToString("0000") + "-" + n.ToString("000");
                        days.Add(tmp);
                        time.Add((long)0);
                        distance.Add((double)0);
                        if (n % 4 == 0)
                        {
                            sub.Add(true);
                        }
                        else
                        {
                            sub.Add(false);
                        }
                        _start = cal.AddWeeks(_start, 1);
                        break;
                }

            }

            //q14 = CompiledQuery.Compile((DataBaseManager dc, string start) => from a in Sessions
            //                                                                  where a.duration != null && a.distance != null && a.avgPace > 0 && a.dayOfSession.CompareTo(start) >= 0 && a.Paths.type != "I"
            //                                                                  group a by a.dayOfSession.Substring(0, 10) into g
            //                                                                  select new DummyResult2()
            //                                                                  {
            //                                                                      day = g.Key,
            //                                                                      totalTime = g.Sum(t => t.duration),
            //                                                                      totalDistance = g.Sum(t => t.distance),
            //                                                                      totalNumber = g.Count()
            //                                                                  });

            bool hasValues = false;

            var c = new SqliteCommand("select substr(dayOfSession, 0, 11),sum(duration), sum(distance), count(1) from Sessions where duration is not null and distance is not null and avgPace > 0 and dayOfSession>=@start group by substr(dayOfSession, 0, 11)", db);
            c.Parameters.AddWithValue("@start", start);

            var query = c.ExecuteReader();
            while (query.Read())
            {
                string k = query.GetString(0);
                DateTime v = DateTime.ParseExact(k, "yyyy'-'MM'-'dd", CultureInfo.InvariantCulture);
                switch (type)
                {
                    case "D":
                        k = v.ToString(format);
                        break;
                    case "M":
                        k = v.ToString(format);
                        break;
                    case "Y":
                        k = v.ToString(format);
                        break;
                    case "W":
                        int n = cal.GetWeekOfYear(v, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                        k = cal.GetYear(v).ToString("0000") + "-" + n.ToString("000");
                        break;
                }
                int i = days.IndexOf(k);
                if (i >= 0)
                {
                    hasValues = true;

                    var totalTime = query.GetLongWithNull(1);
                    var totalDistance = query.GetDoubleWithNull(2);

                    time[i] = (long)time[i] + (long)(totalTime);
                    distance[i] = (double)distance[i] + totalDistance;
                }
            }

            if (hasValues)
                return res;
            else
                return null;
        }


        public int getLongestDistance(string mode)
        {
            /*var allList = from a in Sessions 
                          where a.duration != null && a.distance != null && a.avgPace > 0 && a.Paths.type == mode
                          orderby a.distance descending select a;*/

            var c = new SqliteCommand("select a.id from Sessions a,Paths b where a.idPath=b.id and a.duration is not null and a.distance is not null and a.avgPace > 0 and b.typePath=@mode order by a.distance desc", db);
            c.Parameters.AddWithValue("@mode", mode);

            var query = c.ExecuteReader();
            while (query.Read())
            {
                return query.GetInt32(0);
            }
            return -1;
        }

        public int getLongestTime(string mode)
        {
            /*var allList = from a in Sessions
                          where a.duration != null && a.distance != null && a.avgPace > 0 && a.Paths.type == mode
                          orderby a.duration descending select a;*/

            var c = new SqliteCommand("select a.id from Sessions a,Paths b where a.idPath=b.id and a.duration is not null and a.distance is not null and a.avgPace > 0 and b.typePath=@mode order by a.duration desc", db);
            c.Parameters.AddWithValue("@mode", mode);

            var query = c.ExecuteReader();
            while (query.Read())
            {
                return query.GetInt32(0);
            }
            return -1;
        }

        public int getHigherSpeed(string mode)
        {
            /*var allList = from a in Sessions 
                          where a.Paths.type == mode 
                          orderby a.avgSpeed descending select a;*/

            var c = new SqliteCommand("select a.id from Sessions a,Paths b where a.idPath=b.id and b.typePath=@mode order by a.avgSpeed desc", db);
            c.Parameters.AddWithValue("@mode", mode);

            var query = c.ExecuteReader();
            while (query.Read())
            {
                return query.GetInt32(0);
            }
            return -1;
        }

        public int getHigherPace(string mode)
        {
            /*var allList = from a in Sessions
                          where a.duration != null && a.distance != null && a.avgPace > 0 && a.Paths.type == mode
                          orderby a.avgPace select a;*/

            var c = new SqliteCommand("select a.id from Sessions a,Paths b where a.idPath=b.id and a.duration is not null and a.distance is not null and a.avgPace > 0 and b.typePath=@mode order by a.avgPace", db);
            c.Parameters.AddWithValue("@mode", mode);

            var query = c.ExecuteReader();
            while (query.Read())
            {
                return query.GetInt32(0);
            }
            return -1;
        }


        public IDictionary<string, object> getTotals()
        {
            /*var allList = from a in Sessions 
                          where a.duration != null && a.distance !=  null && a.avgPace > 0
                          group a by a.Paths.type into g 
                          select new
                          {
                              type = g.Key,
                              maxSpeed = g.Max(t => t.maxSpeed),
                              maxDuration = g.Max(t => t.duration),
                              maxDistance = g.Max(t => t.distance),
                              maxPace = g.Min(t => t.avgPace),
                              totalTime = g.Sum(t => t.duration),
                              totalDistance = g.Sum(t => t.distance),
                              totalNumber = g.Count()
                          };*/

            var d = new Dictionary<string, object>();

            double totalDistance = 0;
            long totalTime = 0;
            int totalNumber = 0;

            var c = new SqliteCommand("select b.typePath,max(a.maxSpeed),max(a.duration),max(a.distance),min(a.avgPace),sum(a.duration),sum(a.distance),count(1) from Sessions a,Paths b where a.idPath=b.id and a.duration is not null and a.distance is not null and a.avgPace > 0 group by b.typePath", db);

            var query = c.ExecuteReader();
            while (query.Read())
            {
                var type = query.GetString(0);
                var tmpmaxSpeed = query.GetDoubleWithNull(1);
                var tmpmaxDuration = query.GetLongWithNull(2);
                var tmpmaxDistance = query.GetDoubleWithNull(3);
                var tmpmaxPace = query.GetDoubleWithNull(4);
                var tmptotalTime = query.GetLong(5, 0L);
                var tmptotalDistance = query.GetDouble(6, 0.0);
                var tmptotalNumber = query.GetInt(7, 0);

                if (type.Equals("I"))
                {
                    d.Add("maxSpeed" + type, 0.0);
                    d.Add("maxDuration" + type, 0L);
                    d.Add("maxDistance" + type, tmpmaxDistance);
                    //totalTime += tmptotalTime.HasValue ? tmptotalTime.Value : 0;
                    d.Add("totalTime" + type, 0L);
                    //totalDistance += tmptotalDistance.HasValue ? tmptotalDistance.Value : 0;
                    d.Add("totalDistance" + type, tmptotalDistance);
                    //totalNumber += tmptotalNumber;
                    d.Add("totalNumber" + type, tmptotalNumber);
                    d.Add("maxPace" + type, 0.0);
                }
                else
                {
                    d.Add("maxSpeed" + type, tmpmaxSpeed);
                    d.Add("maxDuration" + type, tmpmaxDuration);
                    d.Add("maxDistance" + type, tmpmaxDistance);
                    totalTime += tmptotalTime;
                    d.Add("totalTime" + type, tmptotalTime);
                    totalDistance += tmptotalDistance;
                    d.Add("totalDistance" + type, tmptotalDistance);
                    totalNumber += tmptotalNumber;
                    d.Add("totalNumber" + type, tmptotalNumber);
                    d.Add("maxPace" + type, tmpmaxPace);
                }
            }

            if (totalNumber > 0)
            {
                d.Add("totalTimeT", totalTime);
                d.Add("totalDistanceT", totalDistance);
                d.Add("totalNumberT", totalNumber);
            }

            return d;
        }


        internal void InsertMesure(Mesures mesure)
        {
            lock (lockPoint)
            {
                try
                {
                    var c = new SqliteCommand("insert into Mesures (idSession , id ,latitude, longitude, speed,altitude,action) values (@idSession , @id ,@latitude, @longitude, @speed,@altitude,@action)", db);
                    c.Parameters.AddWithValue("@idSession", mesure.IdSession);
                    c.Parameters.AddWithValue("@id", mesure.Id);
                    c.Parameters.AddWithValue("@latitude", mesure.Latitude);
                    c.Parameters.AddWithValue("@longitude", mesure.Longitude);
                    c.Parameters.AddWithNullable("@speed", mesure.Speed);
                    c.Parameters.AddWithNullable("@altitude", mesure.Altitude);
                    c.Parameters.AddWithNullable("@action", mesure.Action);

                    var r = c.ExecuteReader();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
        }

        public IEnumerable<Mesures> GetMesures(int idSession)
        {
            /*var allList = (from a in Mesures where a.idSession == _idSession orderby a.idSession, a.id select a);
            return allList;*/

            IList<Mesures> result = new List<Mesures>();

            SqliteCommand c;
            c = new SqliteCommand("select id ,latitude, longitude, speed,altitude,action from Mesures where idSession=@idSession order by idSession,id", db);
            c.Parameters.AddWithValue("@idSession", idSession);

            var query = c.ExecuteReader();
            while (query.Read())
            {
                var p = new Mesures();
                p.IdSession = idSession;
                p.Id = query.GetInt64(0);
                p.Latitude = query.GetDouble(1);
                p.Longitude = query.GetDouble(2);
                p.Speed = query.GetDoubleWithNull(3);
                p.Altitude = query.GetDoubleWithNull(4);
                p.Action = query.GetString(5, "");

                result.Add(p);
            }

            return result;
        }

        public IEnumerable<PointInterest> GetPoints(int idSession)
        {
            IList<PointInterest> result = new List<PointInterest>();

            SqliteCommand c;
            c = new SqliteCommand("select latitude,longitude,distance,time,rhythm,message,type from PointInterest where idSession=@idSession order by idSession, id", db);
            c.Parameters.AddWithValue("@idSession", idSession);

            var query = c.ExecuteReader();
            while (query.Read())
            {
                var p = new PointInterest();
                p.IdSession = idSession;
                p.Latitude = query.GetDouble(0);
                p.Longitude = query.GetDouble(1);
                p.Distance = query.GetDouble(2);
                p.Time = query.GetInt64(3);
                p.Rhythm = query.GetDoubleWithNull(4);
                p.Message = query.GetString(5);
                p.Type = query.GetString(6);

                result.Add(p);
            }

            return result;

        }


        void DeleteMesures(Mesures mesure)
        {
            SqliteCommand c;

            c = new SqliteCommand("delete from Mesures where idSession=@idSession and id=@id", db);
            c.Parameters.AddWithValue("@idSession", mesure.IdSession);
            c.Parameters.AddWithValue("@id", mesure.Id);
            var query = c.ExecuteReader();

        }

        internal void DeleteSession(Sessions sessionData, bool sure = true)
        {
            try
            {
                SqliteCommand c;
                c = new SqliteCommand("delete from PointInterest where idSession=@idSession", db);
                c.Parameters.AddWithValue("@idSession", sessionData.Id);

                var query = c.ExecuteReader();

                c = new SqliteCommand("delete from Mesures where idSession=@idSession", db);
                c.Parameters.AddWithValue("@idSession", sessionData.Id);
                query = c.ExecuteReader();

                c = new SqliteCommand("delete from Sessions where id=@idSession", db);
                c.Parameters.AddWithValue("@idSession", sessionData.Id);
                query = c.ExecuteReader();

                //var p = GetPath(sessionData.IdPath);
                if (sure && !hasPathSessions(sessionData.IdPath))
                {
                    DeletePath(sessionData.IdPath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        void DeletePath(int id)
        {
            SqliteCommand c;
            c = new SqliteCommand("delete from Paths where id=@id", db);
            c.Parameters.AddWithValue("@id", id);

            var query = c.ExecuteReader();
        }

        public void compressData(Sessions obj)
        {
            var messures = GetMesures(obj.Id);

            int i = 0;
            Mesures previous = null;
            foreach (Mesures tmp in messures)
            {
                if (i % 2 == 0 && previous != null)
                {
                    DeleteMesures(previous);
                }
                if (i > 0)
                {
                    previous = tmp;
                }
                i++;
            }
        }

        public void RenamePath(Paths obj, string newName)
        {
            var c = new SqliteCommand("select id from Paths where id!=@id and description = @description and typePath=@type", db);
            c.Parameters.AddWithValue("@id", obj.Id);
            c.Parameters.AddWithValue("@description", newName);
            c.Parameters.AddWithValue("@type", obj.Type);

            Paths exists = null;
            var query = c.ExecuteReader();
            while (query.Read())
            {
                exists = new Paths();
                exists.Id = query.GetInt32(0);
            }

            if (exists != null)
            {
                c = new SqliteCommand("update Sessions set idPath=@idPathNew where idPath=@idPathOld", db);
                c.Parameters.AddWithValue("@idPathNew", exists.Id);
                c.Parameters.AddWithValue("@idPathOld", obj.Id);

                query = c.ExecuteReader();

                c = new SqliteCommand("delete from Paths where id=@id", db);
                c.Parameters.AddWithValue("@id", obj.Id);

                query = c.ExecuteReader();
            }
            else
            {
                c = new SqliteCommand("update Paths set description=@description where id=@id", db);
                c.Parameters.AddWithValue("@id", obj.Id);
                c.Parameters.AddWithValue("@description", newName);

                query = c.ExecuteReader();
            }

            obj.Description = newName;
        }

        public void DeletePath(Paths obj)
        {
            foreach(var session in GetSessions(obj.Id))
            {
                DeleteSession(session, false);
            }
            var c = new SqliteCommand("delete Paths where id=@id", db);
            c.Parameters.AddWithValue("@id", obj.Id);
            var query = c.ExecuteReader();
        }

        public void UpdatePath(Paths path)
        {
            var c = new SqliteCommand("update Paths set description=@description, typePath=@type where id=@id", db);
            c.Parameters.AddWithValue("@id", path.Id);
            c.Parameters.AddWithValue("@description", path.Description);
            c.Parameters.AddWithValue("@type", path.Type);

            var query = c.ExecuteReader();
        }

        //public void updatePath(int idPath, string comme, string mode)
        //{
        //    var c = new SqliteCommand("update Paths set description=@description, typePath=@type where id=@id", db);
        //    c.Parameters.AddWithValue("@id", idPath);
        //    c.Parameters.AddWithValue("@description", comme);
        //    c.Parameters.AddWithValue("@type", mode);

        //    var query = c.ExecuteReader();
        //}

        //public void updateSession(int id, string c)
        //{
        ////    var list = (from a in Sessions where a.id == id select a);
        ////    foreach (var t in list)
        ////    {
        ////        t.comment = c;
        ////    }
        ////    SubmitChanges();

        //}

        public bool hasPathSessions(int id)
        {
            SqliteCommand c;
            c = new SqliteCommand("select count(1) from Sessions where idPath=@idPath", db);
            c.Parameters.AddWithValue("@idPath", id);

            var query = c.ExecuteReader();
            while (query.Read())
            {
                return query.GetInt64(0) > 0;
            }
            return false;
        }



        public string analyze(Sessions session, string mode)
        {
            if (session == null || !session.Duration.HasValue || !session.Distance.HasValue || !session.AvgPace.HasValue
                || session.Duration.Value <= TimeSpan.FromMinutes(1).Ticks || session.Distance.Value <= 500) return null;

            //var w1 = from a in Sessions
            //         where a.duration != null && a.distance != null && a.avgPace > 0 && a.Paths.type == session.Paths.type
            //         && (a.duration.Value > session.duration.Value || a.distance.Value > session.distance.Value || a.avgPace.Value < session.avgPace.Value)
            //         select a;

            var c = new SqliteCommand("select a.distance,a.duration,a.avgPace from Sessions a, Paths b where a.idPath=b.id and  a.duration is not null and a.distance is not null and a.avgPace > 0 and b.typePath == @type and (a.duration > @duration or a.distance > @distance or a.avgPace < @avgPace)", db);
            c.Parameters.AddWithValue("@type", mode);
            c.Parameters.AddWithValue("@distance", session.Distance.Value);
            c.Parameters.AddWithValue("@duration", session.Duration.Value);
            c.Parameters.AddWithValue("@avgPace", session.AvgPace.Value);


            bool c1 = false, c2 = false, c3 = false;

            var query = c.ExecuteReader();
            while (query.Read())
            {
                var distance = query.GetDoubleWithNull(0);
                var duration = query.GetLongWithNull(1);
                var avgPace = query.GetDoubleWithNull(2);

                if (duration > session.Duration.Value) c1 = true;
                if (distance > session.Distance.Value) c2 = true;
                if (avgPace < session.AvgPace.Value) c3 = true;

                if (c1 && c2 && c3) break;
            }

            if (!c1 && !c2 && !c3)
            {
                // Best mark
                return "BestMarkMessage";
            }
            else if (c1 && !c2 && !c3)
            {
                // Longest distance and fast
                return "LongestDistanceAndFastMessage";
            }
            else if (!c1 && c2 && !c3)
            {
                // Longest duration and fast
                return "LongestDurationAndFastMessage";
            }
            else if (!c1 && !c2 && c3)
            {
                // Longest
                return "LongestMessage";
            }

            //foreach (var tmp in q12(this)) //allList)
            //{
            //    if (tmp.type.Equals(session.Paths.type))
            //    {
            //        long totalTime = tmp.totalTime.HasValue ? tmp.totalTime.Value : 0;
            //        double totalDistance = tmp.totalDistance.HasValue ? tmp.totalDistance.Value : 0;

            //        long t1 = ((long)Math.Round(totalDistance - session.distance.Value)) / 500L;
            //        long t2 = ((long)Math.Round(totalDistance)) / 500L;

            //        if (t1 < t2)
            //        {

            //        }

            //        break;
            //    }
            //}

            return null;
        }

    }

}
