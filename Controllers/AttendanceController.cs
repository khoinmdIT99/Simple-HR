using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HC_WEB_FINALPROJECT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HC_WEB_FINALPROJECT.Controllers
{
    public class AttendanceController : Controller
    {
        private AppDbContext _AppDbContext;
        private readonly ILogger<HomeController> _logger;

        public AttendanceController(ILogger<HomeController> logger, AppDbContext appDbContext)
        {
            _AppDbContext = appDbContext;
            _logger = logger;
        }

        [Authorize]
        public IActionResult AttendanceList(int Id)
        {
            var get_Attendance = from u in _AppDbContext.Attendances where u.EmployeeId == Id.ToString() select u;
            ViewBag.Attendance = get_Attendance;
            var get_employee = _AppDbContext.Employee.Find(Id);
            ViewBag.employee = get_employee;
            var now = DateTime.Now;
            var get = now.AddDays(-7);
            ViewBag.items = get;
            var leavereq = from a in _AppDbContext.LeaveRequests
                           where a.status == "pending"
                           select a;
            var countReq = leavereq.Count();
            ViewBag.Req = countReq;
            return View("AttendanceList");
        }

        //  [Authorize]
        // public IActionResult AttendanceHome () {
        //     var get_employee = from a in _AppDbContext.Employee select a;
        //     ViewBag.items = get_employee;
        //     var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
        //     select a;
        //     var countReq = leavereq.Count ();
        //     ViewBag.Req = countReq;
        //     return View ("AttendanceHome");
        // }

        // [Authorize]
        // public IActionResult Search(string keyword)
        // {
        //     var get = from a in _AppDbContext.Employee where (a.Name.Contains(keyword) || a.Phone.Contains(keyword) || a.Address.Contains(keyword) || a.Email.Contains(keyword) || a.Occupation.Contains(keyword) || a.Placement.Contains(keyword)) select a;
        //     ViewBag.items = get;
        //     var leavereq = from a in _AppDbContext.LeaveRequests
        //                    where a.status == "pending"
        //                    select a;
        //     var countReq = leavereq.Count();
        //     ViewBag.Req = countReq;
        //     return View("AttendanceHome");
        // }

        [Authorize]
        public IActionResult AttendanceThisMonth(int Id)
        {
            var get_Attendance = from u in _AppDbContext.Attendances where u.EmployeeId == Id.ToString() select u;
            ViewBag.Id = Id;
            ViewBag.Attendance = get_Attendance;
            var get_employee = _AppDbContext.Employee.Find(Id);
            ViewBag.Employee = get_employee;
            var leavereq = from a in _AppDbContext.LeaveRequests
                           where a.status == "pending"
                           select a;
            var countReq = leavereq.Count();
            ViewBag.Req = countReq;
            return View();
        }

        [Authorize]
        public IActionResult Create_ClockIn(int Id, string Remark)
        {
            var employee = _AppDbContext.Employee.Find(Id);
            var clckin = DateTime.Now;
            var spesific_clockin = from a in _AppDbContext.Attendances where ((a.ClockIn.Day == clckin.Day && a.ClockIn.Month == clckin.Month && a.ClockIn.Year == clckin.Year) && (a.EmployeeId == (employee.Id).ToString())) select a;
            if (!spesific_clockin.Any())
            {
                var obj = new Attendance()
                {
                    ClockIn = DateTime.Now,
                    Remarks_in = Remark,
                    EmployeeId = Id.ToString(),
                    status = "In"
                };
                _AppDbContext.Attendances.Add(obj);
                _AppDbContext.SaveChanges();
            }
            var leavereq = from a in _AppDbContext.LeaveRequests
                           where a.status == "pending"
                           select a;
            var countReq = leavereq.Count();
            ViewBag.Req = countReq;
            return (RedirectToAction("AttendanceList", new { Id = Id }));
        }

        [Authorize]
        public IActionResult Create_ClockOut(int Id, string Remark)
        {
            var employee = _AppDbContext.Employee.Find(Id);
            var clckout = DateTime.Now;
            var dt = new DateTime(2020, 1, 1, 17, 00, 00);
            if (clckout.TimeOfDay < dt.TimeOfDay)
            {
                Console.WriteLine("kuramg dari");
            }
            Console.WriteLine(clckout.TimeOfDay);
            Console.WriteLine("ini clockout");
            var spesific_clockin = from a in _AppDbContext.Attendances where ((a.ClockIn.Day == clckout.Day && a.ClockIn.Month == clckout.Month && a.ClockIn.Year == clckout.Year) && (a.EmployeeId == (employee.Id).ToString())) select a;
            if (spesific_clockin.Any())
            {
                var obj = spesific_clockin.First();
                obj.ClockOut = clckout;
                obj.Remarks_out = Remark;
                obj.status = "Success";
                _AppDbContext.SaveChanges();
            }
            var leavereq = from a in _AppDbContext.LeaveRequests
                           where a.status == "pending"
                           select a;
            var countReq = leavereq.Count();
            ViewBag.Req = countReq;
            return (RedirectToAction("AttendanceList", new { Id = Id }));

        }

        [Authorize]
        public IActionResult ClockIn(int Id)
        {
            var get = _AppDbContext.Employee.Find(Id);
            ViewBag.Employee = get;
            var leavereq = from a in _AppDbContext.LeaveRequests
                           where a.status == "pending"
                           select a;
            var countReq = leavereq.Count();
            ViewBag.Req = countReq;
            return View();
        }


        [Authorize]
        public IActionResult Export(string Id)
        {
            var comlumHeadrs = new string[] {
                "Name",
                "Date",
                "Clock In",
                "Clock Out",
                "Status"
            };
            var getemployee = _AppDbContext.Employee.Find(Convert.ToInt32(Id));
            var items = (from item in _AppDbContext.Attendances
                         where item.EmployeeId == Id
                         select new object[] {
                    $"{getemployee.Name}",
                    $"{item.ClockIn.ToString("dd MMMM yyyy")}",
                    $"{item.ClockIn.ToString("hh:mm:ss tt")}",
                    $"{item.ClockOut.ToString("hh:mm:ss tt")}",
                    $"{item.status}"
            }).ToList();

            var itemcsv = new StringBuilder();
            items.ForEach(line =>
            {
                itemcsv.AppendLine(string.Join(",", line));
            });

            byte[] buffer = Encoding.ASCII.GetBytes($"{string.Join(",", comlumHeadrs)}\r\n{itemcsv.ToString()}");
            var leavereq = from a in _AppDbContext.LeaveRequests
                           where a.status == "pending"
                           select a;
            var countReq = leavereq.Count();
            ViewBag.Req = countReq;
            return File(buffer, "text/csv", $"Attendance.csv");
        }


        [Authorize]
        public IActionResult ClockOut(int Id)
        {
            var get = _AppDbContext.Employee.Find(Id);
            ViewBag.Employee = get;
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending" select a;
            var countReq = leavereq.Count();
            ViewBag.Req = countReq;
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            var leavereq = from a in _AppDbContext.LeaveRequests
                           where a.status == "pending"
                           select a;
            var countReq = leavereq.Count();
            ViewBag.Req = countReq;
            return View();
        }

        [Authorize]
        public IActionResult Search(string keyword)
        {
            var paging = _AppDbContext.AttendancesPagings.Find(1);
            paging.Search = keyword;
            _AppDbContext.SaveChanges();
            if (paging.StatusPage == "Employee")
            {
                var get = from a in _AppDbContext.Employee where (a.Name.Contains(keyword) || a.Phone.Contains(keyword) || a.Address.Contains(keyword) || a.Email.Contains(keyword) || a.Occupation.Contains(keyword) || a.Placement.Contains(keyword)) select a;
                ViewBag.items = get;
                ViewBag.page = paging;
                var leavereq = from a in _AppDbContext.LeaveRequests
                               where a.status == "pending"
                               select a;
                var countReq = leavereq.Count();
                ViewBag.Req = countReq;
                return View("AttendanceHome");
            }
            else
            {
                var today_attendance = from a in _AppDbContext.Attendances where a.ClockIn.Date == DateTime.Now.Date select a.EmployeeId;
                List<int> EmployeeId = new List<int>();
                List<string> EmployeeName = new List<string>();
                foreach (var item in today_attendance)
                {
                    EmployeeId.Add(Convert.ToInt32(item));
                }
                for (int i = 0; i < EmployeeId.Count; i++)
                {
                    var getnama = _AppDbContext.Employee.Find(EmployeeId[i]);
                    EmployeeName.Add(getnama.Name);
                }
                var show = from a in EmployeeName where a.Contains(keyword) select a;
                var set_page = _AppDbContext.AttendancesPagings.Find(1);
                ViewBag.items = show;
                ViewBag.page = set_page;
                var leavereq1 = from a in _AppDbContext.LeaveRequests
                                where a.status == "pending"
                                select a;
                var countReq1 = leavereq1.Count();
                ViewBag.Req = countReq1;
            }
            var leavereq2 = from a in _AppDbContext.LeaveRequests
                            where a.status == "pending"
                            select a;
            var countReq2 = leavereq2.Count();
            ViewBag.Req2 = countReq2;
            return View("AttendanceHome");
        }

        [Authorize]
        public IActionResult AttendanceHome(string status = "Today", int _crntpage = 1)
        {
            if(!_AppDbContext.AttendancesPagings.Any()){
                var obj = new AttendancePaging{
                    StatusPage = "Today",
                    CurentPage = 1,
                    Search = null,
                    ShowItem = 6
                };
                _AppDbContext.AttendancesPagings.Add(obj);
                _AppDbContext.SaveChanges();
            }
            var get_employee = from a in _AppDbContext.Employee select a;
            ViewBag.items = get_employee;
            var set_page = _AppDbContext.AttendancesPagings.Find(1);
            set_page.Search = "";
            set_page.StatusPage = status;
            set_page.CurentPage = _crntpage;
            _AppDbContext.SaveChanges();
            if (set_page.StatusPage == "Today")
            {
                if (set_page.CurentPage == 1)
                {
                    var today_attendance = from a in _AppDbContext.Attendances where a.ClockIn.Date == DateTime.Now.Date select a.EmployeeId;
                    List<int> EmployeeId = new List<int>();
                    List<string> EmployeeName = new List<string>();
                    foreach (var item in today_attendance)
                    {
                        EmployeeId.Add(Convert.ToInt32(item));
                    }
                    for (int i = 0; i < EmployeeId.Count; i++)
                    {
                        var getnama = _AppDbContext.Employee.Find(EmployeeId[i]);
                        EmployeeName.Add(getnama.Name);
                    }
                    var show = EmployeeName.Take(set_page.ShowItem);
                    ViewBag.items = show;
                    ViewBag.page = set_page;
                    var leavereq1 = from a in _AppDbContext.LeaveRequests
                                    where a.status == "pending"
                                    select a;
                    var countReq1 = leavereq1.Count();
                    ViewBag.Req = countReq1;
                    return View("AttendanceHome");
                }
                else
                {
                    var today_attendance = from a in _AppDbContext.Attendances where a.ClockIn.Date == DateTime.Now.Date select a.EmployeeId;
                    List<int> EmployeeId = new List<int>();
                    List<string> EmployeeName = new List<string>();
                    foreach (var item in today_attendance)
                    {
                        EmployeeId.Add(Convert.ToInt32(item));
                    }
                    for (int i = 0; i < EmployeeId.Count; i++)
                    {
                        var getnama = _AppDbContext.Employee.Find(EmployeeId[i]);
                        EmployeeName.Add(getnama.Name);
                    }
                    var take = set_page.ShowItem;
                    var show = EmployeeName.Skip(take * (set_page.CurentPage - 1)).Take(take);
                    ViewBag.items = show;
                    ViewBag.page = set_page;
                    var leavereq2 = from a in _AppDbContext.LeaveRequests
                                    where a.status == "pending"
                                    select a;
                    var countReq2 = leavereq2.Count();
                    ViewBag.Req = countReq2;
                    return View("AttendanceHome");
                }
            }
            else if (set_page.StatusPage == "Employee")
            {
                if (set_page.CurentPage == 1)
                {
                    var take = set_page.ShowItem;
                    var spesific_employee = (from a in _AppDbContext.Employee select a).Take(take);
                    ViewBag.items = spesific_employee;
                    ViewBag.page = set_page;
                    var leavereq3 = from a in _AppDbContext.LeaveRequests
                                    where a.status == "pending"
                                    select a;
                    var countReq3 = leavereq3.Count();
                    ViewBag.Req = countReq3;
                    return View("AttendanceHome");
                }
                else
                {
                    var take = set_page.ShowItem;
                    var spesific_employee = from a in _AppDbContext.Employee select a;
                    var get = from a in spesific_employee.Skip(take * (set_page.CurentPage - 1)).Take(take) select a;
                    ViewBag.items = get;
                    ViewBag.page = set_page;
                    var leavereq4 = from a in _AppDbContext.LeaveRequests
                                    where a.status == "pending"
                                    select a;
                    var countReq4 = leavereq4.Count();
                    ViewBag.Req = countReq4;
                    return View("AttendanceHome");
                }
            }
            var leavereq = from a in _AppDbContext.LeaveRequests
                           where a.status == "pending"
                           select a;
            var countReq = leavereq.Count();
            var set_page_ = _AppDbContext.AttendancesPagings.Find(1);
            ViewBag.page = set_page_;
            ViewBag.Req = countReq;
            return View("AttendanceHome");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}