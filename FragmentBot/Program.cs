using FragmentBot.Requests;
using MySql.Data.MySqlClient;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using WbHelperDB;

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
        //
        public static string id { get; set; }
        public static string bid2 { get; set; }
        public static string Uname { get; set; }
        public static string bid3 { get; set; }

        public static string tonDom { get; set; }
        public static string curBidTon { get; set; }
        public static string curDateTon { get; set; }

        static async Task Main(string[] args)
        {
            // Собираем данные со страницы
            RqToFragment run = new RqToFragment();
            DbHelper db = new DbHelper();

            bool isCtrlPressed = false;

            Console.WriteLine("Нажмите Space + Q для выхода");

            db.OpenConnection();

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                    if (keyInfo.Key == ConsoleKey.Spacebar)
                    {
                        isCtrlPressed = true;
                    }
                    else if (isCtrlPressed && keyInfo.Key == ConsoleKey.Q)
                    {
                        Console.WriteLine("Программа остановлена");
                        break; // Прерывание цикла при нажатии Ctrl + Q
                    }
                    else
                    {
                        isCtrlPressed = false;
                    }
                }

                Console.WriteLine("Запрос TON значений");
                TonJob run2 = new TonJob();

                run2.TonParserDomain();
                run2.TonParserDate();
                run2.TonParserPrice();

                Console.WriteLine("Запрос sold");
                run.getSoldData();

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
                TelegramAlert str = new TelegramAlert();

                // Отправка TON адресов в ТГ
                for (int y = 0; y < TonJob.TonDomainLst.Count; y++)
                {
                    tonDom = TonJob.TonDomainLst[y];
                    curBidTon = TonJob.TonBidLst[y];
                    curDateTon = TonJob.TonDateLst[y];

                    Thread.Sleep(rnd.Next(3000, 5000));

                    await str.TonInToTG(tonDom, curBidTon, curDateTon, DateTime.Now, $"https://dns.ton.org/#{tonDom}");
                }

                for (int i = 0; i < RqToFragment.NameLst.Count; i++)
                {
                    await Task.Delay(rnd.Next(1000, 2000));

                    // Проверить наличие такого же значения в бд
                    // Если нет - записать
                    // Если есть - обновить ставку и вывести новую

                    TgName1 = RqToFragment.NameLst[i];
                    Bid = RqToFragment.BidLst[i];
                    AuctionEnd = RqToFragment.AuctionLst[i];
                    TgName = RqToFragment.NameLst[i];
                    BidAddr = RqToFragment.BidsAddr[i];
                    auctionBid = RqToFragment.AucSt[i];

                    TgName = TgName.Remove(0, 1);

                    // Выборка с базы
                    string query = $"SELECT `id`, `newBid`, `Username`,`CurBid` FROM `bids` WHERE `Username` = '{TgName}'";
                    var command = new MySqlCommand(query, db.Connection);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        id = reader.GetString(0);
                        bid2 = reader.GetString(1);
                        Uname = reader.GetString(2);
                        bid3 = reader.GetString(3);
                    }
                    reader.Close();

                    decimal bidValue;
                    decimal bid2Value;
                    if (decimal.TryParse(Bid, out bidValue) && decimal.TryParse(bid2, out bid2Value))
                    {
                        if (bidValue > bid2Value)
                        {
                            Requests.Conditon set = new Requests.Conditon();
                            set.BidUp = true;

                            MySqlCommand upBd = new MySqlCommand($@"UPDATE `bids`
                        SET `newBid` = IF(`newBid` < {Bid}, {Bid}, `newBid`)
                        WHERE `id` = {id}", db.Connection);

                            upBd.ExecuteNonQuery();

                            await str.BotStart(bidValue.ToString(), TgName1, bid2Value.ToString(), AuctionEnd, BidAddr, $"https://fragment.com/username/{TgName}", auctionBid);
                        }
                        else
                        {
                            // Проверяем есть ли запись с таким Username в бд
                            //MySqlCommand strYes = new MySqlCommand($@"SELECT COUNT(*) FROM `bids` WHERE `Username` = '{TgName}'");
                            //strYes.ExecuteNonQuery();

                            using (var cmd = new MySqlCommand($@"SELECT COUNT(*) FROM `bids` WHERE `Username` = '{TgName}'", db.Connection))
                            {
                                cmd.Parameters.AddWithValue("@Username", TgName);
                                int count = Convert.ToInt32(cmd.ExecuteScalar());

                                if (count > 0)
                                {
                                    Console.WriteLine($"Запись для ника {TgName}, уже есть в базе");
                                }
                                else
                                {
                                    Requests.Conditon set = new Requests.Conditon();
                                    set.BidUp = false;

                                    await str.BotStart(bidValue.ToString(), TgName1, bid2Value.ToString(), AuctionEnd, BidAddr, $"https://fragment.com/username/{TgName}", auctionBid);

                                    MySqlCommand countPlus = new MySqlCommand($@"INSERT INTO `bids` (`AuctionStarted`, `newBid`, `Username`, `CurBid`, `Wallet`, `AuctionEnd`) 
                                VALUES('{auctionBid}','{Bid}','{TgName}','{Bid}','{BidAddr}','{AuctionEnd}')", db.Connection);

                                    countPlus.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    else
                    {
                        // Обработка случая, когда преобразование не удалось
                        // Можно выбрать соответствующее действие или сообщение об ошибке
                    }
                }
                Console.WriteLine("Собрали все ставки, переходим к мониторингу.");
                db.CloseConnection();
            }
        }
    }
}
