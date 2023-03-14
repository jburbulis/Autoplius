using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Autoplius.Repository;
using Autoplius.Repository.Repositories;
using ConsoleApp1.Models;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Timer = ConsoleApp1.Utils.Timer;

namespace AutopliusReader.Services;

public sealed class CarReaderService
{
    private readonly SearchesItemRepository _searchesItemRepository;
    private readonly SearchesRepository _searchesRepository;

    public CarReaderService()
    {
        _searchesRepository = new SearchesRepository(new AutopliusDatabase());
        _searchesItemRepository = new SearchesItemRepository(new AutopliusDatabase());
    }

    public void CheckCars()
    {
        if (Timer.ReadingRunning)
        {
            return;
        }

        if (IsNight())
        {
            return;
        }

        var options = new ChromeOptions();
        options.SetLoggingPreference(LogType.Browser, LogLevel.Off);
        options.SetLoggingPreference(LogType.Driver, LogLevel.Off);
        options.SetLoggingPreference(LogType.Performance, LogLevel.Off);
        IWebDriver driver = new ChromeDriver(options);

        try
        {
            Timer.ReadingRunning = true;

            var nav = driver.Navigate();

            var allActiveSearchesList = _searchesRepository.GetAll(x => x.Active, inc => inc.Include(i => i.Cars));

            Console.WriteLine($"Viso aktyvių paieškų: {allActiveSearchesList.Count}");

            foreach (var search in allActiveSearchesList)
            {
                Console.WriteLine($"-------------------------{search.Name}-------------------------");

                var url = search.Url;

                var page = 1;

                var carNodes = new List<IWebElement>();
                var requestUrl = url;

                var maxPage = 0;

                while (true)
                {
                    try
                    {
                        nav.GoToUrl(requestUrl);

                        try
                        {
                            var totalCars = driver.FindElement(By.ClassName("result-count"))
                                .Text;

                            var result = Regex.Replace(totalCars, @"[^\d]", "");

                            Console.WriteLine($"Viso skelbimų: {result}");

                            Decimal.TryParse(result, out var count);

                            if (count > 0)
                            {
                                maxPage = (int)Math.Ceiling(count / 20);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        var ads = driver.FindElements(By.ClassName("announcement-item"));
                        ;

                        if (ads == null)
                        {
                            Console.WriteLine("Nepavykus nustatyti max skelbimų skaičiaus/negavus paieškos reuzlta - break");

                            break;
                        }

                        foreach (var ad in ads)
                        {
                            carNodes.Add(ad);
                        }

                        if (maxPage != 0 && maxPage == page)
                        {
                            break;
                        }

                        page++;
                        requestUrl = url + "&page_nr=" + page;

                        var millisecondsTimeout = Timer.Delay(delaySeconds: true);
                        Console.WriteLine($"Delay second :{millisecondsTimeout / 1000}");
                        Thread.Sleep(millisecondsTimeout);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);

                        break;
                    }
                }

                var selectedCars = GetSelectedCars(carNodes);

                NewCarNotify(selectedCars, search);
                DroppedPriceNotify(selectedCars, search);

                var millisecondsTimeout2 = Timer.Delay();
                Console.WriteLine($"Delay minutes: {millisecondsTimeout2 / 1000 / 60}");
                Thread.Sleep(millisecondsTimeout2);

                Console.WriteLine("---------------------------------------------------------------");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Timer.ReadingRunning = false;

            SendMail(e.Message, "AutoPlius Crawler down", "AutopliusScanner");

            throw;
        }
        finally
        {
            driver.Dispose();
            Timer.ReadingRunning = false;
        }
    }

    public static bool IsNight(int fromHour = 22, int toHour = 7)
    {
        var now = DateTime.Now;

        if (now.Hour > fromHour)
        {
            return true;
        }

        if (now.Hour < toHour)
        {
            return true;
        }

        return false;
    }

    private void DroppedPriceNotify(List<SelectedCar> selectedCars, Searches search)
    {
        var droppedPriceCars = new List<SearchesItem>();

        foreach (var car in selectedCars)
        {
            var droppedPriceCar = search.Cars.FirstOrDefault(x => x.Url == car.Url && x.Price > car.Price);

            if (droppedPriceCar != null)
            {
                droppedPriceCar.Price = car.Price;

                _searchesItemRepository.UpdateWithSave(droppedPriceCar);

                droppedPriceCars.Add(droppedPriceCar);
            }
        }

        if (droppedPriceCars.Any())
        {
            SendMail(
                droppedPriceCars.Select(x => $"{x.Name} - {x.Price} - {x.Url}")
                    .Aggregate((a, b) => a + $",{Environment.NewLine}" + b),
                $"{search.Name} pasikeitusios kainos Skelibimai {DateTime.Now:yyyy-MM-dd hh:mm}",
                "AutopliusScanner");
        }
    }

    private static List<SelectedCar> GetSelectedCars(List<IWebElement> carNodes)
    {
        var selectedCars = new List<SelectedCar>();

        foreach (var addddasdasdsadddsaadsddsa in carNodes)
        {
            selectedCars.Add(new SelectedCar
            {
                Name = addddasdasdsadddsaadsddsa.FindElement(By.ClassName("announcement-title"))
                    .Text
                    .Replace("\n", "")
                    .TrimStart()
                    .TrimEnd(),

                Price = Convert.ToDouble(new string(addddasdasdsadddsaadsddsa.FindElement(By.ClassName("announcement-pricing-info"))
                    .Text
                    .Where(Char.IsDigit)
                    .ToArray())),
                Url = addddasdasdsadddsaadsddsa.GetAttribute("href")
            });
        }

        return selectedCars;
    }

    private void NewCarNotify(List<SelectedCar> selectedCars, Searches search)
    {
        var searchesItems = search.Cars
            .Select(s => s.Url)
            .ToList();

        var newCars = selectedCars.Where(tr => !searchesItems.Contains(tr.Url))
            .ToList();

        Console.WriteLine($"Rasta naujų skelbimų: {newCars.Count}");

        if (newCars.Any())
        {
            foreach (var newCar in newCars)
            {
                _searchesItemRepository.AddWithSave(new SearchesItem
                {
                    Url = newCar.Url,
                    AddDateTime = DateTime.Now,
                    Name = newCar.Name,
                    Price = newCar.Price,
                    SearchesId = search.Id
                });
            }

            SendMail(newCars.Select(x => $"{x.Name} - {x.Price} - {x.Url}")
                    .Aggregate((a, b) => a + $",{Environment.NewLine}" + b),
                $"{search.Name} nauji Skelibimai {DateTime.Now:yyyy-MM-dd hh:mm}",
                "AutopliusScanner");
        }
    }

    public void SendMail(string bodyStr, string subjectStr, string displayName)
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