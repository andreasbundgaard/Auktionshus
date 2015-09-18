using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading;

namespace AuctionTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            private readonly List<AuctionItem> auctionItems = new List<AuctionItem>(); 

            public Auctions()       //Tilføjer varer til der skal på auktion til listen
        {
            auctionItems.Add(new AuctionItem
            {
                winner = "Ingen",
                startPrice = 50,
                endPrice = 200,
                item = "En spejderdolk"
            });
            auctionItems.Add(new AuctionItem
            {
                winner = "Ingen",
                startPrice = 1000,
                endPrice = 2500,
                item = "En iPhone 6 Plus"
            });
            auctionItems.Add(new AuctionItem
            {
                winner = "Ingen",
                startPrice = 30,
                endPrice = 55,
                item = "20 kg. æbler"
            });
            auctionItems.Add(new AuctionItem
            {
                winner = "Ingen",
                startPrice = 100,
                endPrice = 300,
                item = "En ekslusiv pool-kø"
            });
        }
        }
    }
}
