using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketClientAndServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Intro message
            Console.WriteLine("You are running Socket Programming Version 3.3: \n" +
                "- Supports Client and Server roles.\n" +
                "- Real-time chat (both sides send & receive instantly).\n" +
                "- Works over TCP sockets on LAN.\n");

            // Show role options
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Please select your ROLE (enter 1 or 2):");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" 1. Client");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" 2. Server");
            Console.ForegroundColor = ConsoleColor.White;

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    showIpAddress();
                    runAsClient();
                    break;
                case "2":
                    showIpAddress();
                    runAsServer();
                    break;
                default:
                    Console.WriteLine("Invalid choice, exiting...");
                    break;
            }
        }

        /// <summary>
        /// CLIENT SIDE
        /// - Connects to a server IP:Port
        /// - Runs 2 loops:
        ///   (1) Input loop: read user input & send to server
        ///   (2) Listening loop: always receives & prints server messages
        /// </summary>
        static void runAsClient()
        {
            try
            {
                // Ask for server IP
                Console.WriteLine("\nEnter the SERVER's IP (x.x.x.x):");
                string ipServer = Console.ReadLine();
                IPAddress ipAddr = IPAddress.Parse(ipServer);

                // Ask for port (default 8888)
                Console.WriteLine("Enter the PORT you want to use (1024-65535, default=8888):");
                int portToRun;
                if (!int.TryParse(Console.ReadLine(), out portToRun) || portToRun < 1024 || portToRun > 65535)
                {
                    portToRun = 8888;
                }

                // Connect socket
                IPEndPoint remoteEndPoint = new IPEndPoint(ipAddr, portToRun);
                Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                sender.Connect(remoteEndPoint);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Connected to Server -> {0}", sender.RemoteEndPoint.ToString());

                // THREAD 1: Listening loop (background task)
                Task.Run(() =>
                {
                    while (true)
                    {
                        try
                        {
                            byte[] buffer = new byte[1024];
                            int byteRecv = sender.Receive(buffer);
                            string serverMsg = Encoding.ASCII.GetString(buffer, 0, byteRecv);

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("\nServer: " + serverMsg);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("Client> "); // keep input prompt visible
                        }
                        catch
                        {
                            Console.WriteLine("\nConnection closed by server.");
                            break;
                        }
                    }
                });

                // THREAD 2 (main): Input loop (send messages)
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Client> ");
                    string userMes = Console.ReadLine();

                    if (!string.IsNullOrEmpty(userMes))
                    {
                        byte[] messageSent = Encoding.ASCII.GetBytes(userMes + " <EOF>");
                        sender.Send(messageSent);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Client error: " + e.Message);
            }
        }

        /// <summary>
        /// SERVER SIDE
        /// - Binds to local IP + Port
        /// - Accepts one client
        /// - Runs 2 loops:
        ///   (1) Input loop: server user types messages to send
        ///   (2) Listening loop: always receives & prints client messages
        /// </summary>
        static void runAsServer()
        {
            try
            {
                // Get available local IP addresses
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                Console.WriteLine("Enter the IP index to bind for SERVER Role:");
                int addOrder = Int16.Parse(Console.ReadLine());
                IPAddress ipAddr = ipHost.AddressList[addOrder];
                Console.WriteLine("You are using IP:" + ipAddr.ToString());

                // Ask for port (default 8888)
                Console.WriteLine("Enter the PORT you want to use (1024-65535, default=8888):");
                int portToRun;
                if (!int.TryParse(Console.ReadLine(), out portToRun) || portToRun < 1024 || portToRun > 65535)
                {
                    portToRun = 8888;
                }
                Console.WriteLine("You selected PORT:" + portToRun);

                // Create endpoint and socket
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, portToRun);
                Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                listener.Bind(localEndPoint);
                listener.Listen(10);

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Waiting for client connection...");

                // Accept one client
                Socket clientSocket = listener.Accept();
                Console.WriteLine("Client connected!");

                // THREAD 1: Input loop (server typing)
                Task.Run(() =>
                {
                    while (true)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("Server> ");
                        string serverMsg = Console.ReadLine();

                        if (!string.IsNullOrEmpty(serverMsg))
                        {
                            byte[] message = Encoding.ASCII.GetBytes(serverMsg + " <EOF>");
                            clientSocket.Send(message);
                        }
                    }
                });

                // THREAD 2 (main): Listening loop (receive messages)
                while (true)
                {
                    try
                    {
                        byte[] buffer = new byte[1024];
                        int numByte = clientSocket.Receive(buffer);
                        string data = Encoding.ASCII.GetString(buffer, 0, numByte);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nClient: {0}", data);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("Server> "); // keep prompt visible
                    }
                    catch
                    {
                        Console.WriteLine("\nClient disconnected.");
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Server error: " + e.Message);
            }
        }

        /// <summary>
        /// Utility function:
        /// Displays all local IP addresses so the user can select the correct one.
        /// </summary>
        static void showIpAddress()
        {
            string strHostName = Dns.GetHostName();
            Console.WriteLine("Local Machine's Host Name: " + strHostName);

            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;

            for (int i = 0; i < addr.Length; i++)
            {
                Console.WriteLine("IP Address {0}: {1}", i, addr[i].ToString());
            }
        }
    }
}
