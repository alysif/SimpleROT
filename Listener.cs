using System.Net.Sockets;
using System;
using System.Net;
using System.IO;
using System.Text;

class Listener
{
    public static byte[] ResizeBytes(byte[] array)
    {
        // Rezise byte array
        int size = array.Length - 1;
        while (array[size] == 0) --size;
        byte[] resize = new byte[size+1];
        Array.Copy(array, resize, resize.Length);
        return resize;
    }
    public static void Main(string[] args)
    {
        Socket client = null;
        Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(IPAddress.Any, 9999));
        socket.Listen(10);
        bool listen = true;
        Console.WriteLine("Server listen");
        while(listen)
        {
            client = socket.Accept();
            if(client != null) listen = false;
        }
        Console.WriteLine("Client connected");
        while(true)
        {
            // Get command from user
            Console.Write("$ -> ");
            string command = Console.ReadLine();
            // Send command to client
            byte[] command_ = Encoding.ASCII.GetBytes(command);
            //Console.WriteLine(command_.Length);
            client.Send(command_, 0, command_.Length, SocketFlags.None);
            if(command.StartsWith("exit"))
            {
                break;
            }
            // Buffer for recv
            byte[] buffer = new byte[30000];
            // Recv message
            client.Receive(buffer, 0, buffer.Length, SocketFlags.None);
            // Bytearray resize
            byte[] resize = Listener.ResizeBytes(buffer);
            // Write message in console
            Console.WriteLine(Encoding.ASCII.GetString(resize));
        }
        Environment.Exit(0);
    }
}