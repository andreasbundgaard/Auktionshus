using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Auktionshuset
{
    internal class Program
    {
        public static Auctions _auction = new Auctions();

        static void Main(string[] args)
        {
            new SocketServer(11100);
        }

        private class SocketServer
        {
            public SocketServer(int port)
            {
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                var server = new TcpListener(ip, port);
                server.Start();

                Console.WriteLine("Server er klar på port " + port);

                var auctionThread = new Thread(_auction.RunAuction);        // Starter en ny tråd til auktionshuset
                auctionThread.Start();

                while (true)
                {
                    Socket clientSocket = server.AcceptSocket();
                    Console.WriteLine("En byder blev forbundet...");

                    var handler = new Clienthandler(clientSocket, _auction);

                    var clientThread = new Thread(handler.RunClient);       // Starter en ny tråd til klienten
                    clientThread.Start();
                }
            }
        }
    }
}
