using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleSheetsCrudLibrary.Models
{
    public class SheetDataModel
    {
        public string? SheetName { get; set; }
        public DataTable? SheetData { get; set; }
    }
}
