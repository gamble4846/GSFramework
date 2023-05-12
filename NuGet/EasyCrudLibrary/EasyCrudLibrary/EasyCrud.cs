using System.Data.SqlClient;
using System.Data;
using EasyCrudLibrary.Model;
using static EasyCrudLibrary.GSEnums;

namespace EasyCrudLibrary
{
    public class EasyCrud
    {
        private string ConnectionString = "";
        private SqlConnection ConnectionToSave = null;
        private SqlTransaction TransactionToSave = null;
        public EasyCrud(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void SaveChanges()
        {
            try
            {
                SetUpConnectionAndTransactionToSave();
                if (ConnectionToSave.State == ConnectionState.Closed)
                {
                    ConnectionToSave.Open();
                }
                TransactionToSave.Commit();
                if (ConnectionToSave.State == ConnectionState.Open)
                {
                    ConnectionToSave.Close();
                }
                ConnectionToSave = null;
                TransactionToSave = null;
            }
            catch
            {
                if (ConnectionToSave != null)
                    if (ConnectionToSave.State == ConnectionState.Open)
                    {
                        ConnectionToSave.Close();
                    }
                ConnectionToSave = null;
                TransactionToSave = null;
            }
        }

        public void RollBack()
        {
            try
            {
                SetUpConnectionAndTransactionToSave();
                if (ConnectionToSave.State == ConnectionState.Closed)
                {
                    ConnectionToSave.Open();
                }
                TransactionToSave.Rollback();
                if (ConnectionToSave.State == ConnectionState.Open)
                {
                    ConnectionToSave.Close();
                }
                ConnectionToSave = null;
                TransactionToSave = null;
            }
            catch
            {
                if (ConnectionToSave != null)
                    if (ConnectionToSave.State == ConnectionState.Open)
                    {
                        ConnectionToSave.Close();
                    }
                ConnectionToSave = null;
                TransactionToSave = null;
            }
        }

        private void SetUpConnectionAndTransactionToSave()
        {
            if (ConnectionToSave == null || TransactionToSave == null)
            {
                ConnectionToSave = new SqlConnection(ConnectionString);
                if (ConnectionToSave.State == ConnectionState.Closed)
                {
                    ConnectionToSave.Open();
                }
                TransactionToSave = ConnectionToSave.BeginTransaction();
            }
        }

        public List<T> GetList<T>(int page = -1, int itemsPerPage = -1, List<OrderByModel> orderBy = null, string WhereCondition = null, WithInQuery withInQuery = WithInQuery.None) where T : class, new()
        {
            var TableName = Utility.GetTableName<T>();
            var FirstPropertyName = Utility.GetFirstPropertyName<T>();
            var withString = Utility.GetWithString(withInQuery);
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                var ret = new List<T>();
                int offset = (page - 1) * itemsPerPage;
                string CommandText = "";

                if (page < 0 || itemsPerPage < 0)
                {
                    CommandText = @"SELECT  t.* FROM " + TableName + " t " + withString + " " + WhereCondition;
                }
                else
                {
                    CommandText = @"SELECT  t.* FROM " + TableName + " t " + withString + " " + WhereCondition + " Order by t." + FirstPropertyName + " OFFSET @Offset ROWS FETCH NEXT @ItemsPerPage ROWS ONLY";
                }

                SqlCommand cmd = new SqlCommand(CommandText, connection);

                if (orderBy != null && orderBy.Count > 0)
                {
                    cmd.CommandText = DataAccessHelper.ConverOrderListToSQL(cmd.CommandText, orderBy);
                }

                if (!(page < 0 || itemsPerPage < 0))
                {
                    cmd.Parameters.AddWithValue("@Offset", offset);
                    cmd.Parameters.AddWithValue("@ItemsPerPage", itemsPerPage);
                }

                try
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var t = Utility.ConvertReaderToObject<T>(reader);
                            ret.Add(t);
                        }
                    }
                }
                catch (Exception)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    throw;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }

                return ret;
            }
        }

        public T GetFirstOrDefault<T>(string WhereCondition = null, WithInQuery withInQuery = WithInQuery.None) where T : class, new()
        {
            var TableName = Utility.GetTableName<T>();
            var withString = Utility.GetWithString(withInQuery);
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string CommandText = @"SELECT  t.* FROM " + TableName + " t " + withString + " " + WhereCondition;

                SqlCommand cmd = new SqlCommand(CommandText, connection);

                try
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var t = Utility.ConvertReaderToObject<T>(reader);
                            if (connection.State == ConnectionState.Open)
                            {
                                connection.Close();
                            }
                            return t;
                        }
                    }
                }
                catch (Exception)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    throw;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }

                return null;
            }
        }

        public string Add<T>(T data, string OutputColumn = null, string toIgnoreColumns = null, bool AutoCommit = true) where T : class, new()
        {
            var TableName = Utility.GetTableName<T>();
            var FirstPropertyName = Utility.GetFirstPropertyName<T>();
            var toIgnoreColumnsList = new List<string>();
            toIgnoreColumnsList = toIgnoreColumns.Split(",").ToList();

            if (String.IsNullOrEmpty(OutputColumn))
            {
                OutputColumn = FirstPropertyName;
            }

            SqlConnection connection;

            if (AutoCommit)
            {
                connection = new SqlConnection(ConnectionString);
            }
            else
            {
                SetUpConnectionAndTransactionToSave();
                connection = ConnectionToSave;
            }

            SqlCommand cmd = new SqlCommand("", connection);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                if (!AutoCommit)
                {
                    cmd.Transaction = TransactionToSave;
                }
                return Utility.InsertRowToTable(data, TableName, OutputColumn, cmd, toIgnoreColumnsList);
            }
            catch (Exception)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                throw;
            }
            finally
            {
                if (AutoCommit)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public string Update<T>(T data, string WhereCondition = null, string toIgnoreColumns = null, bool AutoCommit = true) where T : class, new()
        {
            var TableName = Utility.GetTableName<T>();
            var toIgnoreColumnsList = new List<string>();
            toIgnoreColumnsList = toIgnoreColumns.Split(",").ToList();

            SqlConnection connection;

            if (AutoCommit)
            {
                connection = new SqlConnection(ConnectionString);
            }
            else
            {
                SetUpConnectionAndTransactionToSave();
                connection = ConnectionToSave;
            }

            SqlCommand cmd = new SqlCommand("", connection);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                if (!AutoCommit)
                {
                    cmd.Transaction = TransactionToSave;
                }
                return Utility.UpdateRowToTable(data, TableName, cmd, WhereCondition, toIgnoreColumnsList);
            }
            catch (Exception)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                throw;
            }
            finally
            {
                if (AutoCommit)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public string Remove<T>(string WhereCondition, bool AutoCommit = true) where T : class, new()
        {
            var TableName = Utility.GetTableName<T>();

            SqlConnection connection;

            if (AutoCommit)
            {
                connection = new SqlConnection(ConnectionString);
            }
            else
            {
                SetUpConnectionAndTransactionToSave();
                connection = ConnectionToSave;
            }

            SqlCommand cmd = new SqlCommand("", connection);
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                if (!AutoCommit)
                {
                    cmd.Transaction = TransactionToSave;
                }
                return Utility.DeleteRowFromTable(TableName, cmd, WhereCondition);
            }
            catch (Exception)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                throw;
            }
            finally
            {
                if (AutoCommit)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public int Count<T>(string WhereCondition = null, WithInQuery withInQuery = WithInQuery.None) where T : class, new()
        {
            var TableName = Utility.GetTableName<T>();
            var withString = Utility.GetWithString(withInQuery);
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {

                string CommandText = @"SELECT COUNT(*) AS TotalRecords FROM " + TableName + " t " + withString + " " + WhereCondition;
                SqlCommand cmd = new SqlCommand(CommandText, connection);

                try
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var t = reader.GetInt32("TotalRecords");
                            if (connection.State == ConnectionState.Open)
                            {
                                connection.Close();
                            }
                            return t;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    throw;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }

                return 0;
            }
        }

        public dynamic Query(string Query, bool AutoCommit = true, ExecuteType executeType = ExecuteType.ExecuteReader)
        {
            SqlConnection connection;

            if (AutoCommit)
            {
                connection = new SqlConnection(ConnectionString);
            }
            else
            {
                SetUpConnectionAndTransactionToSave();
                connection = ConnectionToSave;
            }

            var ret = new List<dynamic>();
            string CommandText = Query;

            SqlCommand cmd = new SqlCommand(CommandText, connection);

            if (!AutoCommit)
            {
                cmd.Transaction = TransactionToSave;
            }

            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                switch (executeType)
                {
                    case ExecuteType.ExecuteReader:
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var t = Utility.GetFullQueryRow(reader);
                                ret.Add(t);
                            }
                        }
                        return ret;
                    case ExecuteType.ExecuteNonQuery:
                        var recs = cmd.ExecuteNonQuery();
                        if (recs > 0)
                        {
                            if (AutoCommit)
                            {
                                if (connection.State == ConnectionState.Open)
                                {
                                    connection.Close();
                                }
                            }
                            return recs;
                        }
                        return null;
                    default:
                        return null;
                }
            }
            catch (Exception)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                throw;
            }
            finally
            {
                if (AutoCommit)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }
    }
}
