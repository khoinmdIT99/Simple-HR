using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HC_WEB_FINALPROJECT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace HC_WEB_FINALPROJECT.Controllers
{
    public class HomeController : Controller
    {

        public IConfiguration Configuration;
        private AppDbContext _AppDbContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, AppDbContext appDbContext, IConfiguration configuration)
        {
            _AppDbContext = appDbContext;
            _logger = logger;
            Configuration = configuration;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(string Email, string Password)
        {
            IActionResult response = Unauthorized();

            var user = AuthenticatedUser(Email, Password);
            if (user != null)
            {
                var token = GenerateJwtToken(user);
                HttpContext.Session.SetString("JWTToken", token);
                var get = HttpContext.Session.GetString("JWTToken");
                Console.WriteLine(get);
                Console.WriteLine("ini JWT token");
                var cek = from x in _AppDbContext.Account select x;
                foreach (var item in cek)
                {
                    if (item.Email == Email && item.Password == Password)
                    {
                        HttpContext.Response.Cookies.Append("email", Email);
                        return RedirectToAction("Dashboard", "Home");
                    }
                }
            }
            return View("Index");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWTToken");
            return RedirectToAction("Index", "Home");
        }

        private string GenerateJwtToken(Account user)
        {
            var secuityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(secuityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim (JwtRegisteredClaimNames.Sub, Convert.ToString (user.Email)),
                new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid ().ToString ())
            };

            var token = new JwtSecurityToken(
                issuer: Configuration["Jwt:Issuer"],
                audience: Configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials);
            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodedToken;
        }

        private Account AuthenticatedUser(string Email, string Password)
        {
            Account user = null;
            var get = from i in _AppDbContext.Account select i;
            foreach (var i in get)
            {
                if (i.Email == Email && i.Password == Password)
                {
                    user = new Account
                    {
                        Email = Email,
                        Password = Password,
                    };
                }
            }
            return user;
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            var now = DateTime.Now;
            var Event = from a in _AppDbContext.Events where a.day.Date.AddDays(-3) == now.Date || a.day.Date.AddDays(-2) == now.Date || a.day.Date.AddDays(-1) == now.Date select a;
            var presence = from a in _AppDbContext.Attendances where a.ClockIn.Day == now.Day && a.ClockIn.Month == now.Month && a.ClockIn.Year == now.Year select a;
            var presenceCount = presence.Count();
            var Employee = from a in _AppDbContext.Employee select a;
            var Female = from a in _AppDbContext.Employee
                         where a.Gender == "female"
                         select a;
            var Male = from a in _AppDbContext.Employee
                       where a.Gender == "male"
                       select a;
            var Applicant = from a in _AppDbContext.Applicant
                            where a.Status_Proccess == "unproccess"
                            select a;
            var ApplicantView = (from a in _AppDbContext.Applicant
                                 where a.Status_Proccess == "unproccess"
                                 select a).Take(4);
            foreach (var a in ApplicantView)
            {
                Console.WriteLine(a.Name);
                Console.WriteLine("inicuy");
            }
            var CountApplicant = Applicant.Count();
            var countFemale = Female.Count();
            var countMale = Male.Count();
            var countEmployee = Employee.Count();
            var Leave = from a in _AppDbContext.LeaveRequests where (a.status == "approve") && (now.Date >= a.Start.Date && now.Date <= a.End.Date) select a;
            var countLeave = Leave.Count();
            Console.WriteLine("CEK ISI");
            Console.WriteLine(countLeave);
            Console.WriteLine(countEmployee);
            ViewBag.Event = Event;
            ViewBag.Leave = countLeave;
            ViewBag.Presence = presenceCount;
            ViewBag.Employee = countEmployee;
            ViewBag.Female = countFemale;
            ViewBag.Male = countMale;
            ViewBag.ApplicantView = ApplicantView;
            ViewBag.ApplicantCount = CountApplicant;
            var leavereq = from a in _AppDbContext.LeaveRequests
                           where a.status == "pending"
                           select a;
            var countReq = leavereq.Count();
            ViewBag.Req = countReq;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}