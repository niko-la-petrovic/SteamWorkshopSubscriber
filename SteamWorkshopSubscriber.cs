using System;
using System.Collections.Generic;
using SteamWorkshopSubscriber.Enums;
using System.Text;
using System.Linq;
using OpenQA.Selenium;
using System.IO;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Text.RegularExpressions;
using System.Threading;

namespace SteamWorkshopSubscriber
{
    public class SteamWorkshopSubscriber
    {

        List<string> workshopUrls;

        BrowserType browserType;
        string driverPath;
        IWebDriver webDriver;

        int driverTimeoutSeconds = 10;

        const string additionalRequiredItemsNameNoExt = "additional_required_items";
        IEnumerable<string> additionalRequiredItems = new List<string>();

        public SteamWorkshopSubscriber(List<string> workshopListsPaths)
        {
            workshopUrls = new List<string>();
            foreach(string workshopListPath in workshopListsPaths)
            {
                workshopUrls = workshopUrls.Concat(
                    System.Text.Json.JsonSerializer
                .Deserialize<List<string>>(File.ReadAllText(workshopListPath)))
                    .ToList();
            }
        }

        public void Start()
        {
            SelectBrowser();
            SelectBrowserVersion();
            InitializeWebDriver();
            UserSignIn();
            StartSubscribing();
            WriteAdditionalItemsToFile();
            Console.WriteLine("Finished subscribing.");
        }

        private void WriteAdditionalItemsToFile()
        {
            string additionalRequiredItemsName = $"{additionalRequiredItemsNameNoExt}_{DateTimeOffset.Now.ToUnixTimeMilliseconds()}.json";
            if (additionalRequiredItems.Count() > 0)
                File.WriteAllText(additionalRequiredItemsName, System.Text.Json.JsonSerializer.Serialize(additionalRequiredItems));
        }

        private void StartSubscribing()
        {
            for(int i = 0; i < workshopUrls.Count; i++)
            {
                SubscribeToItem(workshopUrls[i]);
            }
        }

        private void SubscribeToItem(string workshopItemUrl)
        {
            webDriver.Navigate().GoToUrl(workshopItemUrl);
            webDriver.FindElement(By.Id("SubscribeItemBtn"));

            IWebElement elementToClick = null;
            Func<string, bool> IsElementVisible = delegate (string idSelector)
            {
                elementToClick = webDriver.FindElement(By.Id(idSelector));
                return elementToClick.Displayed && elementToClick.Enabled;
            };
            if (IsElementVisible("SubscribeItemOptionAdd"))
                elementToClick.Click();
            Thread.Sleep(500);

            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
            try
            {
                IWebElement newModal = webDriver.FindElement(By.ClassName("newmodal"));
                var additionalATags = webDriver.FindElement(By.Id("RequiredItems")).FindElements(By.TagName("a"));

                List<string> additionalItems = new List<string>();
                additionalItems = additionalItems.Concat(additionalATags.ToList().Select(a => a.GetAttribute("href"))).ToList();
                additionalItems.Add(workshopItemUrl);

                additionalRequiredItems = additionalRequiredItems.Concat(additionalItems);
            }
            catch (NoSuchElementException) {  }
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(driverTimeoutSeconds);
        }

        private void UserSignIn()
        {
            Console.WriteLine();
            Console.WriteLine("Log in to Steam and press enter when you've navigated to your Steam profile page.");
            Console.WriteLine();
            webDriver.Navigate().GoToUrl("https://steamcommunity.com/login/home/");
            Console.ReadLine();
        }

        private void InitializeWebDriver()
        {
            switch (browserType)
            {
                case BrowserType.Chrome:
                    webDriver = new ChromeDriver(driverPath);
                    break;
                case BrowserType.Firefox:
                    try
                    {
                        webDriver = new FirefoxDriver(driverPath);
                    }
                    catch(Exception ex) { Console.WriteLine($"Exception: {ex.Message}"); }
                    break;
            }
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(driverTimeoutSeconds);
        }

        public void SelectBrowser()
        {
            Console.WriteLine("Select your browser type:");
            var browserTypeNames = Enum.GetNames(typeof(BrowserType)).Cast<string>().ToList();
            var browserTypeValues = Enum.GetValues(typeof(BrowserType)).Cast<BrowserType>().ToList();
            var browserTypeDict = browserTypeValues.Zip(browserTypeNames).ToDictionary(tup => tup.First, tup => tup.Second);
            foreach(var kvp in browserTypeDict)
            {
                Console.WriteLine($"{kvp.Value} - {(int)kvp.Key}");
            }
            BrowserType response = BrowserType.Unknown;
            do
            {
                response = (BrowserType)Convert.ToInt32(Console.ReadLine());
            } while (!browserTypeValues.Contains(response));
            browserType = response;
        }

        public void SelectBrowserVersion()
        {
            string driverDirectory = null;
            if(browserType == BrowserType.Chrome)
            {
                driverDirectory = "chromedrivers";
            }
            else if(browserType == BrowserType.Firefox)
            {
                driverDirectory = "geckodrivers";
            }
            List<string> versionPaths = Directory.GetDirectories(driverDirectory).ToList();
            List<string> versionNames = versionPaths.Select(d => Path.GetFileName(d)).ToList();
            Console.WriteLine("Select your browser version: ");
            for(int i = 0; i < versionNames.Count; i++)
            {
                Console.WriteLine($"{versionNames[i]} - {i}");
            }
            int response = 0;
            do
            {
                response = Convert.ToInt32(Console.ReadLine());
            } while (!versionNames.Contains(versionNames[response]));
            driverPath = versionPaths[response];
        }

    }
}
