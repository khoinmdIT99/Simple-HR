using System;

namespace HC_WEB_FINALPROJECT.Models
{
    public class Employee
    {
        public int Id {get; set;}
        public string Image { get; set; }
        public string Name { get; set; }
        public string Email {get; set;}
        public string Phone {get; set;}
        public string Gender {get; set;}
        public DateTime BirthDate {get; set;}
        public string BirthPlace {get; set;}
        public string Occupation {get; set;}
        public string Placement {get; set;}
        public string Status {get; set;}
        public string Address {get; set;}
        public string EmergencyContact1 {get; set;}
        public string Phone1 {get; set;}
        public string EmergencyContact2 {get; set;}
        public string Phone2 {get; set;}
        public string EmergencyContact3 {get; set;}
        public string Phone3 {get; set;}
    }
}