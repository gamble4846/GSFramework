using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using GoogleSheetsCrudLibrary.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            if(CurrentSheetData == null)
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

            //var request = _SheetsService.Spreadsheets.Values.Get(SpreadsheetId, );
            return new List<T>();
        }

        //public bool UpdateDataTableFromSpreadsheet()
        //{
        //    var request = _SheetsService.Spreadsheets.Get(SpreadsheetId);
        //    var response = request.Execute();
        //    return false;
        //}
    }
}
