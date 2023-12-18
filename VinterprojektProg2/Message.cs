using System.Net;
using System.Net.Sockets;

public class NetworkCode
{
    TcpListener listener;
    TcpClient client;
    NetworkStream nwStream;

    // Message har en public user 
    public User userexample;
    string OtherUsername;

    // Gör messagehistory till en queue
    public static readonly Queue<string> messagehistory = new();

    //börjar hosta och väntar sen på en client
    public void Host()
    {
        // listener börjar lyssna på data som den får in på port 5000
        // IPAddress.Any gör att den lyssnar på all inkommande trafik.
        listener = new TcpListener(IPAddress.Any, 5000);
        Console.WriteLine("waiting for client");
        listener.Start();
        // Lyssnar på en client och sparar den när den ser att någon försöker koppla sig till en port.
        client = listener.AcceptTcpClient();
        // Client stream är all inkommande data som den får in från client och senare sparar den för framtida användning.
        nwStream = client.GetStream();

        GUI.Menu();

        // Buffer är en array variabel som ska senare innehålla något
        // Skapar en buffer variabel som är lika stor som mängden data som tas in
        // Buffer behövs skapas eftersom nwStream.Read() måste lagra data nånstans.
        byte[] buffer = new byte[client.ReceiveBufferSize];
        // lagrar datan i buffer, 0 offset och sen mängden bytes som datan är.
        // Läser av hela packet som den får in
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
        // Datan som den får in är i bytes så man gör om den till UTF8 karaktärer. (bytes to convert, index to start at, number of bytes)
        OtherUsername = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);

        Send(userexample.username);
    }

    // connects to the given ip
    public void Connect()
    {
        Console.WriteLine("Enter server IP");
        string SERVER_IP = Console.ReadLine();
        // Behövs inte men gör converten från localhost till 127.0.0.1 snabbare än hur datorn själv räknar ut det
        if (SERVER_IP == "localhost")
        {
            SERVER_IP = "127.0.0.1";
        }
        client = new TcpClient(SERVER_IP, 5000);
        nwStream = client.GetStream();

        GUI.Menu();

        Send(userexample.username);

        byte[] buffer = new byte[client.ReceiveBufferSize];
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
        OtherUsername = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
    }

    private void Send(string UserMessage)
    {
        // checka ifall första tecknet är /
        if (UserMessage[0] == '/')
        {
            Commands.RunCommand(UserMessage);
        }
        //Converts the string into bytes so that it can be sent
        byte[] bytesToSend = System.Text.Encoding.UTF8.GetBytes(UserMessage);
        nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        GUI.Menu();
        QueueHandler();
        messagehistory.Enqueue($"{userexample.username}: {UserMessage}");
        Console.Write("\b");
        Console.SetCursorPosition(0, 3);
        MessageHistoryWriter();
    }

    private static void MessageHistoryWriter()
    {
        foreach (string UserMessage in messagehistory)
        {
            Console.WriteLine($"{UserMessage}");
        }
    }

    private void Receive()
    {
        byte[] buffer = new byte[client.ReceiveBufferSize];

        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

        //convert the bytes received into a string
        string dataReceived = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine(dataReceived[0]);

        if (dataReceived[0] == '/')
        {
            if (Commands.GlobalCommands.Contains(dataReceived))
            {
                Commands.RunCommand(dataReceived);
            }
        }

        GUI.Menu();
        QueueHandler();
        messagehistory.Enqueue($"{OtherUsername}: {dataReceived}");
        Console.Write("\b");
        Console.SetCursorPosition(0, 3);
        MessageHistoryWriter();
    }

    public void MessageThread()
    {
        Thread SendT = new Thread(() =>
        {
            while (true)
            {
                Send(Console.ReadLine());
                Console.SetCursorPosition(2, 14);
                Console.Write("                                                                        ");
                Console.SetCursorPosition(2, 14);
            }
        });

        Thread ReceiveT = new(() =>
        {
            while (true)
            {
                Receive();
                Console.SetCursorPosition(2, 14);
            }
        });

        SendT.IsBackground = true;
        SendT.Start();

        ReceiveT.IsBackground = true;
        ReceiveT.Start();
    }

    public static void QueueHandler()
    {
        if (messagehistory.Count > 9)
        {
            messagehistory.Dequeue();
        }
    }
}
