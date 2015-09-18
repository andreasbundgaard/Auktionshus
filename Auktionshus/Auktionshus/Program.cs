using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Auktionshuset
{
    internal class Program
    {
        public static Auction _auction = new Auction();

        static void Main(string[] args)
        {
            new SocketServer(20001);
        }

        private class SocketServer
        {
            public SocketServer(int port)
            {
                var server = new TcpListener(IPAddress.Any, port);
                server.Start();

                Console.WriteLine("Server er klar på port " + port);

                var auctionThread = new Thread(_auction.RunAuction);        // Starter en ny tråd til auktionshuset
                auctionThread.Start();

                while (true)
                {
                    Socket clientSocket = server.AcceptSocket();
                    Console.WriteLine("En byder blev forbundet...");

                    var handler = new Clienthandler(clientSocket, _auction);    //Initialisere et objekt af typen Clienthandler

                    var clientThread = new Thread(handler.RunClient);       // Starter en ny tråd til klienten
                    clientThread.Start();
                }
            }
        }
    }
}
