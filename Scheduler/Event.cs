using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MimeKit;
using HC_WEB_FINALPROJECT.Models;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace HC_WEB_FINALPROJECT.Scheduler
{
public class EventService : IHostedService
{
        private readonly IServiceScopeFactory _scopeFactory;
        public EventService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Start");
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var x = from i in context.Events select i;
                foreach (var i in x)
                {
                    if (i.day.Date.AddDays(-3) == DateTime.Now.Date)
                    {
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
                var DB = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var emp = from i in DB.Employee select i;
                var reminder = from i in DB.Events where i.day.Date.AddDays(-3)==DateTime.Now.Date select i; 
                List<string> nameReminder = new List<string>();
                List<DateTime> dateReminder = new List<DateTime>();
                foreach(var x in reminder)
                {
                    nameReminder.Add(x.Name);
                    dateReminder.Add(x.day);
                }
                for(int i=0;i<nameReminder.Count();i++)
                {
                    foreach (var x in emp)
                    {
                        Console.WriteLine(nameReminder[i]);
                        Console.WriteLine("Masuk event sini");
                        var message = new MimeMessage();
                        message.To.Add(new MailboxAddress(x.Name, x.Email));
                        message.From.Add(new MailboxAddress("HR","HumanResource@HR.co.id"));
                        message.Subject = "Happy "+nameReminder[i];
                        message.Body = new TextPart("plain")
                        {
                            Text = "Dalam memperingati Hari "+nameReminder[i]+" maka perusahaan meliburkan semua employee pada tanggal : "+dateReminder[0]
                        };
                    using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
                    {
                        emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        emailClient.Connect("smtp.mailtrap.io", 587, false);
                        emailClient.Authenticate ("5eeab4aa9b0dc4", "6000743583ad4f");
                        emailClient.Send(message);
                        emailClient.Disconnect(true);
                    }
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