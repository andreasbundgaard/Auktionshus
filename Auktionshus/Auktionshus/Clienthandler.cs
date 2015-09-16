using System;
using System.IO;
using System.Net.Sockets;

namespace Auktionshuset
{
    internal class Clienthandler
    {
        private readonly Auction _auction;
        private readonly Socket _client;
        private string _clientName;
        private bool done;

        public Clienthandler(Socket client, Auction auction)
        {
            _auction = auction;
            _client = client;

            _auction.broadcastEvent += _auction_broadcastEvent;     //Forbinder sig til klienten
        }

        private void _auction_broadcastEvent(string message)
        {
            var stream = new NetworkStream(_client);
            var writer = new StreamWriter(stream);
            writer.AutoFlush = true;
            writer.WriteLine(message);
        }

        public void RunClient()
        {
            var stream = new NetworkStream(_client);
            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            writer.WriteLine("Skriv dit navn:");

            _clientName = reader.ReadLine();

            writer.WriteLine("Velkommen til! {0}\r\nSkriv 'farvel' for at lukke auktionshuset.\r\n'byd' og dit bud, for at byde.", _clientName);

            done = false;
            while (!done)
            {
                try
                {
                    string[] commands = reader.ReadLine().Split(' ');

                    switch (commands[0])
                    {
                        case "farvel":
                            _auction.broadcastEvent -= _auction_broadcastEvent;
                            writer.WriteLine("Tak for denne gang, på gensyn!...");
                            done = true;
                            break;
                        case "byd":
                            string bidString = _auction.Bid(_clientName, int.Parse(commands[1]));

                            writer.WriteLine(bidString);
                            break;
                        default:

                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Expection thrown by Clienthandler - Command(): {0}", ex);
                    done = true;
                }
            }

            writer.Close();
            reader.Close();
            stream.Close();
            _client.Close();
        }
    }
}
