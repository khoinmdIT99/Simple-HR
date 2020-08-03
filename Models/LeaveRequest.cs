using System;

namespace HC_WEB_FINALPROJECT.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeDepartment { get; set; }
        public string EmployeeOccupation { get; set; }
        public string Remarks { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string status { get; set; }
    }
}