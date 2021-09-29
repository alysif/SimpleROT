using System.Net.Sockets;
using System;
using System.Net;
using System.IO;
using System.Text;
using System.Windows.Forms;
class AtlatService
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

    public static string GetTextFromCommand(string data)
    {
        string[] splitData = data.Split(' ');
        string text = "";
        for(int i=1;i<splitData.Length;i++)
        {
            if(i<splitData.Length)
            {
                text += splitData[i]+" ";
            }
            else
            {
                text += splitData[i];
            }
        }
        return text;
    }
    public static void SendMessage(string message, Socket client)
    {
        // Response to server
        byte[] response = Encoding.ASCII.GetBytes(message);
        // Send command
        client.Send(response, 0, response.Length, SocketFlags.None);
    }

    public static void SendFile(string filename, Socket client)
    {
        // Response to server
        byte[] response = Encoding.ASCII.GetBytes("Command recv");
        // Send command
        client.Send(response, 0, response.Length, SocketFlags.None);
    }

    public static void Main(string[] args)
    {
        Socket client = new Socket(SocketType.Stream, ProtocolType.Tcp);
        client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999));
        while(true)
        {
            // Create buffer to recv message
            byte[] buffer = new byte[30000];
            // Recv message
            client.Receive(buffer, 0, buffer.Length, SocketFlags.None);
            byte[] resize = AtlatService.ResizeBytes(buffer);
            // Show message
            string data = Encoding.ASCII.GetString(resize);
            if(data.StartsWith("exit"))
            {
                client.Close();
                break;
            }
            else if(data.StartsWith("msg"))
            {
                string message = AtlatService.GetTextFromCommand(data);  
                MessageBox.Show(message);
                AtlatService.SendMessage("Show message in PC", client);
            }
            else if(data.StartsWith("cd"))
            {
                string moveTo = AtlatService.GetTextFromCommand(data);
                Directory.SetCurrentDirectory(moveTo);
                AtlatService.SendMessage(string.Format("Directory changed to: {0}", Directory.GetCurrentDirectory()), client);
            }
            else
            {
                AtlatService.SendMessage("Command recv", client);
                Console.WriteLine("Command recv");
            }
        }
        Environment.Exit(0);
    }
}