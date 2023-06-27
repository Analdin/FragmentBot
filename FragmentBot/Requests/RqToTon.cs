using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FragmentBot.Requests
{
    public class TonJob
    {
        IWebDriver driver = new ChromeDriver();
        HttpClient client = new HttpClient();
        public static List<string> TonDomainLst = new List<string>();
        public static List<string> TonBidLst = new List<string>();
        public static List<string> TonDateLst = new List<string>();
        public void TonParserDomain()
        {
            driver.Navigate().GoToUrl("https://tonviewer.com/auctions?tld=ton");
            Thread.Sleep(25000);

            List<IWebElement> tonDomains = driver.FindElements(By.XPath("//div[contains(@class, 'grid__GridItem')]/a[contains(@href, 'bids')]")).ToList();
            if(tonDomains.Count > 0)
            {
                foreach(var elm in tonDomains)
                {
                    TonDomainLst.Add(elm.Text);
                }
            }

            Thread.Sleep(2000);
        }

        public void TonParserPrice()
        {
            try
            {
                List<IWebElement> tonDomains = driver.FindElements(By.XPath("//div[contains(@class, 'grid__GridItem')]/div/span[contains(@class, 'text__Text-sc')]")).ToList();
                if (tonDomains.Count > 0)
                {
                    foreach (var elm in tonDomains)
                    {
                        TonBidLst.Add(elm.Text);
                    }
                }

                Thread.Sleep(2000);

                tonDomains.RemoveAll(elm => elm.Text.Contains("bid"));

                Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }
            driver.Quit();
        }

        public void TonParserDate()
        {
            try
            {
                List<IWebElement> tonDates = driver.FindElements(By.XPath("//div[contains(@class, 'row__Row-sc')]/span[contains(@class, 'text__Text-sc')]")).ToList();
                if (tonDates.Count > 0)
                {
                    foreach (var elm in tonDates)
                    {
                        TonDateLst.Add(elm.Text);
                    }
                }

                Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }
        }

    }
}
