using GoogleSheetsCrudLibrary;
using ToRunProject;

var SheetId = "1e3XFN67BJBSsJjxuCnrMYUafCIqy2m1iQT2K0CfT4OM";
var ClientSecretsJSON = @"
    {
      ""type"": ""service_account"",
      ""project_id"": ""cypherkeeper"",
      ""private_key_id"": ""5cac8d1b438968109c8d287d077e3513f71035e5"",
      ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDFRbtDBDbUx9jZ\nRKjyLkWn84eBU48gB8Z5WpVqTrYlUWuz+pglN6MR4I2CLaPZcQexHn9gaAuslJql\nYRMOgIEK9kMR8nx4ilMCfO42WJFUfTzuxTVv55Ebq69ImVMiCWvqLEg8OFXweLdg\n7Izb7OjeZXh+7Z09R72DjLqhrIc0sKUDkGWyDBysBcO3RWe28i9B0o4Z30CV5kgy\nRtEB5QW3Z+2Nc3wM7Y0uQDw7QiNctNB5LhsN/RAzKRT1uPoRdrWdfEgX7RYN3F24\nCDzy0RfaWCpI0MELXbQBeNqFGTZq4kXyVcQbmQ82oAauBCc8boQR7QbZjeWj2VQd\nMT++oU4RAgMBAAECggEAAJqhY30keyukJat+JkUEqupQpJFlmMrX+BB88KG5EQ2E\nxOekMPOl/iyv15GINObnB8sjcgofhFxtdMrX921momqf80geIB2xpLT5OGq2VzSN\noTWIClsCfhEygjMQpsQHSTpFe68NzvoE+w5TssCw8y1gXGh3Lh7ax7JV8p9ynTNZ\nN6kcwz1+VVuaf/P2nU4N9J1TX4FF6F/QrXTHdCCyyUqPBsxuXmiDIY2g2yUMuf2y\nh4K7BMdrvoYKq4WW0w46q//3HoIQCtBlBuNHuCk5CkGZ7vnxAjoEz5R5qZ1NBDOX\nNjkoa3uTDOcdij9YZeTo/guoTaP5o5VS9iHKHNdIOQKBgQDvV9KXSWf46VK9HEWH\nRPBzJ8ODCKzeEs3qhUJOCYlUcCxBoVdvkGl+45bl03d0/wNDwS5ubWFxPSoMfRz1\nG4KQ8hAr9R3Vj6AY3QSV86IQ4K+2GMfB4/DHZuI6eMVAsEp+3I/axyHKffIn6SLR\nN3k34PMnjn9XboMPlM9nlBPjTwKBgQDTAF7cat2qwRQqCqyni4TN9Y3IhOQJqN7k\nIjqiIday/hNK71pYz5dawMMuS9h0Dg5wz+gJG16RbH0VyRN/deKhZpHOIwC6smiy\non1ZdsChlHsip0OzPsd0WE9MlbrqdO2TcSe/sFO3x8bR+VUH5KvfzzC1qfUb9FIr\ntjOa+f7gnwKBgCdiLCT4z0MGIbc8j7RSg75vLJzDMK7aKUgN5Xsx1ocubhnekqSo\n/rMCgDIROfbmf4MGpr1FAmO/zMuBSuKKRFHVgxO4odAichWlSLvj+ycL0d4E9dnR\nyC0HoPI+LTcXyypU4Nb/LXBEiTRICJKkri2jtl8r84hPhwFpWzeQBjXBAoGAWKkl\nMxUsg52R7YCCv/weF7UAmKtObsior8+6N3pkjt24GZqtqffAr3+kBAYWs91cMAcM\nhxcbC5w8izTsnnhYfF1vEuEeY1SoLbedWERP+RU9EHicN+s/QCIDYDjiS7ZKRdQc\nUa2z39twLbbJBgIOPoL/Arahqddl17w9vvchJB0CgYEAt/j26/cKaAN7FuAaCrTY\nBz+YYlmiDJimKbenedY1cftChpqnlQ/nD2qQq/cBiXxdZ88mi7qbH+8gs9cvcFDY\nRsBETMNCIk4i3YiFRmPdsap6HJspfxUh0vdbVSqrWBItrOgmAlB8hnQjMFR6U5Hl\nnO/Jy6ZvDEh637C2VkIdpkg=\n-----END PRIVATE KEY-----\n"",
      ""client_email"": ""cypherkeeper@cypherkeeper.iam.gserviceaccount.com"",
      ""client_id"": ""116077985974182289714"",
      ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
      ""token_uri"": ""https://oauth2.googleapis.com/token"",
      ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
      ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/cypherkeeper%40cypherkeeper.iam.gserviceaccount.com"",
      ""universe_domain"": ""googleapis.com""
    }
";

var _GoogleSheetsCrud = new GoogleSheetsCrud(SheetId, "TEST", ClientSecretsJSON);

_GoogleSheetsCrud.GetList<tbGroupsModel>();
//var model = new TitlesModel()
//{
//    Series_Id = "Series_Id_Update",
//    Series_MainName = "Series_MainName_Update",
//    Series_AltNames = "Series_AltNames_Update",
//    Series_Genre = "Series_Genre_Update",
//    Series_ReleaseYear = "Series_ReleaseYear_Update",
//    Series_Poster = "Series_Poster_Update",
//    Series_ExtraInformation = "Series_ExtraInformation_Update",
//    Series_IMDB_ID_TAG_ID = "Series_IMDB_ID_TAG_ID_Update",
//    Series_Ver_Poster = "Series_Ver_Poster_Update",
//};
////_GoogleSheetsCrud.Add(model);

//var rowIndex = _GoogleSheetsCrud.GetRowNumbers<TitlesModel>("Series_Id", "219");
//Console.WriteLine(rowIndex.ToString());
//_GoogleSheetsCrud.Update(model, rowIndex);
//_GoogleSheetsCrud.Delete<TitlesModel>(rowIndex);