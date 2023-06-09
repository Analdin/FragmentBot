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
        public static string TgName { get; set; }
        public static string Bid { get; set; }
        public static string AuctionEnd { get; set; }
        public static string BidAddr { get; set; }

        static void Main(string[] args)
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


            for(int i = 0; i < Requests.RqToFragment.NameLst.Count; i++)
            {
                TgName = Requests.RqToFragment.NameLst[i];
                Bid = Requests.RqToFragment.BidLst[i];
                AuctionEnd = Requests.RqToFragment.AuctionLst[i];
                TgName = Requests.RqToFragment.NameLst[i];
                BidAddr = Requests.RqToFragment.BidsAddr[i];

                // Отправка всех данных в ТГ
                Requests.TelegramAlert str = new Requests.TelegramAlert();
                str.BotStart(TgName, Bid, AuctionEnd, BidAddr);

                //Thread.Sleep(2000);

                //Requests.RqToFragment.NameLst.RemoveAt(0);
                //Requests.RqToFragment.BidLst.RemoveAt(0);
                //Requests.RqToFragment.AuctionLst.RemoveAt(0);
                //Requests.RqToFragment.NameLst.RemoveAt(0);
                //Requests.RqToFragment.NameLst.Add(TgName);
                //Requests.RqToFragment.BidLst.Add(Bid);
                //Requests.RqToFragment.AuctionLst.Add(AuctionEnd);
                //Requests.RqToFragment.NameLst.Add(BidAddr);
            }
        }
    }
}
