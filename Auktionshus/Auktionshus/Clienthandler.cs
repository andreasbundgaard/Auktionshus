using System;
using System.IO;
using System.Net.Sockets;

namespace Auktionshuset
{
    internal class Clienthandler
    {
        private readonly Auctions _auction;
        private readonly Socket _client;
        private string _clientName;
        private bool done;

        public Clienthandler(Socket client, Auctions auction)
        {
            _auction = auction;
            _client = client;

            _auction.broadcastEvent += _auction_broadcastEvent;     //Forbinder sig til broadcasten
        }

        private void _auction_broadcastEvent(string message)        //Opsætter en stream til at sende og modtage in- og output til broadcasten
        {
            var stream = new NetworkStream(_client);
            var writer = new StreamWriter(stream);
            writer.AutoFlush = true;
            writer.WriteLine(message);
        }

        public void RunClient()         //Opsætter en stream til at sende og modtage in- og output mellem server og klient
        {
            var stream = new NetworkStream(_client);
            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            writer.WriteLine("Skriv dit navn:");        //Beder byderen om at indtaste sit navn

            _clientName = reader.ReadLine();

            writer.WriteLine("Velkommen til! {0}\r\nSkriv 'farvel' for at lukke auktionshuset.\r\n'byd' og dit bud, for at byde.", _clientName);

            done = false;
            while (!done)
            {
                try             //Hvis der er en byder der er forbundet, broadcaster den følgende
                {
                    string[] commands = reader.ReadLine().Split(' ');

                    switch (commands[0])        //Broadcaster hvilke kommandoer byderen kan gøre brug af
                    {
                        case "farvel":              //Forbindelsen til klienten ophører
                            _auction.broadcastEvent -= _auction_broadcastEvent;
                            writer.WriteLine("Tak for denne gang, på gensyn!...");
                            done = true;
                            break;
                        case "byd":                 //Modtager bud fra klienten
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

            //Der bliver lukket for stream og socket
            writer.Close();
            reader.Close();
            stream.Close();
            _client.Close();
        }
    }
}
