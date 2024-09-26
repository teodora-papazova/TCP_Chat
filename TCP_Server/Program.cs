using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;//добавяме библиотеки

class Server//клас
{
    private static readonly List<TcpClient> clients = new List<TcpClient>();//списък
    private const int Port = 8888;//константа от числен тип

    static void Main()//главен клас
    {
        TcpListener server = new TcpListener(IPAddress.Any, Port);//обект
        server.Start();
        Console.WriteLine($"Server started on port {Port}");//изписва на екрана съобщение

        while (true)//сървъра получава клиенти и се записват в списъка
        {
            TcpClient client = server.AcceptTcpClient();
            clients.Add(client);
            Thread clientThread = new Thread(HandleClient);
            clientThread.Start(client);
        }
    }

    static void HandleClient(object obj)
    {
        TcpClient tcpClient = (TcpClient)obj;
        NetworkStream stream = tcpClient.GetStream();

        byte[] buffer = new byte[1024];
        int bytesRead;

        while (true)
        {
            try
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);//чете съобщението като гледа колко числа има
                if (bytesRead == 0)//когато числата свършат цикъла спира
                {
                    break;
                }

                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);//запазва съобщението
                Console.WriteLine($"Received: {message}");//изкарва съобщението на екрана

                BroadcastMessage(tcpClient, message);
            }
            catch (Exception)//ако се получи esception цикъла спира
            {
                break;
            }
        }

        clients.Remove(tcpClient);
        tcpClient.Close();//затваря програмата
    }

    static void BroadcastMessage(TcpClient sender, string message)
    {
        byte[] broadcastBuffer = Encoding.ASCII.GetBytes(message);

        foreach (TcpClient client in clients)
        {
            if (client != sender)
            {
                NetworkStream stream = client.GetStream();
                stream.Write(broadcastBuffer, 0, broadcastBuffer.Length);
            }
        }
    }
}
