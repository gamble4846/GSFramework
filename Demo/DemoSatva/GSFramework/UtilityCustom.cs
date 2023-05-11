using FastMember;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using static EasyCrudDB.GSEnums;
using Newtonsoft.Json;

namespace EasyCrudDB
{
    public static class UtilityCustom
    {
        public static T ConvertReaderToObject<T>(this SqlDataReader rd) where T : class, new()
        {
            Type type = typeof(T);
            var accessor = TypeAccessor.Create(type);
            var members = accessor.GetMembers();
            var t = new T();
            try
            {
                for (int i = 0; i < rd.FieldCount; i++)
                {
                    if (!rd.IsDBNull(i))
                    {
                        string fieldName = rd.GetName(i);

                        if (members.Any(m => string.Equals(m.Name, fieldName, StringComparison.OrdinalIgnoreCase)))
                        {
                            accessor[t, fieldName] = rd.GetValue(i);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var x = ex;
            }


            return t;
        }

        public static dynamic GetFullQueryRow(SqlDataReader rd)
        {
            dynamic RowData = new System.Dynamic.ExpandoObject();

            for (int i = 0; i < rd.FieldCount; i++)
            {
                var currentValue = rd.GetValue(i);
                var currentColumn = rd.GetName(i);
                var currentType = rd.GetFieldType(i);

                if (String.IsNullOrEmpty(currentValue.ToString()))
                {
                    ((IDictionary<String, Object>)RowData).Add(currentColumn, null);
                }
                else
                {
                    ((IDictionary<String, Object>)RowData).Add(currentColumn, currentValue);
                }
            }

            return RowData;
        }

        public static DataTable GetDataTableFromCommand(SqlCommand cmd)
        {
            DataTable dt = new DataTable();
            try
            {
                cmd.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }
            catch (Exception)
            {
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    cmd.Connection.Close();
                }
                throw;
            }
            finally
            {
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    cmd.Connection.Close();
                }
            }

            return dt;
        }

        public static T ToObject<T>(this DataRow dataRow) where T : new()
        {
            T item = new T();

            foreach (DataColumn column in dataRow.Table.Columns)
            {
                PropertyInfo property = GetProperty(typeof(T), column.ColumnName);

                if (property != null && dataRow[column] != DBNull.Value && dataRow[column].ToString() != "NULL")
                {
                    property.SetValue(item, ChangeType(dataRow[column], property.PropertyType), null);
                }
            }

            return item;
        }

        private static PropertyInfo GetProperty(Type type, string attributeName)
        {
            PropertyInfo property = type.GetProperty(attributeName);

            if (property != null)
            {
                return property;
            }

            return type.GetProperties()
                 .Where(p => p.IsDefined(typeof(DisplayAttribute), false) && p.GetCustomAttributes(typeof(DisplayAttribute), false).Cast<DisplayAttribute>().Single().Name == attributeName)
                 .FirstOrDefault();
        }

        public static object ChangeType(object value, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                return Convert.ChangeType(value, Nullable.GetUnderlyingType(type));
            }

            return Convert.ChangeType(value, type);
        }

        public static T DataTableToObject<T>(this DataTable dt) where T : new()
        {
            var dataRows = dt.AsEnumerable().ToList();
            return dataRows[0].ToObject<T>();
        }

        public static List<T> DataTableToObjectList<T>(this DataTable dt) where T : new()
        {
            var dataRows = dt.AsEnumerable().ToList();
            var ListObject = new List<T>();
            foreach (var dataRow in dataRows)
            {
                ListObject.Add(dataRow.ToObject<T>());
            }
            return ListObject;
        }

