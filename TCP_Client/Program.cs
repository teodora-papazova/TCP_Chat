using System;
using System.Net.Sockets;
using System.Text;
using System.Threading; //добавяме библиотеки

class Client //клас
{
    private const int Port = 8888; //константа от числен тип за порт
    private const string ServerIp = "127.0.0.1";//контанта от низов тип за IP на сървъра

    static void Main() //главен клас
    {
        TcpClient client = new TcpClient(ServerIp, Port);//създаване на обект 
        Console.WriteLine("Connected to server. Start chatting!");//изписва на екрана това в кавичките

        NetworkStream stream = client.GetStream();

        Thread receiveThread = new Thread(ReceiveMessages);//създаване на обект
        receiveThread.Start(stream);

        while (true)//цикъл който чете съобщение и го записва в масив като байтове
        {
            string message = Console.ReadLine();
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
        }
    }

    static void ReceiveMessages(object obj)//обект за получаване на съобщения
    {
        NetworkStream stream = (NetworkStream)obj;
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
                Console.WriteLine(message);//изкарва съобщението на екрана
            }
            catch (Exception)//ако се получи esception цикъла спира
            {
                break;
            }
        }
    }
}
