﻿using FastMember;
using Google.Apis.Sheets.v4.Data;
using GoogleSheetsCrudLibrary.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
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

        public static List<T> ConvertToSheetsDataObject<T>(List<object> TitleData, List<List<object>> BodyData) where T : class, new()
        {
            if(BodyData == null)
            {
                return new List<T>();
            }

            var FullSheetsData = new List<List<SheetEmptyModel>>();
            for (var BodyIndex = 0; BodyIndex < BodyData.Count; BodyIndex++)
            {
                var RowSheetsData = new List<SheetEmptyModel>();
                for (var TitleIndex = 0; TitleIndex < TitleData.Count; TitleIndex++)
                {
                    var FullSheetsData_ToSave = new SheetEmptyModel()
                    {
                        Title = TitleData[TitleIndex].ToString(),
                        Data = BodyData[BodyIndex][TitleIndex].ToString()
                    };
                    RowSheetsData.Add(FullSheetsData_ToSave);
                }
                FullSheetsData.Add(RowSheetsData);
            }

            Type type = typeof(T);
            var accessor = TypeAccessor.Create(type);
            var members = accessor.GetMembers();
            var SheetsData = new List<T>();
            for (var FullIndex = 0; FullIndex < FullSheetsData.Count; FullIndex++)
            {
                var RowSheetData = FullSheetsData[FullIndex];
                var t = new T();
                for (var RowIndex = 0; RowIndex < RowSheetData.Count; RowIndex++)
                {
                    var RowData = RowSheetData[RowIndex];
                    try
                    { 
                        var obj = new T();
                        Type _type = obj.GetType();
                        PropertyInfo prop = _type.GetProperty(RowData.Title);
                        switch (prop.PropertyType.ToString().ToUpper())
                        {
                            case "GUID":
                                    break;
                            case "DATETIME":
                                break;
                            default:
                                accessor[t, RowData.Title] = RowData.Data.ToString();
                                break;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                SheetsData.Add(t);
            }

            return SheetsData;
        }

        public static List<object> GetValuesListFromObject<T>(ValueRange TitleResponse, T model) where T : class, new()
        {
            var ValuesList = new List<object>();

            foreach (var CurrectField in TitleResponse.Values[0])
            {
                try
                {
                    ValuesList.Add((model.GetType().GetProperty(CurrectField.ToString()).GetValue(model, null)) ?? "");
                }
                catch (Exception)
                {
                    ValuesList.Add("");
                }
            }

            return ValuesList;
        }

        public static string GetColumnName(int columnNumber)
        {
            string columnName = "";

            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }

            return columnName;
        }
    }
}
