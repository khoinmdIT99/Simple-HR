using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HC_WEB_FINALPROJECT.Models
{
    public class Attendance
    {
        public int Id {get; set;}

        [ForeignKey("EmployeeId")]
        public string EmployeeId { get; set; }
        public DateTime ClockIn {get; set;}
        public DateTime ClockOut {get; set;}
        public string Remarks_in {get; set;}
        public string Remarks_out {get; set;}
        public string status {get; set;}
    }
}