using FastMember;
using GoogleSheetsCrudLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleSheetsCrudLibrary
{
    public class Utility
    {
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

        public static RowStartEndNumberModel GetStartAndEndRowNumber(int? RowCount, int page = -1, int pageSize = -1)
        {
            if (RowCount == null) { return new RowStartEndNumberModel() { Start = 0, End = 0, }; }

            if (page == -1 || pageSize == -1) { return new RowStartEndNumberModel() { Start = 2, End = RowCount ?? 0, }; }

            var _RowStartEndNumberModel = new RowStartEndNumberModel()
            {
                Start = ((page - 1) * pageSize) + 2,
                End = (page * pageSize) + 1,
            };

            if (_RowStartEndNumberModel.End > RowCount)
            {
                _RowStartEndNumberModel.End = RowCount ?? 0;
            }

            return _RowStartEndNumberModel;
        }

        public static T ConvertToSheetsDataObject<T>(List<object> TitleData, List<List<object>> BodyData)
        {
            var SheetsData = new List<dynamic>();

            Type type = typeof(List<dynamic>);
            var accessor = TypeAccessor.Create(type);

            for (var BodyIndex = 0; BodyIndex < BodyData.Count; BodyIndex++)
            {
                dynamic SheetData = new System.Dynamic.ExpandoObject();
                for (var TitleIndex = 0; TitleIndex < TitleData.Count; TitleIndex++)
                {
                    var Prop = TitleData[TitleIndex].ToString();
                    accessor[SheetData, Prop] = BodyData[BodyIndex][TitleIndex].ToString();
                }
                SheetsData.Add(SheetData);
            }
            return SheetsData;
        }
    }
}
