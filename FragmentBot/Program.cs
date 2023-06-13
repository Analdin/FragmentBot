using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FragmentBot
{
    internal class Program
    {
        public static Random rnd = new Random();
        public static string TgName1 { get; set; }
        public static string TgName { get; set; }
        public static string Bid { get; set; }
        public static string AuctionEnd { get; set; }
        public static string BidAddr { get; set; }
        public static string auctionBid { get; set; }

        static async Task Main(string[] args)
        {
            // Собираем данные со страницы
            Requests.RqToFragment run = new Requests.RqToFragment();

            Console.WriteLine("Запрос имен с ТГ");
            run.getTGUserName();

            Console.WriteLine("Запрос ставки");
            run.GetMinBid();

            Console.WriteLine("Запрос времени окончания аукциона");
            run.GetAuctionEnd();

            Console.WriteLine("Запрос бид адреса");
            run.GetAddBidAddr();

            Console.WriteLine("Запрос времени старта аукциона");
            run.AuctionStarted();

            // Отправка всех данных в ТГ
            Requests.TelegramAlert str = new Requests.TelegramAlert();

            for (int i = 0; i < Requests.RqToFragment.NameLst.Count; i++)
            {
                Thread.Sleep(rnd.Next(4000, 6000));

                TgName1 = Requests.RqToFragment.NameLst[i];
                Bid = Requests.RqToFragment.BidLst[i];
                AuctionEnd = Requests.RqToFragment.AuctionLst[i];
                TgName = Requests.RqToFragment.NameLst[i];
                BidAddr = Requests.RqToFragment.BidsAddr[i];
                auctionBid = Requests.RqToFragment.AucSt[i];

                TgName = TgName.Remove(0, 1);

                //BidAddr = BidAddr.Replace("-", "\\-");

                await str.BotStart(TgName1, Bid, AuctionEnd, BidAddr, $"https://fragment.com/", auctionBid);
            }
        }
    }
}
