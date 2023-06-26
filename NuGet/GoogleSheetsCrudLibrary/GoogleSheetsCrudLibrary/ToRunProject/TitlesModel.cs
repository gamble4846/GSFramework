using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToRunProject
{
    [Table("Titles")]
    public class TitlesModel
    {
        public string? Series_Id { get; set; }
        public string? Series_MainName { get; set; }
        public string? Series_AltNames { get; set; }
        public string? Series_Genre { get; set; }
        public string? Series_ReleaseYear { get; set; }
        public string? Series_Poster { get; set; }
        public string? Series_ExtraInformation { get; set; }
        public string? Series_IMDB_ID_TAG_ID { get; set; }
        public string? Series_Ver_Poster { get; set; }
    }
}
