using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Auktionshuset
{
	internal class Auction
	{
		public delegate void broadcastDelegate(string message); //Laver en ny delegat til broadcast

		private readonly List<AuctionItem> auctionItems = new List<AuctionItem>();
			//Laver en liste med varer der skal på auktion

		private readonly object auctionariousLock = new object();
		private readonly object itemLock = new object();
		private bool _auctionRunning;
		private AuctionItem _currentAuction;
		private int _auctionarious;

		public Auction() //Tilføjer varer til der skal på auktion til listen
		{
			auctionItems.Add(new AuctionItem
			{
				winner = "Ingen",
				startPrice = 50,
				highestBid = 50,
				item = "En spejderdolk"
			});
			auctionItems.Add(new AuctionItem
			{
				winner = "Ingen",
				startPrice = 1000,
				highestBid = 1000,
				item = "En iPhone 6 Plus"
			});
			auctionItems.Add(new AuctionItem
			{
				winner = "Ingen",
				startPrice = 30,
				highestBid = 30,
				item = "20 kg. æbler"
			});
			auctionItems.Add(new AuctionItem
			{
				winner = "Ingen",
				startPrice = 100,
				highestBid = 100,
				item = "En ekslusiv pool-kø"
			});
		}

		public event broadcastDelegate broadcastEvent;

		public void RunAuction() //Tager varerne fra listen, og starter en ny auktion på den pågældende vare
		{
			foreach (AuctionItem auctionItem in auctionItems)
			{
				_currentAuction = auctionItem;
				_auctionRunning = true;
				ResetAuctionarious();

				if (broadcastEvent != null)
					broadcastEvent("Starter auktion for: " + _currentAuction.item + " Start pris: " +
					               _currentAuction.startPrice);
				Console.WriteLine("Starter auktion for: " + _currentAuction.item + " Start pris: " + _currentAuction.startPrice);

				while (_auctionRunning)
					//Hammerslag, der efter 10 sekunder(første gang), 5 sekunder (anden gang) og 0 sekunder(solgt)
				{
					Thread.Sleep(1000);

					lock (auctionariousLock) //Låser auktionarius
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
					//Kigger på om der er nogen som har budt. Hvis ikke, så skriver den at ingen har budt.
					if (_currentAuction.highestBid <= _currentAuction.startPrice)
					{
						broadcastEvent(_currentAuction.winner + " havde det højeste bud\nIngen vandt " + _currentAuction.item);
						Console.WriteLine(_currentAuction.winner + " havde det højeste bud\nIngen vandt " + _currentAuction.item);
					}
					else
					{
						//Ellers skriver den hvem der har det højeste bud på den pågældende auktion, og hvor stort buddet er
						broadcastEvent(_currentAuction.winner + " havde det højeste bud på\r\n" + _currentAuction.item + " : " +
													 _currentAuction.highestBid + " kr.");
						Console.WriteLine(_currentAuction.winner + " havde det højeste bud på\r\n" + _currentAuction.item + " : " +
															_currentAuction.highestBid + " kr.");
					}

			}

			if (broadcastEvent != null) //Broadcaster at auktionen nu er slut
				broadcastEvent("Tak fordi du brugte Auktionarous!");
		}

		private void ResetAuctionarious() //Nulstiller auktionarius 
		{
			lock (auctionariousLock)
			{
				_auctionarious = 30;
			}
		}

		public string Bid(string name, int amount) //Kigger på hvem der har budt, og hvor meget buddet lyder på.
		{
			lock (itemLock) //Låser den pågældende auktion for at tage imod bud
			{
				if (_currentAuction.highestBid < amount)
				{
					broadcastEvent(name + " bud " + amount);
					_currentAuction.highestBid = amount;
					_currentAuction.winner = name;
					ResetAuctionarious();
				}
				else
				{
					return "Budet er for lavt";
				}
			}

			return "Du byder nu : " + amount; //Returnerer information om det pågældende bud til klienten
		}

		public string GetCurrentAuctionItemInfo()
		{
			lock (itemLock) //Låser den pågældende auktion for at tage imod bud
			{
				return "Genstand til salg lige nu: " + _currentAuction.item
				       + " \nStart pris: " + _currentAuction.startPrice
				       + " \nHøjeste bud: " + _currentAuction.highestBid 
							 + " \nNuværende vinder: " + _currentAuction.winner;
			}
		}

		internal class AuctionItem //En intern klasse, med forskellige properties
		{
			public int startPrice { get; set; }
			public int highestBid { get; set; }
			public string winner { get; set; }
			public string item { get; set; }
		}
	}
}
