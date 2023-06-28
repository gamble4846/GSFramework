using FastMember;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using GoogleSheetsCrudLibrary.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;

namespace GoogleSheetsCrudLibrary
{
    public class GoogleSheetsCrud
    {
        string[] Scopes = { SheetsService.Scope.Spreadsheets };
        string ApplicationName = "";
        string SpreadsheetId = "";
        SheetsService _SheetsService;
        GoogleCredential _GoogleCredential;
        List<SheetDataModel> Sheets = new List<SheetDataModel>();


        public GoogleSheetsCrud(string spreadsheetId, string applicationName, string ClientSecretsJSON)
        {
            SpreadsheetId = spreadsheetId;
            ApplicationName = applicationName;

            _GoogleCredential = GoogleCredential.FromJson(ClientSecretsJSON).CreateScoped(Scopes);

            _SheetsService = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = _GoogleCredential,
                ApplicationName = ApplicationName,
            });
        }

        public List<T> GetList<T>(int page = -1, int itemsPerPage = -1) where T : class, new()
        {
            var SheetName = Utility.GetTableName<T>();
            var SheetsDataRequest = _SheetsService.Spreadsheets.Get(SpreadsheetId);
            var SheetsDataResponse = SheetsDataRequest.Execute();
            var CurrentSheetData = SheetsDataResponse.Sheets.Where(x => x.Properties.Title == SheetName).FirstOrDefault();

            if (CurrentSheetData == null)
            {
                throw new Exception($"{SheetName} - Not Found in Spreadsheet {SheetsDataResponse.Properties.Title}");
            }

            var TitleRange = $"{SheetName}!1:1";
            var TitleRequest = _SheetsService.Spreadsheets.Values.Get(SpreadsheetId, TitleRange);
            var TitleResponse = TitleRequest.Execute();

            var StartAndEndRowNumber = Utility.GetStartAndEndRowNumber(CurrentSheetData.Properties.GridProperties.RowCount, page, itemsPerPage);
            var BodyRange = $"{SheetName}!{StartAndEndRowNumber.Start}:{StartAndEndRowNumber.End}";
            var BodyRequest = _SheetsService.Spreadsheets.Values.Get(SpreadsheetId, BodyRange);
            var BodyResponse = BodyRequest.Execute();
            var BodyData = JsonConvert.DeserializeObject<List<List<object>>>(JsonConvert.SerializeObject(BodyResponse.Values));

            var FinalSheetData = Utility.ConvertToSheetsDataObject<T>(TitleResponse.Values[0].ToList(), BodyData);
            return FinalSheetData;
        }

        public AppendValuesResponse Add<T>(T model) where T : class, new()
        {
            var SheetName = Utility.GetTableName<T>();
            var SheetsDataRequest = _SheetsService.Spreadsheets.Get(SpreadsheetId);
            var SheetsDataResponse = SheetsDataRequest.Execute();
            var CurrentSheetData = SheetsDataResponse.Sheets.Where(x => x.Properties.Title == SheetName).FirstOrDefault();

            if (CurrentSheetData == null)
            {
                throw new Exception($"{SheetName} - Not Found in Spreadsheet {SheetsDataResponse.Properties.Title}");
            }

            var TitleRange = $"{SheetName}!1:1";
            var TitleRequest = _SheetsService.Spreadsheets.Values.Get(SpreadsheetId, TitleRange);
            var TitleResponse = TitleRequest.Execute();

            var ValueRangeData = new ValueRange();
            ValueRangeData.Values = new List<IList<object>> { Utility.GetValuesListFromObject(TitleResponse, model) };

            var appendRequest = _SheetsService.Spreadsheets.Values.Append(ValueRangeData, SpreadsheetId, TitleRange);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();

            return appendResponse;
        }

        public UpdateValuesResponse Update<T>(T model, int rowNumber) where T : class, new()
        {
            var SheetName = Utility.GetTableName<T>();
            var SheetsDataRequest = _SheetsService.Spreadsheets.Get(SpreadsheetId);
            var SheetsDataResponse = SheetsDataRequest.Execute();
            var CurrentSheetData = SheetsDataResponse.Sheets.Where(x => x.Properties.Title == SheetName).FirstOrDefault();

            if (CurrentSheetData == null)
            {
                throw new Exception($"{SheetName} - Not Found in Spreadsheet {SheetsDataResponse.Properties.Title}");
            }

            var TitleRange = $"{SheetName}!1:1";
            var TitleRequest = _SheetsService.Spreadsheets.Values.Get(SpreadsheetId, TitleRange);
            var TitleResponse = TitleRequest.Execute();

            var ValueRangeData = new ValueRange();
            ValueRangeData.Values = new List<IList<object>> { Utility.GetValuesListFromObject(TitleResponse, model) };

            var updateRange = $"{SheetName}!{rowNumber}:{rowNumber}";
            var updateRequest = _SheetsService.Spreadsheets.Values.Update(ValueRangeData, SpreadsheetId, updateRange);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var updateResponse = updateRequest.Execute();

            return updateResponse;
        }

        public ClearValuesResponse Delete<T>(int rowNumber) where T : class, new()
        {
            var SheetName = Utility.GetTableName<T>();
            var SheetsDataRequest = _SheetsService.Spreadsheets.Get(SpreadsheetId);
            var SheetsDataResponse = SheetsDataRequest.Execute();
            var CurrentSheetData = SheetsDataResponse.Sheets.Where(x => x.Properties.Title == SheetName).FirstOrDefault();

            if (CurrentSheetData == null)
            {
                throw new Exception($"{SheetName} - Not Found in Spreadsheet {SheetsDataResponse.Properties.Title}");
            }

            var requestBody = new ClearValuesRequest();
            var deleteRange = $"{SheetName}!{rowNumber}:{rowNumber}";
            var deleteRequest = _SheetsService.Spreadsheets.Values.Clear(requestBody, SpreadsheetId, deleteRange);
            var deleteResponse = deleteRequest.Execute();

            return deleteResponse;
        }

        public List<int> GetRowNumbers<T>(string ColumnName, string Value) where T : class, new()
        {
            var SheetName = Utility.GetTableName<T>();
            var SheetsDataRequest = _SheetsService.Spreadsheets.Get(SpreadsheetId);
            var SheetsDataResponse = SheetsDataRequest.Execute();
            var CurrentSheetData = SheetsDataResponse.Sheets.Where(x => x.Properties.Title == SheetName).FirstOrDefault();

            if (CurrentSheetData == null)
            {
                throw new Exception($"{SheetName} - Not Found in Spreadsheet {SheetsDataResponse.Properties.Title}");
            }

            var TitleRange = $"{SheetName}!1:1";
            var TitleRequest = _SheetsService.Spreadsheets.Values.Get(SpreadsheetId, TitleRange);
            var TitleResponse = TitleRequest.Execute();

            var ColumnIndex = TitleResponse.Values[0].IndexOf(ColumnName);
            if(ColumnIndex == -1)
            {
                throw new Exception($"{ColumnName} - Not Found in Sheet {SheetName},{SheetsDataResponse.Properties.Title}");
            }

            var GoogleSheetsColumnName = Utility.GetColumnName(ColumnIndex + 1);
            var StartAndEndRowNumber = Utility.GetStartAndEndRowNumber(CurrentSheetData.Properties.GridProperties.RowCount, -1, -1);
            var BodyRange = $"{SheetName}!{GoogleSheetsColumnName}1:{GoogleSheetsColumnName}{StartAndEndRowNumber.End}";
            var BodyRequest = _SheetsService.Spreadsheets.Values.Get(SpreadsheetId, BodyRange);
            var BodyResponse = BodyRequest.Execute();
            var BodyResponseValueSolo = new List<string>();
            foreach (var value in BodyResponse.Values)
            {
                BodyResponseValueSolo.Add(value[0].ToString() ?? "");
            }
            //var RowNumber = BodyResponseValueSolo.IndexOf(Value) + 1;
            //int[] allIndexes = BodyResponseValueSolo.ToArray().Find(s => s.Contains(value)).ToArray();
            var RowNumbers = BodyResponseValueSolo.Select((value, index) => new { value, index })
                    .Where(x => x.value == Value)
                    .Select(x => x.index)
                    .ToList();

            for(int i = 0; i < RowNumbers.Count; i++)
            {
                RowNumbers[i] = RowNumbers[i] + 1;
            }

            return RowNumbers;
        }
    }
}
