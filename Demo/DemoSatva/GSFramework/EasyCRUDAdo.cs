using DemoSatva.Utility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;
using System.Diagnostics;
using static EasyCRUD.ADO.GSEnums;

namespace EasyCRUD.ADO
{
    public class EasyCRUDAdo
    {
        private string ConnectionString = "";
        private SqlConnection ConnectionToSave = null;
        private SqlTransaction TransactionToSave = null;
        public EasyCRUDAdo(string connectionString)
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
                if(ConnectionToSave != null)
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
            if(ConnectionToSave == null || TransactionToSave == null)
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
            var TableName = UtilityCustom.GetTableName<T>();
            var FirstPropertyName = UtilityCustom.GetFirstPropertyName<T>();
            var withString = UtilityCustom.GetWithString(withInQuery);
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
                            var t = UtilityCustom.ConvertReaderToObject<T>(reader);
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
            var TableName = UtilityCustom.GetTableName<T>();
            var withString = UtilityCustom.GetWithString(withInQuery);
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
                            var t = UtilityCustom.ConvertReaderToObject<T>(reader);
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
            var TableName = UtilityCustom.GetTableName<T>();
            var FirstPropertyName = UtilityCustom.GetFirstPropertyName<T>();
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
                return UtilityCustom.InsertRowToTable(data, TableName, OutputColumn, cmd, toIgnoreColumnsList);
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
            var TableName = UtilityCustom.GetTableName<T>();
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
                return UtilityCustom.UpdateRowToTable(data, TableName, cmd, WhereCondition, toIgnoreColumnsList);
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
            var TableName = UtilityCustom.GetTableName<T>();

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
                return UtilityCustom.DeleteRowFromTable(TableName, cmd, WhereCondition);
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
