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

namespace HC_WEB_FINALPROJECT.Controllers {
    public class LeaveRequestController : Controller {
        private AppDbContext _AppDbContext;
        private readonly ILogger<LeaveRequestController> _logger;

        public LeaveRequestController (ILogger<LeaveRequestController> logger, AppDbContext appDbContext) {
            _AppDbContext = appDbContext;
            _logger = logger;
        }

        [Authorize]
        public IActionResult LeaveRequestList (string status = "pending", int _crntpage = 1) {
               if(!_AppDbContext.LeavePagings.Any()){
                var obj = new LeavePaging{
                    StatusPage = "Unproccess",
                    CurentPage = 1,
                    Search = null,
                    ShowItem = 6
                };
                _AppDbContext.LeavePagings.Add(obj);
                _AppDbContext.SaveChanges();
            }
            var pagesetting = _AppDbContext.LeavePagings.Find (1);
            pagesetting.Search = "";
            pagesetting.StatusPage = status;
            pagesetting.CurentPage = _crntpage;
            _AppDbContext.SaveChanges ();
            if (pagesetting.CurentPage == 1) {
                var take = pagesetting.ShowItem;
                var SpesificLeave = from a in _AppDbContext.LeaveRequests where a.status == pagesetting.StatusPage select a;
                var get = from a in SpesificLeave.Take (take) where a.status == pagesetting.StatusPage select a;
                ViewBag.items = get;
                ViewBag.page = pagesetting;
            } else {
                var take = pagesetting.ShowItem;
                var SpesificLeave = from a in _AppDbContext.LeaveRequests where a.status == pagesetting.StatusPage select a;
                var get = from a in SpesificLeave.Skip (take * (pagesetting.CurentPage - 1)).Take (take) select a;
                ViewBag.items = get;
                ViewBag.page = pagesetting;
            }
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return View ();
        }

        [Authorize]
        public IActionResult Approve (int Id) {
            var request = _AppDbContext.LeaveRequests.Find (Id);
            request.status = "approve";
            _AppDbContext.SaveChanges ();
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return (RedirectToAction("LeaveRequestList", new { status = request.status }));
        }

        [Authorize]
        public IActionResult Reject (int Id) {
            var reject = _AppDbContext.LeaveRequests.Find (Id);
            reject.status = "reject";
            _AppDbContext.SaveChanges ();
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return (RedirectToAction("LeaveRequestList", new { status = reject.status }));
        }

        [Authorize]
        public IActionResult Search (string keyword) {
            var paging = _AppDbContext.LeavePagings.Find (1);
            paging.Search = keyword;
            _AppDbContext.SaveChanges ();
            var get = from a in _AppDbContext.LeaveRequests where (a.status == paging.StatusPage) && a.EmployeeName.Contains (keyword) || a.Remarks.Contains (keyword) || a.EmployeeDepartment.Contains (keyword) || a.EmployeeOccupation.Contains (keyword) || a.EmployeeID.Contains (keyword) select a;
            ViewBag.items = get;
            ViewBag.page = paging;
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return View ("LeaveRequestList");
        }

        [Authorize]
        public IActionResult Remove (int Id) {
            var rmv = _AppDbContext.LeaveRequests.Find (Id);
            var stat = rmv.status;
            _AppDbContext.LeaveRequests.Remove (rmv);
            _AppDbContext.SaveChanges ();
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return (RedirectToAction("LeaveRequestList", new { status = stat }));
        }

        [Authorize]
        public IActionResult ExportAll () {
            var comlumHeadrs = new string[] {
                "Id",
                "Employee ID",
                "Employee Name",
                "Department",
                "Occupation",
                "Leave Start",
                "Leave End",
                "Remarks",
                "Status"
            };
            var items = (from item in _AppDbContext.LeaveRequests select new object[] {
                item.Id,
                    $"{item.EmployeeID}",
                    $"{item.EmployeeName}",
                    $"{item.EmployeeDepartment}",
                    $"{item.EmployeeOccupation}",
                    $"{item.Start}",
                    $"{item.End}",
                    $"{item.Remarks}",
                    $"{item.status}"
            }).ToList ();

            var itemcsv = new StringBuilder ();
            items.ForEach (line => {
                itemcsv.AppendLine (string.Join (",", line));
            });

            byte[] buffer = Encoding.ASCII.GetBytes ($"{string.Join(",", comlumHeadrs)}\r\n{itemcsv.ToString()}");
            return File (buffer, "text/csv", $"AllLeaveRequest.csv");
        }

        [Authorize]
        public IActionResult ExportCSV () {
            var paging = _AppDbContext.LeavePagings.Find (1);
            var comlumHeadrs = new string[] {
                "Id",
                "Employee ID",
                "Employee Name",
                "Department",
                "Occupation",
                "Leave Start",
                "Leave End",
                "Remarks",
                "Status"
            };
            var items = new List<object[]> ();
            if (paging.Search == null) {
                items = (from item in _AppDbContext.LeaveRequests where item.status == paging.StatusPage select new object[] {
                    item.Id,
                        $"{item.EmployeeID}",
                        $"{item.EmployeeName}",
                        $"{item.EmployeeDepartment}",
                        $"{item.EmployeeOccupation}",
                        $"{item.Start}",
                        $"{item.End}",
                        $"{item.Remarks}",
                        $"{item.status}"
                }).ToList ();
            } else if (paging.Search != null) {
                items = (from item in _AppDbContext.LeaveRequests where item.status == paging.StatusPage && (item.EmployeeName.Contains (paging.Search) || item.EmployeeDepartment.Contains (paging.Search) || item.EmployeeOccupation.Contains (paging.Search) || item.EmployeeID.Contains (paging.Search) || item.Remarks.Contains (paging.Search)) select new object[] {
                    item.Id,
                        $"{item.EmployeeID}",
                        $"{item.EmployeeName}",
                        $"{item.EmployeeDepartment}",
                        $"{item.EmployeeOccupation}",
                        $"{item.Start}",
                        $"{item.End}",
                        $"{item.Remarks}",
                        $"{item.status}"
                }).ToList ();
            }

            var itemcsv = new StringBuilder ();
            items.ForEach (line => {
                itemcsv.AppendLine (string.Join (",", line));
            });

            byte[] buffer = Encoding.ASCII.GetBytes ($"{string.Join(",", comlumHeadrs)}\r\n{itemcsv.ToString()}");
            return File (buffer, "text/csv", $"LeaveRequest.csv");
            // return RedirectToAction("ListEmployees","Employees");
        }

        public IActionResult AddData (string remark, DateTime start, DateTime end, int EmployeeId) {
            var get_employee = _AppDbContext.Employee.Find (EmployeeId);
            var obj = new LeaveRequest () {
                EmployeeID = (get_employee.Id).ToString (),
                Remarks = remark,
                status = "pending",
                EmployeeDepartment = get_employee.Placement,
                EmployeeName = get_employee.Name,
                EmployeeOccupation = get_employee.Occupation,
                Start = start,
                End = end
            };
            _AppDbContext.LeaveRequests.Add (obj);
            _AppDbContext.SaveChanges ();
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return RedirectToAction ("LeaveRequestList", "LeaveRequest");
        }
        public IActionResult AddLeaveRequest () {
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return View ();
        }
        public IActionResult Privacy () {
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return View ();
        }

        [ResponseCache (Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error () {
            return View (new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}