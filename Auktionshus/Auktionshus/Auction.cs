using System;
using System.Collections.Generic;
using System.Threading;

namespace Auktionshuset
{
    internal class Auctions
    {
        public delegate void broadcastDelegate(string message);

        private readonly List<AuctionItem> auctionItems = new List<AuctionItem>();

        private readonly object auctionariousLock = new object();
        private readonly object itemLock = new object();
        private bool _auctionRunning;
        private AuctionItem _currentAuction;
        private int _auctionarious;

        public Auctions()
        {
            auctionItems.Add(new AuctionItem
            {
                winner = "Ingen",
                startPrice = 100,
                endPrice = 100,
                item = "En spejderdolk"
            });
            auctionItems.Add(new AuctionItem
            {
                winner = "Ingen",
                startPrice = 100,
                endPrice = 100,
                item = "En iPhone 6 Plus"
            });
            auctionItems.Add(new AuctionItem
            {
                winner = "Ingen",
                startPrice = 100,
                endPrice = 100,
                item = "20 kg. æbler"
            });
            auctionItems.Add(new AuctionItem
            {
                winner = "Ingen",
                startPrice = 100,
                endPrice = 100,
                item = "En ekslusiv pool-kø"
            });
        }

        public event broadcastDelegate broadcastEvent;

        public void RunAuction()
        {
            foreach (AuctionItem auctionItem in auctionItems)
            {
                _currentAuction = auctionItem;
                _auctionRunning = true;
                ResetAuctionarious();

                if (broadcastEvent != null)
                    broadcastEvent("Starter auktion for: " + _currentAuction.item + " Start pris: " +
                                   _currentAuction.startPrice);

                while (_auctionRunning)
                {
                    Thread.Sleep(1000);

                    lock (auctionariousLock)
                    {
                        _auctionarious--;

                        if (_auctionarious == 10)
                        {
                            if (broadcastEvent != null)
                                broadcastEvent("Auktionarius: Første gang!");
                            Console.WriteLine("Auktionarius: Første gang!");
                        }
                        else if (_auctionarious == 5)
                        {
                            if (broadcastEvent != null)
                                broadcastEvent("Auktionarius: Anden gang!");
                            Console.WriteLine("Auktionarius: Anden gang!");
                        }
                        else if (_auctionarious == 0)
                        {
                            if (broadcastEvent != null)
                                broadcastEvent("Auktionarius: Solgt!");
                            Console.WriteLine("Auktionarius: Solgt!");
                            _auctionRunning = false;
                        }
                    }
                }
                if (broadcastEvent != null)
                    broadcastEvent(_currentAuction.winner + " havde det højeste bud på\r\n" + _currentAuction.item + " : " +
                                   _currentAuction.endPrice + " kr.");
            }

            if (broadcastEvent != null)
                broadcastEvent("Dette var dagens auktioner - Tak fordi du brugte Auktionshuset - på gensyn!");
        }

        private void ResetAuctionarious()
        {
            lock (auctionariousLock)
            {
                _auctionarious = 30;
            }
        }

        public string Bid(string name, int amount)
        {
            lock (itemLock)
            {
                if (_currentAuction.endPrice < amount)
                {
                    broadcastEvent(name + " bud " + amount);
                    _currentAuction.endPrice = amount;
                    _currentAuction.winner = name;
                    ResetAuctionarious();
                }
                else
                {
                    return "Bud for lavt";
                }
            }

            return "Du byder nu : " + amount;
        }
    }

    internal class AuctionItem
    {
        public int startPrice { get; set; }
        public int endPrice { get; set; }
        public string winner { get; set; }
        public string item { get; set; }
    }
}
