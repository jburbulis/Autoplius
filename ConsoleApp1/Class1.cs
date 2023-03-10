using System.Net;
using System.Net.Mail;
using Autoplius.Repository;
using Autoplius.Repository.Repositories;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;

namespace AutopliusReader.Services;

public class SelectedCar
{
    public string Name { get; set; }

    public double Price { get; set; }

    public string Url { get; set; }
}

public static class Timmer
{
    public static bool ReadingRunning;

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

            Console.WriteLine("pries");
            var v = new SearchesRepository(new AutopliusDatabase());
            var v2 = new SearchesItemRepository(new AutopliusDatabase());

            var allActiveSearchesList = v.GetAll(x => x.Active, inc => inc.Include(i => i.Cars));

            Console.WriteLine($"nuskaite {allActiveSearchesList.Count}");

            foreach (var search in allActiveSearchesList)

            {
                // From Web
                var url = search.Url;
                var web = new HtmlWeb();

                web.UserAgent = "Mozilla/5.0 (X11; CrOS armv7l 13597.84.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.5112.105 Safari/537.36";

               web.UseCookies = true;

                var page = 1;

                var tt = new HtmlNodeCollection(null);
                var requestUrl = url;

                var doc2 = web.Load("https://autoplius.lt/skelbimai/motociklai-moto-apranga/motociklai?slist=1682976212&category_id=3&make_date_from=2020&engine_capacity_from=570&sell_price_to=10000");
                Console.WriteLine(doc2.DocumentNode.OuterHtml);

                //HtmlAgilityPack.HtmlDocument doc1 = web.Load("https://autoplius.lt/skelbimai/motociklai-moto-apranga/motociklai?slist=1682976212&category_id=3&make_date_from=2020&engine_capacity_from=570&sell_price_to=10000",
                //    "140.238.65.230", 1080, string.Empty, string.Empty);
                //Console.WriteLine("DOC1 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                //Console.WriteLine(doc1.DocumentNode.OuterHtml);
                //Console.WriteLine("finDOC1 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");


                var millisecondsTimeout3 = Timmer.Delay(delaySeconds: true);
                Console.WriteLine($"pamiegosiu {millisecondsTimeout3 / 100}");
                Thread.Sleep(millisecondsTimeout3);
                while (true)
                {
                    try
                    {
                        var doc = web.Load(requestUrl);
                        Console.WriteLine("");
                        page++;

                        if (doc ==null)
                        {
                            Console.WriteLine($"doc null");
                        }

                        if (doc.DocumentNode == null)
                        {
                            Console.WriteLine($"DocumentNode null");
                        }

                            Console.WriteLine(doc.DocumentNode.OuterHtml);
                        

                        var l = doc.DocumentNode.SelectNodes("//a[contains(@class,'announcement-item')]");

                        if (l == null)
                        {
                            Console.WriteLine($"null");
                            break;
                        }
                        Console.WriteLine(" iki timer");

                        var millisecondsTimeout = Timmer.Delay(delaySeconds: true);
                        Console.WriteLine($"pamiegosiu {millisecondsTimeout/100}");
                        Thread.Sleep(millisecondsTimeout);

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
                var millisecondsTimeout2 = Timmer.Delay(delaySeconds: true);
                Console.WriteLine($"pamiegosiu {millisecondsTimeout2 / 100}");
                Thread.Sleep(millisecondsTimeout2);

                var selectedCars = new List<SelectedCar>();

                foreach (var addddasdasdsadddsaadsddsa in tt)
                {
                    selectedCars.Add(new SelectedCar
                    {
                        Name = addddasdasdsadddsaadsddsa.SelectSingleNode(".//div[contains(@class,'announcement-title')]").InnerText.Replace("\n", "").TrimStart().TrimEnd(),
                        Price = Convert.ToDouble(new string(addddasdasdsadddsaadsddsa.SelectSingleNode(".//div[contains(@class,'announcement-pricing-info')]")
                            .InnerText
                            .Where(char.IsDigit)
                            .ToArray())),
                        Url = addddasdasdsadddsaadsddsa.Attributes["href"].Value
                    });
                }

                var searchesItems = search.Cars.Select(s => s.Url).ToList();

                var newCars = selectedCars.Where(tr => !searchesItems.Contains(tr.Url)).ToList();

                Console.WriteLine($"yra nauju {newCars.Count}");

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

                var dropedPriceCars = new List<SearchesItem>();

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

            SendMail(
                e.Message,
                "AutoPlius Crawler down",
                "AutopliusScanner");

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
}