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
    public class EmployeeController : Controller {
        private AppDbContext _AppDbContext;
        private readonly ILogger<HomeController> _logger;

        public EmployeeController (ILogger<HomeController> logger, AppDbContext appDbContext) {
            _AppDbContext = appDbContext;
            _logger = logger;
        }

        [Authorize]
        public IActionResult EmployeeList (string status_employee, int _crntpage = 1) {
            if (status_employee == null) {
                status_employee = "permanent";
            }
               if(!_AppDbContext.Pagings.Any()){
                var obj = new Paging{
                    StatusPage = "Permanent",
                    CurentPage = 1,
                    Search = null,
                    ShowItem = 6
                };
                _AppDbContext.Pagings.Add(obj);
                _AppDbContext.SaveChanges();
            }
            var set_page = _AppDbContext.Pagings.Find (1);
            set_page.Search = "";
            set_page.StatusPage = status_employee;
            set_page.CurentPage = _crntpage;
            _AppDbContext.SaveChanges ();
            if (set_page.CurentPage == 1) {
                var take = set_page.ShowItem;
                var spesific_employee = from a in _AppDbContext.Employee where a.Status == set_page.StatusPage select a;
                var get = from a in spesific_employee.Take (take) where a.Status == set_page.StatusPage select a;
                ViewBag.items = get;
                ViewBag.page = set_page;
            } else {
                var take = set_page.ShowItem;
                var spesific_employee = from a in _AppDbContext.Employee where a.Status == set_page.StatusPage select a;
                var get = from a in spesific_employee.Skip (take * (set_page.CurentPage - 1)).Take (take) select a;
                ViewBag.items = get;
                ViewBag.page = set_page;
            }
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Paging =set_page;
            ViewBag.Req = countReq;
            return View ("EmployeeList");
        }

        //Search belum dihandling kalo munculnya banyak harus tetep bisa pagination
        [Authorize]
        public IActionResult Search (string keyword) {
            var paging = _AppDbContext.Pagings.Find (1);
            paging.Search = keyword;
            _AppDbContext.SaveChanges ();
            var get = from a in _AppDbContext.Employee where (a.Status == paging.StatusPage) && (a.Name.Contains (keyword) || a.Phone.Contains (keyword) || a.Address.Contains (keyword) || a.Email.Contains (keyword) || a.Occupation.Contains (keyword) || a.Placement.Contains (keyword)) select a;
            ViewBag.items = get;
            ViewBag.Paging = paging;
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return View ("EmployeeList");
        }

        [Authorize]
        public IActionResult EmployeeRemove (int Id) {
            var rmv = _AppDbContext.Employee.Find (Id);
            _AppDbContext.Remove (rmv);
            _AppDbContext.SaveChanges ();
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return RedirectToAction ("EmployeeList", "Employee");
        }

        [Authorize]
        public IActionResult EmployeeDetail (int Id) {
            var show = _AppDbContext.Employee.Find (Id);
            ViewBag.items = show;
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return View ("EmployeeDetail");
        }
        public IActionResult EmployeeUpdate (int Id) {
            var show = _AppDbContext.Employee.Find (Id);
            ViewBag.items = show;
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return View ("EmployeeUpdate");
        }

        [Authorize]
        public IActionResult EmployeeUpdateData (int Id, string name, string email, string address, string phone, string gender, DateTime birth_date, string birth_place, string occupation, string placement, string emergency1, string emergency2, string emergency3, string phone1, string phone2, string phone3, string status, IFormFile image = null) {
            var file = "";
            if (image == null) {
                var getemployee = _AppDbContext.Employee.Find (Id);
                file = getemployee.Image;
            } else if (image != null) {
                var path = "wwwroot//image";
                Directory.CreateDirectory (path);
                var Filename = Path.Combine (path, Path.GetFileName (image.FileName));
                image.CopyTo (new FileStream (Filename, FileMode.Create));
                file = Filename.Substring (8).Replace (@"\", "/");
                Console.WriteLine (file);
                Console.WriteLine ("ini nama file");
            }
            var get = _AppDbContext.Employee.Find (Id);
            get.Image = file;
            get.Name = name;
            get.Email = email;
            get.Address = address;
            get.Phone = phone;
            get.Occupation = occupation;
            get.Placement = placement;
            get.EmergencyContact1 = emergency1;
            get.EmergencyContact2 = emergency2;
            get.EmergencyContact3 = emergency3;
            get.Phone1 = phone1;
            get.Phone2 = phone2;
            get.Phone3 = phone3;
            get.Status = status;
            _AppDbContext.SaveChanges ();
            ViewBag.items = get;
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return View ("EmployeeDetail");
        }

        [Authorize]
        public IActionResult EmployeeAddData (string name, string email, string address, string phone, string gender, DateTime birth_date, string birth_place, string occupation, string placement, string emergency1, string emergency2, string emergency3, string phone1, string phone2, string phone3, string status, IFormFile image = null) {
            var file = "";
            if (image != null) {
                var path = "wwwroot//image";
                Directory.CreateDirectory (path);
                var Filename = Path.Combine (path, Path.GetFileName (image.FileName));
                image.CopyTo (new FileStream (Filename, FileMode.Create));
                file = Filename.Substring (8).Replace (@"\", "/");
                Console.WriteLine (file);
                Console.WriteLine ("ini nama file");
            }
            var obj = new Employee () {
                Image = file,
                Name = name,
                Email = email,
                Address = address,
                Phone = phone,
                BirthDate = birth_date,
                BirthPlace = birth_place,
                Gender = gender,
                Occupation = occupation,
                Placement = placement,
                EmergencyContact1 = emergency1,
                EmergencyContact2 = emergency2,
                EmergencyContact3 = emergency3,
                Phone1 = phone1,
                Phone2 = phone2,
                Phone3 = phone3,
                Status = status,
            };
            _AppDbContext.Add (obj);
            _AppDbContext.SaveChanges ();
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return RedirectToAction ("EmployeeList", "Employee");
        }
        public IActionResult EmployeeAddDataAgain (string name, string email, string address, string phone, string gender, DateTime birth_date, string birth_place, string occupation, string placement, string emergency1, string emergency2, string emergency3, string phone1, string phone2, string phone3, string status, IFormFile image = null) {
            var file = "";
            if (image != null) {
                var path = "wwwroot//image";
                Directory.CreateDirectory (path);
                var Filename = Path.Combine (path, Path.GetFileName (image.FileName));
                image.CopyTo (new FileStream (Filename, FileMode.Create));
                file = Filename.Substring (8).Replace (@"\", "/");
                Console.WriteLine (file);
                Console.WriteLine ("ini nama file");
            }
            var obj = new Employee () {
                Image = file,
                Name = name,
                Email = email,
                Address = address,
                Phone = phone,
                BirthDate = birth_date,
                BirthPlace = birth_place,
                Gender = gender,
                Occupation = occupation,
                Placement = placement,
                EmergencyContact1 = emergency1,
                EmergencyContact2 = emergency2,
                EmergencyContact3 = emergency3,
                Phone1 = phone1,
                Phone2 = phone2,
                Phone3 = phone3,
                Status = status
            };
            _AppDbContext.Add (obj);
            _AppDbContext.SaveChanges ();
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return RedirectToAction ("AddEmployee", "Employee");
        }

        [Authorize]
        [HttpPost]
        public IActionResult Imports ([FromForm (Name = "file")] IFormFile file) 
        {
            Console.WriteLine(file.FileName);
            Console.WriteLine ("masuk method sini");
            string filePath = string.Empty;
            if (file != null) {
                Console.WriteLine (file.FileName);
                try {
                    string fileExtension = Path.GetExtension (file.FileName);
                    if (fileExtension != ".csv") {
                        ViewBag.Message = "Please select the csv file";
                        return RedirectToAction ("EmployeeList", "Employee");
                    }
                    using (var reader = new StreamReader (file.OpenReadStream ())) {
                        string[] header = reader.ReadLine ().Split (',');
                        while (!reader.EndOfStream) {
                            Console.WriteLine ("Masuk while");
                            string[] rows = reader.ReadLine ().Split (',');
                            var obj = new Employee () {
                                Name = rows[0].ToString (),
                                Email = rows[1].ToString (),
                                Phone = rows[2].ToString (),
                                Gender = rows[3].ToString (),
                                BirthDate = Convert.ToDateTime (rows[4].ToString ()),
                                BirthPlace = rows[5].ToString (),
                                Occupation = rows[6].ToString (),
                                Placement = rows[7].ToString (),
                                Address = rows[8].ToString (),
                                Status = rows[9].ToString (),
                                EmergencyContact1 = rows[10].ToString (),
                                Phone1 = rows[11].ToString (),
                                EmergencyContact2 = rows[12].ToString (),
                                Phone2 = rows[13].ToString (),
                                EmergencyContact3 = rows[14].ToString (),
                                Phone3 = rows[15].ToString (),
                                Image = rows[16].ToString ()
                            };
                            _AppDbContext.Employee.Add (obj);
                        }
                        _AppDbContext.SaveChanges ();
                    }
                    var leaver = from a in _AppDbContext.LeaveRequests where a.status == "pending"
                    select a;
                    var countR = leaver.Count ();
                    ViewBag.Req = countR;
                    return RedirectToAction ("EmployeeList", "Employee");
                } catch (Exception e) {
                    ViewBag.Message = e.Message;
                }
            }
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return RedirectToAction ("Dashboard", "Home");
        }

        [Authorize]
        public IActionResult ExportAll () {
            var comlumHeadrs = new string[] {
                "Id",
                "Name",
                "Email",
                "Phone",
                "Gender",
                "BirthDate",
                "BirthPlace",
                "Occupation",
                "Placement",
                "Address",
                "Status",
                "Emergency Name1",
                "Phone 1",
                "Emergency Name2",
                "Phone 2",
                "Emergency Name3",
                "Phone 3",
                "Image"
            };
            var items = (from item in _AppDbContext.Employee select new object[] {
                item.Id,
                    $"{item.Name}",
                    $"{item.Email}",
                    $"{item.Phone}",
                    $"{item.Gender}",
                    $"{item.BirthDate}",
                    $"{item.BirthPlace}",
                    $"{item.Occupation}",
                    $"{item.Placement}",
                    $"{item.Address}",
                    $"{item.Status}",
                    $"{item.EmergencyContact1}",
                    $"{item.Phone1}",
                    $"{item.EmergencyContact2}",
                    $"{item.Phone2}",
                    $"{item.EmergencyContact3}",
                    $"{item.Phone3}",
                    $"{item.Image}"
            }).ToList ();

            var itemcsv = new StringBuilder ();
            items.ForEach (line => {
                itemcsv.AppendLine (string.Join (",", line));
            });

            byte[] buffer = Encoding.ASCII.GetBytes ($"{string.Join(",", comlumHeadrs)}\r\n{itemcsv.ToString()}");
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return File (buffer, "text/csv", $"AllEmployees.csv");
        }
        
        public IActionResult CSVformat () {
            var comlumHeadrs = new string[] {
                "Name",
                "Email",
                "Phone",
                "Gender",
                "BirthDate",
                "BirthPlace",
                "Occupation",
                "Departement",
                "Address",
                "Status",
                "Emergency Name1",
                "Phone 1",
                "Emergency Name2",
                "Phone 2",
                "Emergency Name3",
                "Phone 3",
                "Image"
            };
            var itemcsv = new StringBuilder ();
            byte[] buffer = Encoding.ASCII.GetBytes ($"{string.Join(",", comlumHeadrs)}\r\n{itemcsv.ToString()}");
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return File (buffer, "text/csv", $"EmployeeFormat.csv");
        }

        [Authorize]
        public IActionResult ExportCSV () {
            var paging = _AppDbContext.Pagings.Find (1);

            var comlumHeadrs = new string[] {
                "Id",
                "Name",
                "Email",
                "Phone",
                "Gender",
                "BirthDate",
                "BirthPlace",
                "Occupation",
                "Departement",
                "Address",
                "Status",
                "Emergency Name1",
                "Phone 1",
                "Emergency Name2",
                "Phone 2",
                "Emergency Name3",
                "Phone 3",
                "Image"
            };
            var items = new List<object[]> ();
            if (paging.Search == null) {
                items = (from item in _AppDbContext.Employee where item.Status == paging.StatusPage select new object[] {
                    item.Id,
                        $"{item.Name}",
                        $"{item.Email}",
                        $"{item.Phone}",
                        $"{item.Gender}",
                        $"{item.BirthDate}",
                        $"{item.BirthPlace}",
                        $"{item.Occupation}",
                        $"{item.Placement}",
                        $"{item.Address}",
                        $"{item.Status}",
                        $"{item.EmergencyContact1}",
                        $"{item.Phone1}",
                        $"{item.EmergencyContact2}",
                        $"{item.Phone2}",
                        $"{item.EmergencyContact3}",
                        $"{item.Phone3}",
                        $"{item.Image}"
                }).ToList ();
            } else if (paging.Search != null) {
                items = (from item in _AppDbContext.Employee where item.Status == paging.StatusPage && (item.Name.Contains (paging.Search) || item.Email.Contains (paging.Search) || item.Phone.Contains (paging.Search) || item.Occupation.Contains (paging.Search) || item.Address.Contains (paging.Search) || item.Placement.Contains (paging.Search)) select new object[] {
                    item.Id,
                        $"{item.Name}",
                        $"{item.Email}",
                        $"{item.Phone}",
                        $"{item.Gender}",
                        $"{item.BirthDate}",
                        $"{item.BirthPlace}",
                        $"{item.Occupation}",
                        $"{item.Placement}",
                        $"{item.Address}",
                        $"{item.Status}",
                        $"{item.EmergencyContact1}",
                        $"{item.EmergencyContact2}",
                        $"{item.EmergencyContact3}",
                        $"{item.Phone1}",
                        $"{item.Phone2}",
                        $"{item.Phone3}",
                        $"{item.Image}"
                }).ToList ();
            }

            var itemcsv = new StringBuilder ();
            items.ForEach (line => {
                itemcsv.AppendLine (string.Join (",", line));
            });

            byte[] buffer = Encoding.ASCII.GetBytes ($"{string.Join(",", comlumHeadrs)}\r\n{itemcsv.ToString()}");
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return File (buffer, "text/csv", $"Employees.csv");
            // return RedirectToAction("ListEmployees","Employees");
        }

        [Authorize]
        public IActionResult AddEmployee () {
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return View ();
        }

        [Authorize]
        public IActionResult Warning (int Id) {
            var emp = _AppDbContext.Employee.Find(Id);
            ViewBag.Employee = emp;
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return View ();
        }

        [Authorize]
        public IActionResult SendMessage (int Id, string title, string body) {
            var emp = _AppDbContext.Employee.Find(Id);
                Broadcast broad = new Broadcast () {
                title = title,
                date = DateTime.Now,
                body = body,
            };
            _AppDbContext.Broadcasts.Add (broad);
            _AppDbContext.SaveChanges ();
            var message = new MimeMessage ();
                message.To.Add (new MailboxAddress (emp.Name, emp.Email));
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
            var leavereq = from a in _AppDbContext.LeaveRequests where a.status == "pending"
            select a;
            var countReq = leavereq.Count ();
            ViewBag.Req = countReq;
            return RedirectToAction("EmployeeList","Employee");
        }

        [Authorize]
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