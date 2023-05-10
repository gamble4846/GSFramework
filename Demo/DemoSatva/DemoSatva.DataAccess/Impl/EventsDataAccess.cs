using DemoSatva.DataAccess.Interface;
using DemoSatva.Model;
using DemoSatva.Utility;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DemoSatva.DataAccess.Impl
{
    public class EventsDataAccess : IEventsDataAccess
    {
        private MSSqlDatabase MSSqlDatabase { get; set; }
        public EventsDataAccess(MSSqlDatabase msSqlDatabase)
        {
            MSSqlDatabase = msSqlDatabase;
        }
        public List<EventsModel> GetAllEvents(int page = 1, int itemsPerPage = 100, List<OrderByModel> orderBy = null)
        {
            var ret = new List<EventsModel>();
            int offset = (page - 1) * itemsPerPage;
            var cmd = this.MSSqlDatabase.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = @"SELECT  t.* FROM Events t  Order by t.Id OFFSET @Offset ROWS FETCH NEXT @ItemsPerPage ROWS ONLY";
            if (orderBy != null && orderBy.Count > 0)
            {
                cmd.CommandText = Helper.ConverOrderListToSQL(cmd.CommandText, orderBy);
            }
            cmd.Parameters.AddWithValue("@Offset", offset);
            cmd.Parameters.AddWithValue("@ItemsPerPage", itemsPerPage);
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                {
                    var t = new EventsModel()
                    {
                        Id = reader.GetValue<Int32>("Id"),
                        EventTitle = reader.GetValue<String>("EventTitle"),
                        StartDate = reader.GetValue<DateTime>("StartDate"),
                        EndDate = reader.GetValue<DateTime>("EndDate"),
                        EventDescription = reader.GetValue<String>("EventDescription"),
                        EventPriority = reader.GetValue<Int32>("EventPriority"),
                    };

                    ret.Add(t);
                }
            return ret;
        }

        public List<EventsModel> SearchEvents(string searchKey, int page = 1, int itemsPerPage = 100, List<OrderByModel> orderBy = null)
        {
            var ret = new List<EventsModel>();
            int offset = (page - 1) * itemsPerPage;
            var cmd = this.MSSqlDatabase.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = @"SELECT  t.* FROM Events t  WHERE  t.EventTitle LIKE CONCAT('%',@SearchKey,'%') OR t.EventDescription LIKE CONCAT('%',@SearchKey,'%') Order by t.Id OFFSET @Offset ROWS FETCH NEXT @ItemsPerPage ROWS ONLY";
            if (orderBy != null && orderBy.Count > 0)
            {
                cmd.CommandText = Helper.ConverOrderListToSQL(cmd.CommandText, orderBy);
            }
            cmd.Parameters.AddWithValue("@SearchKey", searchKey);
            cmd.Parameters.AddWithValue("@Offset", offset);
            cmd.Parameters.AddWithValue("@ItemsPerPage", itemsPerPage);
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                {
                    var t = new EventsModel()
                    {
                        Id = reader.GetValue<Int32>("Id"),
                        EventTitle = reader.GetValue<String>("EventTitle"),
                        StartDate = reader.GetValue<DateTime>("StartDate"),
                        EndDate = reader.GetValue<DateTime>("EndDate"),
                        EventDescription = reader.GetValue<String>("EventDescription"),
                        EventPriority = reader.GetValue<Int32>("EventPriority"),
                    };

                    ret.Add(t);
                }
            return ret;
        }

        public int GetAllTotalRecordEvents()
        {
            var cmd = this.MSSqlDatabase.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = @"SELECT Count(*) as TotalRecord FROM Events t";
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                {
                    return reader.GetInt32("TotalRecord");
                }
            return 0;
        }
        public int GetSearchTotalRecordEvents(string searchKey)
        {
            var cmd = this.MSSqlDatabase.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = @"SELECT Count(*) as TotalRecord FROM Events t  WHERE  t.EventTitle LIKE CONCAT('%',@SearchKey,'%') OR t.EventDescription LIKE CONCAT('%',@SearchKey,'%')";
            cmd.Parameters.AddWithValue("@SearchKey", searchKey);
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                {
                    return reader.GetInt32("TotalRecord");
                }
            return 0;
        }



        public EventsModel GetEventsByID(int Id)
        {

            var cmd = this.MSSqlDatabase.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = @"SELECT  t.* FROM Events t  WHERE t.Id= @Id Order by t.Id OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY";
            cmd.Parameters.AddWithValue("@Id", Id);

            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                {
                    return new EventsModel()
                    {
                        Id = reader.GetValue<Int32>("Id"),
                        EventTitle = reader.GetValue<String>("EventTitle"),
                        StartDate = reader.GetValue<DateTime>("StartDate"),
                        EndDate = reader.GetValue<DateTime>("EndDate"),
                        EventDescription = reader.GetValue<String>("EventDescription"),
                        EventPriority = reader.GetValue<Int32>("EventPriority"),
                    };
                }
            return null;
        }

        public List<EventsModel> FilterEvents(List<FilterModel> filterBy, string andOr, int page, int itemsPerPage, List<OrderByModel> orderBy)
        {
            var ret = new List<EventsModel>();
            int offset = (page - 1) * itemsPerPage;
            var cmd = this.MSSqlDatabase.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = @"SELECT  t.* FROM Events t  {filterColumns} Order by t.Id OFFSET @Offset ROWS FETCH NEXT @ItemsPerPage ROWS ONLY";
            if (filterBy != null && filterBy.Count > 0)
            {
                var whereClause = string.Empty;
                int paramCount = 0;
                foreach (var r in filterBy)
                {
                    if (!string.IsNullOrEmpty(r.ColumnName))
                    {
                        paramCount++;
                        if (!string.IsNullOrEmpty(whereClause))
                        {
                            whereClause = whereClause + " " + andOr + " ";
                        }
                        whereClause = whereClause + "t." + r.ColumnName + "=@" + r.ColumnName + paramCount;
                        cmd.Parameters.AddWithValue("@" + r.ColumnName + paramCount, r.ColumnValue);
                    }
                }
                whereClause = whereClause.Trim();
                cmd.CommandText = cmd.CommandText.Replace("{filterColumns}", "Where " + whereClause);
            }
            else
            {
                cmd.CommandText = cmd.CommandText.Replace("{filterColumns}", "");
            }
            if (orderBy != null && orderBy.Count > 0)
            {
                cmd.CommandText = Helper.ConverOrderListToSQL(cmd.CommandText, orderBy);
            }
            cmd.Parameters.AddWithValue("@Offset", offset);
            cmd.Parameters.AddWithValue("@ItemsPerPage", itemsPerPage);
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                {
                    var t = new EventsModel()
                    {
                        Id = reader.GetValue<Int32>("Id"),
                        EventTitle = reader.GetValue<String>("EventTitle"),
                        StartDate = reader.GetValue<DateTime>("StartDate"),
                        EndDate = reader.GetValue<DateTime>("EndDate"),
                        EventDescription = reader.GetValue<String>("EventDescription"),
                        EventPriority = reader.GetValue<Int32>("EventPriority"),
                    };

                    ret.Add(t);
                }
            return ret;
        }

        public int GetFilterTotalRecordEvents(List<FilterModel> filterBy, string andOr)
        {
            var cmd = this.MSSqlDatabase.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = @"SELECT Count(*) as TotalRecord FROM Events t {filterColumns}";
            if (filterBy != null && filterBy.Count > 0)
            {
                int paramCount = 0;
                var whereClause = string.Empty;
                foreach (var r in filterBy)
                {
                    if (!string.IsNullOrEmpty(r.ColumnName))
                    {
                        paramCount++;
                        if (!string.IsNullOrEmpty(whereClause))
                        {
                            whereClause = whereClause + " " + andOr + " ";
                        }
                        whereClause = whereClause + "t." + r.ColumnName + "=@" + r.ColumnName + paramCount;
                        cmd.Parameters.AddWithValue("@" + r.ColumnName + paramCount, r.ColumnValue);
                    }
                }
                whereClause = whereClause.Trim();
                cmd.CommandText = cmd.CommandText.Replace("{filterColumns}", "Where " + whereClause);
            }
            else
            {
                cmd.CommandText = cmd.CommandText.Replace("{filterColumns}", "");
            }
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                {
                    return reader.GetInt32("TotalRecord");
                }
            return 0;
        }

        public bool UpdateEvents(EventsModel model)
        {
            var cmd = this.MSSqlDatabase.Connection.CreateCommand() as SqlCommand;
            SqlTransaction transaction = this.MSSqlDatabase.Connection.BeginTransaction("");
            cmd.Transaction = transaction;
            cmd.CommandText = @"UPDATE Events SET EventTitle=@EventTitle,StartDate=@StartDate,EndDate=@EndDate,EventDescription=@EventDescription,EventPriority=@EventPriority WHERE Id = @Id;";
            cmd.Parameters.AddWithValue("@EventTitle", model.EventTitle);
            cmd.Parameters.AddWithValue("@StartDate", model.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", model.EndDate);
            cmd.Parameters.AddWithValue("@EventDescription", model.EventDescription);
            cmd.Parameters.AddWithValue("@EventPriority", model.EventPriority);
            cmd.Parameters.AddWithValue("@Id", model.Id);
            var recs = cmd.ExecuteNonQuery();
            if (recs > 0)
            {
                transaction.Commit();
                return true;
            }
            return false;
        }

        public long AddEvents(EventsModel model)
        {
            var cmd = this.MSSqlDatabase.Connection.CreateCommand() as SqlCommand;
            SqlTransaction transaction = this.MSSqlDatabase.Connection.BeginTransaction("");
            cmd.Transaction = transaction;
            cmd.CommandText = @"INSERT INTO Events (EventTitle,StartDate,EndDate,EventDescription,EventPriority) VALUES (@EventTitle,@StartDate,@EndDate,@EventDescription,@EventPriority);SELECT SCOPE_IDENTITY();";
            cmd.Parameters.AddWithValue("@EventTitle", model.EventTitle);
            cmd.Parameters.AddWithValue("@StartDate", model.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", model.EndDate);
            cmd.Parameters.AddWithValue("@EventDescription", model.EventDescription);
            cmd.Parameters.AddWithValue("@EventPriority", model.EventPriority);
            var recs = cmd.ExecuteScalar();
            if (recs != null)
            {
                transaction.Commit();
                return long.Parse(recs.ToString());
            }
            return -1;

        }

        public bool DeleteEvents(int Id)
        {
            var cmd = this.MSSqlDatabase.Connection.CreateCommand() as SqlCommand;
            SqlTransaction transaction = this.MSSqlDatabase.Connection.BeginTransaction("");
            cmd.Transaction = transaction;
            cmd.CommandText = @"DELETE FROM Events Where Id=@Id";
            cmd.Parameters.AddWithValue("@Id", Id);
            var recs = cmd.ExecuteNonQuery();
            if (recs > 0)
            {
                transaction.Commit();
                return true;
            }
            return false;
        }
        public bool DeleteMultipleEvents(List<DeleteMultipleModel> deleteParam, string andOr)
        {
            var cmd = this.MSSqlDatabase.Connection.CreateCommand() as SqlCommand;
            SqlTransaction transaction = this.MSSqlDatabase.Connection.BeginTransaction("");
            cmd.Transaction = transaction;
            cmd.CommandText = @"DELETE FROM Events Where";
            int count = 0;
            foreach (var r in deleteParam)
            {
                if (count == 0)
                {
                    cmd.CommandText = cmd.CommandText + " " + r.ColumnName + "=@" + r.ColumnName;
                }
                else
                {
                    cmd.CommandText = cmd.CommandText + " " + andOr + " " + r.ColumnName + "=@" + r.ColumnName;
                }
                cmd.Parameters.AddWithValue("@" + r.ColumnName, r.ColumnValue);
                count++;
            }

            var recs = cmd.ExecuteNonQuery();
            if (recs > 0)
            {
                transaction.Commit();
                return true;
            }
            return false;
        }

    }
}

