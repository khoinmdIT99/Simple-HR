using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MimeKit;
using HC_WEB_FINALPROJECT.Models;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace HC_WEB_FINALPROJECT.Scheduler
{
public class BirthdayService : IHostedService
{
        private readonly IServiceScopeFactory _scopeFactory;
        public BirthdayService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Start");
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var Emp = (from i in context.Employee where i.BirthDate.Day == DateTime.Now.Day && i.BirthDate.Month == DateTime.Now.Month select i).FirstOrDefault();
                Console.WriteLine(Emp.Name);
                Console.WriteLine("HBD");
                 if(Emp !=null)
                {
                    if(Emp.BirthDate.Day==DateTime.Now.Day && Emp.BirthDate.Month==DateTime.Now.Month )
                    {Console.WriteLine("ini masuk");
                        Task.Run(TaskRoutine, cancellationToken);
                    }
                    else
                    {
                        Task.Run(Dont, cancellationToken);
                    }
                }
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Sync Task stopped");
            return Task.CompletedTask;
        }

        public Task TaskRoutine()
        {
            
            while (true)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var x = from i in context.Employee where (i.BirthDate.Month == DateTime.Now.Month && i.BirthDate.Day == DateTime.Now.Day) select i;
                foreach (var i in x)
                {
                var message = new MimeMessage();
                message.To.Add(new MailboxAddress(i.Name, i.Email));
                message.From.Add(new MailboxAddress("HR","HumanResource@HR.co.id"));
                message.Subject = "Happy Birthday " +i.Name;
                message.Body = new TextPart("plain")
                {
                    Text = "May your birthday be start of a year filled with\n"
                            +"good luck, good health, and much happiness\n"
                            +"We wish all the best for you."
                            +"Have a good day !"
                };
                using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
                {
                    emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    emailClient.Connect("smtp.mailtrap.io", 587, false);
                    emailClient.Authenticate("5eeab4aa9b0dc4", "6000743583ad4f");
                    emailClient.Send(message);
                    emailClient.Disconnect(true);
                }
                }
                }
                //Wait 10 minutes till next execution
                DateTime nextStop = DateTime.Now.AddDays(1);
                var timeToWait = nextStop - DateTime.Now;
                var millisToWait = timeToWait.TotalMilliseconds;
                Thread.Sleep((int)millisToWait);
            }
        }

        public Task Dont()
        {
            
            while (true)
            {
                Console.WriteLine("NOT HAD BHIRTDAY");
                //Wait 10 minutes till next execution
                DateTime nextStop = DateTime.Now.AddDays(1);
                var timeToWait = nextStop - DateTime.Now;
                var millisToWait = timeToWait.TotalMilliseconds;
                Thread.Sleep((int)millisToWait);
            }
        }
    }
}