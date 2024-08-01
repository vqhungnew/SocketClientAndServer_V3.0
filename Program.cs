using System;
using System.Collections.Generic;
using System.Linq;
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
            Console.WriteLine("You are running Socket Programming Version 3.0: \n" +
                "- Easy to understand a few technical items.\n" +
                "- BASIC commands.\n" +
                "- No typing CONVERSATION between Server & Client\n" +
                "- MUST ENTER SERVER'S IP to run.\n" +
                "- MUST Select PORT to run.\n" +
                "- CLIENT can Send multi text.Server send Automatically predefined message\n"+
                "- SERVER can Answer clients.\n");

            string role_1 = "Client";
            string role_2 = "Server";
            string your_role = "You choose to be NOTHING. BB and See you again";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nWelcome to Socket programming");
            Console.WriteLine("Please select your ROLE (enter 1 or 2):");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\n 1." + role_1 + "   ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("2." + role_2 + "\n");
            Console.ForegroundColor = ConsoleColor.White;
            string your_choice = null;
            your_choice = Console.ReadLine();   //Wait for a character from keyboard
            switch (your_choice)
            {
                case "1":
                    your_role = role_1;

                    Console.WriteLine("\nYou select your ROLE: " + your_choice + ":" + your_role);
                    showIpAddress();
                    runAsClient();
                    //Select IP to run by ORDER
                   
                    break;
                case "2":
                    your_role = role_2;

                    Console.WriteLine("\nYou select your ROLE: " + your_choice + ":" + your_role);
                    showIpAddress();
                    runAsServer();
                    break;
            }

            Console.WriteLine("\nYou select your ROLE: " + your_choice + ":" + your_role + "\n");


            Console.ReadLine(); // Wait ENTER to exit

        }
        static void runAsClient()
        {

            try
            {

                // Establish the remote endpoint 
                // for the socket. This example 
                // uses port 11111 on the local 
                // computer.
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                Console.WriteLine("\nEnter the SERVER's IP (x.x.x.x) you want to use:");
                //int addOrder = Int16.Parse(Console.ReadLine());
                string ipServer = Console.ReadLine();
                IPAddress ipAddr = IPAddress.Parse(ipServer); //AddressList[addOrder];
                Console.WriteLine("You are using IP:" + ipAddr.ToString());

                //Select PORT to run, default PORT:8888
                Console.WriteLine("Enter the PORT you want to use (1024-65535):");
                int portToRun = Convert.ToInt32(Console.ReadLine());
                if (portToRun > 1023 & portToRun < 65535)
                {
                    //Keep the port USER entered.
                }
                else
                {
                    portToRun = 8888;
                }
                {
                    Console.WriteLine("You selected PORT:" + portToRun);
                }
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, portToRun);

                // Creation TCP/IP Socket using 
                // Socket Class Constructor
                Socket sender = new Socket(ipAddr.AddressFamily,
                           SocketType.Stream, ProtocolType.Tcp);

                try
                {

                    // Connect Socket to the remote 
                    // endpoint using method Connect()
                    sender.Connect(localEndPoint);

                    // We print EndPoint information 
                    // that we are connected
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Socket connected to -> {0} ", sender.RemoteEndPoint.ToString());

                    // Creation of message that
                    // we will send to Server
                    byte[] messageSent = Encoding.ASCII.GetBytes("Test Client<EOF>");
                    int byteSent = sender.Send(messageSent);

                    // Data buffer
                    byte[] messageReceived = new byte[1024];

                    // We receive the message using 
                    // the method Receive(). This 
                    // method returns number of bytes
                    // received, that we'll use to 
                    // convert them to string
                    int byteRecv = sender.Receive(messageReceived);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Message from Server -> {0}", Encoding.ASCII.GetString(messageReceived, 0, byteRecv));


                    while (true)    // Make a LOOP to WAIT user TEXT, after that SEND to Server
                    {
                        //Client send Message to Server/Listenner
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("\n Start CHATING by ENTER Something");

                        Console.ForegroundColor = ConsoleColor.Red;
                        string userMes = Console.ReadLine();

                        messageSent = Encoding.ASCII.GetBytes(userMes + " <EOF>");
                        byteSent = sender.Send(messageSent);

                        //listen form Server
                        byteRecv = sender.Receive(messageReceived);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        //Console.WriteLine("Message from Server -> {0}",Encoding.ASCII.GetString(messageReceived,0, byteRecv));
                        Console.WriteLine("Server:", Encoding.ASCII.GetString(messageReceived, 0, byteRecv));

                    }

                    // Close Socket using 
                    // the method Close()
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }

                // Manage of Socket's Exceptions
                catch (ArgumentNullException ane)
                {

                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }

                catch (SocketException se)
                {

                    Console.WriteLine("SocketException : {0}", se.ToString());
                }

                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }

            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
        }

        static void showIpAddress()  // https://stackoverflow.com/questions/6803073/get-local-ip-address
        {
            String strHostName = string.Empty;
            // Getting Ip address of local machine...
            // First get the host name of local machine.
            strHostName = Dns.GetHostName();
            Console.WriteLine("Local Machine's Host Name: " + strHostName);
            // Then using host name, get the IP address list..
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);

            IPAddress[] addr = ipEntry.AddressList;

            for (int i = 0; i < addr.Length; i++)
            {
                Console.WriteLine("IP Address {0}: {1} ", i, addr[i].ToString());
            }
            //return ;
            //Console.ReadLine();
        }

        static void runAsServer()
        {

            // Establish the local endpoint 
            // for the socket. Dns.GetHostName
            // returns the name of the host 
            // running the application.
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());

            //Select IP to run by ORDER
            Console.WriteLine("Enter the IP you want to use for SERVER Role:");
            int addOrder = Int16.Parse(s: Console.ReadLine());
            IPAddress ipAddr = ipHost.AddressList[addOrder];
            Console.WriteLine("You are using IP:" + ipAddr.ToString());


            //Select PORT to run, default PORT:8888
            Console.WriteLine("Enter the PORT you want to use (1024-65535):");
            int portToRun = Convert.ToInt32(Console.ReadLine());
            if (portToRun > 1023 & portToRun < 65535)
            {
                //Keep the port USER entered.
            }
            else
            {
                portToRun = 8888;
            }
            {
                Console.WriteLine("You selected PORT:" + portToRun + "\n");
            }
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, portToRun);

            // Creation TCP/IP Socket using 
            // Socket Class Constructor
            Socket listener = new Socket(ipAddr.AddressFamily,SocketType.Stream, ProtocolType.Tcp);

            try
            {

                // Using Bind() method we associate a
                // network address to the Server Socket
                // All client that will connect to this 
                // Server Socket must know this network
                // Address
                listener.Bind(localEndPoint);

                // Using Listen() method we create 
                // the Client list that will want
                // to connect to Server
                listener.Listen(10);

                Socket clientSocket = listener.Accept();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Waiting connection ... ");
                while (true)
                {
                    
                    // Suspend while waiting for
                    // incoming connection Using 
                    // Accept() method the server 
                    // will accept connection of client


                    // Data buffer
                    byte[] bytes = new Byte[1024];
                    string data = null;

                    while (true)
                    {

                        int numByte = clientSocket.Receive(bytes);

                        data += Encoding.ASCII.GetString(bytes,0, numByte);

                        if (data.IndexOf("<EOF>") > -1)
                            break;
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Text received -> {0} ", data);
                    //byte[] message = Encoding.ASCII.GetBytes("Test Server");
                    byte[] message = Encoding.ASCII.GetBytes(s: Console.ReadLine());

                    // Send a message to Client 
                    // using Send() method
                    clientSocket.Send(message);

                    // Close client Socket using the
                    // Close() method. After closing,
                    // we can use the closed Socket 
                    // for a new Client Connection

                }
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
    }
}
