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
    public class BroadcastController : Controller {
        private AppDbContext _AppDbContext;
        private readonly ILogger<BroadcastController> _logger;

        public BroadcastController (ILogger<BroadcastController> logger, AppDbContext appDbContext) {
            _AppDbContext = appDbContext;
            _logger = logger;
        }

        [Authorize]
        public IActionResult Add (string title, string body) {
            Broadcast broad = new Broadcast () {
                title = title,
                date = DateTime.Now,
                body = body,
            };
            _AppDbContext.Broadcasts.Add (broad);
            _AppDbContext.SaveChanges ();
            var message = new MimeMessage ();
            var get = from a in _AppDbContext.Employee select a;
            foreach (var a in get) {
                message.To.Add (new MailboxAddress (a.Name, a.Email));
                message.From.Add (new MailboxAddress ("HR", "HumanResource@HR.co.id"));
                message.Subject = title;
                message.Body = new TextPart ("plain") {
                    Text = body
                };
                using (var emailClient = new MailKit.Net.Smtp.SmtpClient ()) {
                    emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    emailClient.Connect ("smtp.mailtrap.io", 587, false);
                    emailClient.Authenticate ("5eeab4aa9b0dc4", "6000743583ad4f");
                    emailClient.Send (message);
                    emailClient.Disconnect (true);
                }
            }
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return View ("Broadcast");
        }

        [Authorize]
        public IActionResult Broadcast () {
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