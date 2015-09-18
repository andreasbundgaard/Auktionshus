using System;
using Auktionshuset;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AuctionTest
{
    [TestClass]
    public class AuctionTest
    {
        [TestMethod]
        public void BidTest()
        {
						Auction auction = new Auction();
						auction.Bid("Sebastian", 1000);

						
        }
    }
}
