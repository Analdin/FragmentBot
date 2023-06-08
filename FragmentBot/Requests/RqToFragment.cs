using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FragmentBot.Requests
{
    public class RqToFragment
    {
        HttpClient client = new HttpClient();
        public static List<string> NameLst = new List<string>();
        public static List<string> BidLst = new List<string>();
        public static List<string> AuctionLst = new List<string>();
        public static List<string> BidsAddr = new List<string>();

        public void getTGUserName()
        {
            // Отправляем GET-запрос и получаем ответ
            HttpResponseMessage response = client.GetAsync("https://fragment.com/").Result;
            response.EnsureSuccessStatusCode();

            // Извлекаем содержимое страницы
            string content = response.Content.ReadAsStringAsync().Result;
            string pattern = @"(?<=tm-value"">).*(?=</div>)";
            MatchCollection matchNames = Regex.Matches(content, pattern);

            foreach(Match elm in matchNames)
            {
                NameLst.Add(elm.Value);
                Console.WriteLine("Имя ТГ - " + elm.Value);
            }
        }

        public void GetMinBid()
        {
            // Отправляем GET-запрос и получаем ответ
            HttpResponseMessage response = client.GetAsync("https://fragment.com/").Result;
            response.EnsureSuccessStatusCode();

            // Извлекаем содержимое страницы
            string content = response.Content.ReadAsStringAsync().Result;
            string pattern = @"(?<=tm-value\ icon-before\ icon-ton"">).*(?=</div>)";
            MatchCollection matchBids = Regex.Matches(content, pattern);

            foreach(Match elm in matchBids)
            {
                BidLst.Add(elm.Value);
                Console.WriteLine("Минимальная ставка - " + elm.Value);
            }
        }

        public void GetAuctionEnd()
        {
            // Отправляем GET-запрос и получаем ответ
            HttpResponseMessage response = client.GetAsync("https://fragment.com/").Result;
            response.EnsureSuccessStatusCode();

            // Извлекаем содержимое страницы
            string content = response.Content.ReadAsStringAsync().Result;
            string pattern = @"(?<=data-relative=""short-text"">).*(?=</time>)";
            MatchCollection matchTime = Regex.Matches(content, pattern);

            foreach (Match elm in matchTime)
            {
                AuctionLst.Add(elm.Value);
                Console.WriteLine("Время до окончания аукциона - " + elm.Value);
            }
        }

        public void GetAddBidAddr()
        {
            foreach(var elm in NameLst)
            {
                string elm1 = elm.Remove(0, 1);
                // Отправляем GET-запрос и получаем ответ
                HttpResponseMessage response = client.GetAsync($"https://fragment.com/username/{elm1}").Result;
                Console.WriteLine($"https://fragment.com/username/{elm1}");

                response.EnsureSuccessStatusCode();

                // Извлекаем содержимое страницы
                string content = response.Content.ReadAsStringAsync().Result;
                string pattern = @"(?<=tonviewer\.com/).*(?=""\ class=""tm-wallet)";
                string bidAddr = Regex.Match(content, pattern).ToString();

                BidsAddr.Add(bidAddr);

                Console.WriteLine($"Бид адрес - {bidAddr}");
            }
        }
    }
}
