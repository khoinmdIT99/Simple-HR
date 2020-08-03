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
using MimeKit;

namespace HC_WEB_FINALPROJECT.Controllers {
    public class EventController : Controller {
        private AppDbContext _AppDbContext;
        private readonly ILogger<EventController> _logger;

        public EventController (ILogger<EventController> logger, AppDbContext appDbContext) {
            _AppDbContext = appDbContext;
            _logger = logger;
        }


        [Authorize]
        public IActionResult Event () {
            var Event = from a in _AppDbContext.Events select a;
            ViewBag.Event = Event;
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return View ();
        }

        [Authorize]
        public IActionResult AddEventData (string name, DateTime date) {
            var obj = new Event{
                Name = name,
                day = date
            };
            _AppDbContext.Events.Add(obj);
            _AppDbContext.SaveChanges();
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return RedirectToAction ("Event","Event");
        }

        [Authorize]
        public IActionResult EditEvent (int Id) {
            var get = _AppDbContext.Events.Find(Id);
            ViewBag.Event = get;
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return View ();
        }

        [Authorize]
        public IActionResult EditEventData (int Id, string name, DateTime date) {
            var get = _AppDbContext.Events.Find(Id);
            get.Name = name;
            get.day = date;
            _AppDbContext.SaveChanges();
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return RedirectToAction ("Event","Event");
        }

        [Authorize]
        public IActionResult AddEvent () {
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return View ();
        }

        public IActionResult Remove (int Id) {
            var Event = _AppDbContext.Events.Find(Id);
            _AppDbContext.Remove(Event);
            _AppDbContext.SaveChanges();
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return RedirectToAction ("Event","Event");
        }

        [ResponseCache (Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error () {
            return View (new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}