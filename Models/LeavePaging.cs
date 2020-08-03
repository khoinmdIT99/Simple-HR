using System;

namespace HC_WEB_FINALPROJECT.Models
{
    public class LeavePaging
    {
        public int Id { get; set; }
        public string Search { get; set; }
        public int ShowItem { get; set; }
        public string StatusPage { get; set; }
        public int CurentPage { get; set; }
    }
}