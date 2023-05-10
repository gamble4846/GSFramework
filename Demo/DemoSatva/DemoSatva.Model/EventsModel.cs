using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using EasyCRUD.ADO;
using System.ComponentModel.DataAnnotations.Schema;
using DemoSatva.Utility;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

namespace DemoSatva.Model
{
    [Table("Events")]
    public class EventsModel
    {
        public int Id { get; set; }
        [Required]
        public string EventTitle { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public string EventDescription { get; set; }
        [Range(int.MinValue, int.MaxValue)]
        public int EventPriority { get; set; }
    }
}

