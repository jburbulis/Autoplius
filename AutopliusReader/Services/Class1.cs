using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using Autoplius.Repository;
using Autoplius.Repository.Repositories;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AutopliusReader.Services
{
    public class SelectedCar
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }

    public static class Timmer
    {
        public static bool ReadingRunning = false;

        public static int Delay(bool delayHour = false, bool delayMinutes = false, bool delaySeconds = false)
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            var ticks = rnd.Next(60000, 300000);

            if (delaySeconds)
            {
                ticks = rnd.Next(10000, 20000);
            }

            if (delayHour)
            {
                ticks = 1000 * 60 * 60 * rnd.Next(3, 5) + rnd.Next(30000, 2500000);
            }

            if (delayMinutes)
            {
                ticks = rnd.Next(600000, 900000); // from:10 minutes; until:15 minutes
            }

            return 7000 + ticks;
        }
    }

    public sealed class Class1
    {
        public static void CheckCars()
        {
            if (Timmer.ReadingRunning)
            {
                return;
            }

            try
            {
                Timmer.ReadingRunning = true;





                var v = new SearchesRepository(new AutopliusDatabase());
                var v2 = new SearchesItemRepository(new AutopliusDatabase());

                var allActiveSearchesList = v.GetAll(x => x.Active, inc => inc.Include(i => i.Cars));

                foreach (var search in allActiveSearchesList)

                {
                    // From Web
                    var url = search.Url;
                    var web = new HtmlWeb();
                    var page = 1;

                    var tt = new HtmlNodeCollection(null);
                    var requestUrl = url;






                 //   string htmlCode = new WebClient().DownloadString(requestUrl);











                    while (true)
                    {
                        try
                        {
                            var doc = web.Load(requestUrl);
                            page++;
                            var l = doc.DocumentNode.SelectNodes("//a[contains(@class,'announcement-item')]");

                            if (l == null)
                            {
                                break;
                            }

                            Thread.Sleep(Timmer.Delay(delaySeconds: true));
                            foreach (var llll in l)
                            {
                                tt.Add(llll);
                            }

                            requestUrl = url + "&page_nr=" + page;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);

                            break;
                        }
                    }

                    var selectedCars = new List<SelectedCar>();

                    foreach (var addddasdasdsadddsaadsddsa in tt)
                    {
                        selectedCars.Add(new SelectedCar
                        {
                            Name = addddasdasdsadddsaadsddsa.SelectSingleNode(".//div[contains(@class,'announcement-title')]").InnerText.Replace("\n", "").TrimStart().TrimEnd(),
                            Price = Convert.ToDouble(new string(addddasdasdsadddsaadsddsa.SelectSingleNode(".//div[contains(@class,'announcement-pricing-info')]").InnerText
                                .Where(char.IsDigit).ToArray())),
                            Url = addddasdasdsadddsaadsddsa.Attributes["href"].Value
                        });
                    }

                    var searchesItems = search.Cars.Select(s => s.Url).ToList();

                    var newCars = selectedCars.Where(tr => !searchesItems.Contains(tr.Url)).ToList();

                    if (newCars.Any())
                    {
                        foreach (var newCar in newCars)
                        {
                            v2.AddWithSave(new SearchesItem
                            {
                                Url = newCar.Url,
                                AddDateTime = DateTime.Now,
                                Name = newCar.Name,
                                Price = newCar.Price,
                                SearchesId = search.Id
                            });
                        }

                        SendMail(newCars.Select(x => $"{x.Name} - {x.Price} - {x.Url}").Aggregate((a, b) => a + $",{Environment.NewLine}" + b),
                            $"{search.Name} nauji Skelibimai {DateTime.Now:yyyy-MM-dd hh:mm}",
                            "AutopliusScanner");
                    }

                    var dropedPriceCars =new List<SearchesItem>();

                    foreach (var car in selectedCars)
                    {
                        var droppedPriceCar = search.Cars.FirstOrDefault(x => x.Url == car.Url && x.Price > car.Price);

                        if (droppedPriceCar != null)
                        {
                            droppedPriceCar.Price = car.Price;

                            v2.UpdateWithSave(droppedPriceCar);

                            dropedPriceCars.Add(droppedPriceCar);
                        }
                    }

                    if (dropedPriceCars.Any())
                    {
                        SendMail(
                            dropedPriceCars.Select(x => $"{x.Name} - {x.Price} - {x.Url}").Aggregate((a, b) => a + $",{Environment.NewLine}" + b),
                            $"{search.Name} pasikeitusios kainos Skelibimai {DateTime.Now:yyyy-MM-dd hh:mm}",
                            "AutopliusScanner");
                    }




                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Timmer.ReadingRunning = false;

                throw;
            }
            finally
            {
                Timmer.ReadingRunning = false;
            }
        }

        private static void SendMail(string bodyStr, string subjectStr, string displayName)
        {
            var fromAddress = new MailAddress("kaulazs@gmail.com", displayName);
            var toAddress = new MailAddress("j.burbulis@gmail.com", "Justinas");
            var fromPassword = "ivbqaehtklvyolgm";
            var subject = subjectStr;
            var body = bodyStr;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };

            smtp.Send(message);
        }

        public class FundType
        {
            public string type { get; set; }
            public string name { get; set; }
            public object isEnabled { get; set; }
        }

        public class ReferralNeed
        {
            public string type { get; set; }
            public string name { get; set; }
            public object isEnabled { get; set; }
        }

        public class Datum
        {
            public object practitionerId { get; set; }
            public object professionCode { get; set; }
            public object professionName { get; set; }
            public int healthcareServiceId { get; set; }
            public string healthcareServiceName { get; set; }
            public Class1.FundType fundType { get; set; }
            public Class1.ReferralNeed referralNeed { get; set; }
            public int organizationId { get; set; }
            public string organizationName { get; set; }

            [JsonConverter(typeof(Class1.MicrosecondEpochConverter))]
            public DateTime earliestTime { get; set; }

            public int periodTimesCount { get; set; }
        }

        public class Links
        {
            public string self { get; set; }
            public object prev { get; set; }
            public string next { get; set; }
        }

        public class Meta
        {
            public int totalPages { get; set; }
        }

        public class Root
        {
            public List<Class1.Datum> data { get; set; }
            public Class1.Links links { get; set; }
            public Class1.Meta meta { get; set; }
        }

        public class MicrosecondEpochConverter : DateTimeConverterBase
        {
            private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 3, 0, 0, DateTimeKind.Utc);

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteRawValue(((DateTime)value - _epoch).TotalMilliseconds + "");
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.Value == null)
                {
                    return null;
                }

                return _epoch.AddMilliseconds((long)reader.Value);
            }
        }
    }
}