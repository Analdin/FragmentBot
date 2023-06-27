using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FragmentBot.Requests
{
    public class RqToFragment
    {
        HttpClient client = new HttpClient();
        public static List<string> NameLst = new List<string>();
        public static List<string> BidLst = new List<string>();
        public static List<string> AuctionLst = new List<string>();
        public static List<string> BidsAddr = new List<string>();
        public static List<string> AucSt = new List<string>();

        public static List<string> numLst = new List<string>();
        public static List<string> tonLst = new List<string>();
        public static List<string> walletLst = new List<string>();
        public static List<string> purchasedDate = new List<string>();

        // Промежуточный список
        public static List<string> buffer = new List<string>();

        public static string text = String.Empty;

        public void getSoldData()
        {
            HttpResponseMessage response = client.GetAsync("https://fragment.com/numbers?sort=ending&filter=sold").Result;
            response.EnsureSuccessStatusCode();

            // Извлекаем содержимое страницы
            string content = response.Content.ReadAsStringAsync().Result;
            string pattern = @"(?<=table-cell-value\ tm-value"">).*(?=</div>)";
            MatchCollection matchNums = Regex.Matches(content, pattern);

            foreach (Match elm in matchNums)
            {
                numLst.Add(elm.Value);
                Console.WriteLine("sold номер - " + elm.Value);
            }

            // ton sold
            HttpResponseMessage response2 = client.GetAsync("https://fragment.com/numbers?sort=ending&filter=sold").Result;
            response.EnsureSuccessStatusCode();

            string content2 = response2.Content.ReadAsStringAsync().Result;
            string pattern2 = @"(?<=\ icon-before\ icon-ton"">).*(?=</div>)";
            MatchCollection matchTons = Regex.Matches(content2, pattern2);

            foreach (Match elm in matchTons)
            {
                numLst.Add(elm.Value);
                Console.WriteLine("sold ton - " + elm.Value);
            }

            // ton wallet sold
            foreach(var elm in numLst)
            {
                string num = elm.Remove(0, 1);
                num = num.Replace(" ", "");
                HttpResponseMessage response3 = client.GetAsync($"https://fragment.com/number/{num}").Result;
                response3.EnsureSuccessStatusCode();

                string content3 = response3.Content.ReadAsStringAsync().Result;
                string pattern3 = @"tonviewer\.com\/(.*?)\bclass\b";
                MatchCollection matchWallet = Regex.Matches(content3, pattern3);

                foreach(Match wl in matchWallet)
                {
                    text = wl.Value.Replace("tonviewer.com/", "").Replace(" class", "");
                    buffer.Add(text);
                }

                walletLst.Add(buffer.FirstOrDefault());
            }
            buffer.Clear();

            // ton time sold
            HttpResponseMessage response4 = client.GetAsync("https://fragment.com/numbers?sort=ending&filter=sold").Result;
            response4.EnsureSuccessStatusCode();

            string content4 = response4.Content.ReadAsStringAsync().Result;
            string pattern4 = @"(?<= table - cell - desc""><time>).*(?=</time>)";
            MatchCollection matchTime = Regex.Matches(content4, pattern4);

            foreach (Match elm in matchTime)
            {
                purchasedDate.Add(elm.Value);
                Console.WriteLine("sold time - " + elm.Value);
            }
        }

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
        public void AuctionStarted()
        {
            foreach(var nm in NameLst)
            {
                string nm1 = nm.Remove(0, 1);
                HttpResponseMessage resp = client.GetAsync($"https://fragment.com/username/{nm1}").Result;
                Console.WriteLine($"https://fragment.com/username/{nm1}");

                resp.EnsureSuccessStatusCode();

                string content = resp.Content.ReadAsStringAsync().Result;
                string pattern = @"(?<=short\"">)[\w\W]*?(?=</time>)";

                string ts = Regex.Match(content, pattern).ToString();

                buffer.Add(ts);

                AucSt.Add(buffer.LastOrDefault());
                Console.WriteLine($"Начало аукциона - {buffer.LastOrDefault()}");
            }
        }
    }
}