        public static string InsertRowToTable<T>(T data, string tableName, string OutputColumn, SqlCommand cmd, List<string> toIgnoreColumns = null) where T : new()
        {
            string OutputVAR = null;
            cmd.Parameters.Clear();
            Type type = typeof(T);
            var accessor = TypeAccessor.Create(type);
            var members = accessor.GetMembers();
            var t = new T();

            if (toIgnoreColumns == null)
            {
                toIgnoreColumns = new List<string>();
            }


            var ToUseMemeber = members.Where(x => !toIgnoreColumns.Contains(x.Name)).ToList();
            List<string> ColumnNames = ToUseMemeber.Select(x => x.Name).ToList();
            var QueryString = "DECLARE @OutputVAR TABLE (OutputColumn NVARCHAR(MAX)); INSERT INTO " + tableName + " ([" + string.Join("],[", ColumnNames.ToArray()) + "]) OUTPUT INSERTED." + OutputColumn + " INTO @OutputVAR VALUES (@" + string.Join(",@", ColumnNames.ToArray()) + "); SELECT * FROM @OutputVAR";
            cmd.CommandText = QueryString;
            foreach (var column in ColumnNames)
            {
                cmd.Parameters.AddWithValue("@" + column, data.GetType().GetProperty(column).GetValue(data));
            }

            foreach (SqlParameter parameter in cmd.Parameters)
            {
                if (parameter.Value == null)
                {
                    parameter.Value = DBNull.Value;
                }
            }


            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    OutputVAR = reader.GetString("OutputColumn");
                }
            }
            return OutputVAR;
        }

        public static string UpdateRowToTable<T>(T data, string tableName, SqlCommand cmd, string WhereCondition, List<string> toIgnoreColumns = null) where T : new()
        {
            try
            {
                cmd.Parameters.Clear();
                Type type = typeof(T);
                var accessor = TypeAccessor.Create(type);
                var members = accessor.GetMembers();
                var t = new T();

                if (toIgnoreColumns == null)
                {
                    toIgnoreColumns = new List<string>();
                }


                var ToUseMemeber = members.Where(x => !toIgnoreColumns.Contains(x.Name)).ToList();
                List<string> ColumnNames = ToUseMemeber.Select(x => x.Name).ToList();
                var QueryString = "UPDATE " + tableName + " SET ";

                foreach (var column in ColumnNames)
                {
                    QueryString += column + " = @" + column + ", ";
                }

                QueryString = QueryString.Substring(0, QueryString.Length - 2);

                QueryString += " " + WhereCondition;

                cmd.CommandText = QueryString;
                foreach (var column in ColumnNames)
                {
                    cmd.Parameters.AddWithValue("@" + column, data.GetType().GetProperty(column).GetValue(data));
                }

                foreach (SqlParameter parameter in cmd.Parameters)
                {
                    if (parameter.Value == null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                }

                var recs = cmd.ExecuteNonQuery();

                if (recs > 0)
                {
                    return true.ToString();
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string DeleteRowFromTable(string tableName, SqlCommand cmd, string WhereCondition)
        {
            try
            {
                string QueryString = "DELETE FROM " + tableName + " " + WhereCondition;
                cmd.CommandText = QueryString;
                var recs = cmd.ExecuteNonQuery();
                if (recs > 0)
                {
                    return true.ToString();
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string GetTableName<T>() where T : class, new()
        {
            Type modelType = typeof(T);
            try
            {
                TableAttribute tableAttribute = (TableAttribute)Attribute.GetCustomAttribute(modelType, typeof(TableAttribute));
                string tableName = tableAttribute.Name;
                return tableName;
            }
            catch
            {
                return modelType.Name;
            }
        }

        public static string GetFirstPropertyName<T>() where T : class, new()
        {
            Type modelType = typeof(T);
            var FirstProperty = modelType.GetProperties().FirstOrDefault();
            if (FirstProperty != null)
            {
                return FirstProperty.Name;
            }
            return "";
        }

        public static string GetWithString(WithInQuery withInQuery)
        {
            switch (withInQuery)
            {
                case WithInQuery.None:
                    return "";
                case WithInQuery.ReadPast:
                    return "WITH(readpast)";
                case WithInQuery.NoLock:
                    return "WITH(nolock)";
                default:
                    return "";
            }
        }

        public static T ConvertDynamicToType<T>(dynamic data) where T : new()
        {
            string jsonString = JsonConvert.SerializeObject(data);
            T result = JsonConvert.DeserializeObject<T>(jsonString);
            return result;
        }
    }
}
