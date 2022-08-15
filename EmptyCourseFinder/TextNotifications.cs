using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using EmptyCourseFinder.Models;
using EmptyCourseFinder.Models.Mongo;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace EmptyCourseFinder
{
    public class TextNotifications
    {
        public static List<Course> courses = new List<Course>();
        public readonly TwilioSettings _twilioSettings;
        public readonly MongoSettings _mongoSettings;

        public TextNotifications(IOptions<TwilioSettings> twilioSettings, IOptions<MongoSettings> mongoSettings)
        {
            _twilioSettings = twilioSettings.Value;
            _mongoSettings = mongoSettings.Value;
        }

        [FunctionName("TextNotifications")]
        public async Task RunAsync([TimerTrigger("0 0 6-18 * * *")]TimerInfo myTimer, ILogger log)
        {

            var ssid = _twilioSettings.TwilioSSID;
            var token = _twilioSettings.TwilioSecret;

            TwilioClient.Init(ssid, token);

            var client = new MongoClient(_mongoSettings.ConnectionString);

            var database = client.GetDatabase(_mongoSettings.Database);

            var collection = database.GetCollection<User>(_mongoSettings.Collection);

            var results = await collection.FindAsync(_ => true);
            var realsults = await results.ToListAsync();

            foreach (var result in realsults)
            {
                var currentTime = DateTime.Now;

                if(currentTime.Hour >= result.TimeStart && currentTime.Hour <= result.TimeEnd)
                {
                    var emptyCourses = GetEmptyCourses(result.Lat, result.Lon);

                    var body = "Empty Courses Near You:\n";

                    foreach (var course in emptyCourses)
                    {
                        body += $"{course.Name}: {course.Current_Popularity}\n";
                    }

                    var message = MessageResource.Create(
                        body: body,
                        from: new Twilio.Types.PhoneNumber("+12166778085"),
                        to: new Twilio.Types.PhoneNumber(result.Number)
                    );

                    courses.RemoveAll(x => x.Name != null);
                }
            }
        }

        public IEnumerable<Course> GetEmptyCourses(string lat, string lon)
        {
            var cmd = $"-u {Environment.GetEnvironmentVariable("Script")} {lat} {lon}";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Environment.GetEnvironmentVariable("Python"),
                    Arguments = cmd,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                },
                EnableRaisingEvents = true
            };

            process.ErrorDataReceived += Process_Error;
            process.OutputDataReceived += Process_OutputDataReceived;

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();

            courses = courses.OrderBy(c => c.Current_Popularity).ToList();

            return courses;
        }

        static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                var item = JsonConvert.DeserializeObject<Course>(e.Data);
                courses.Add(item);
            }
        }

        static void Process_Error(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }
    }
}
