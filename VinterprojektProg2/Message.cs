using System.Net;
using System.Net.Sockets;

public class NetworkCode
{
    TcpListener listener;
    TcpClient client;
    NetworkStream nwStream;

    // Message har en public user 
    public User userexample;
    private string OtherUsername;

    // Gör messagehistory till en queue
    public static Queue<string> messagehistory = new();

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
        // Gör om string till bytes så man kan skicka meddelandet till andra datorn
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
        // Buffer är en array variabel som ska senare innehålla något
        // Skapar en buffer variabel som är lika stor som mängden data som tas in
        // Buffer behövs skapas eftersom nwStream.Read() måste lagra data nånstans.
        byte[] buffer = new byte[client.ReceiveBufferSize];

        // Lagrar datan i buffer och sen mängden bytes som datan är.
        // Läser av hela packet som den får in
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

        // Gör om bytes som den får in till en string
        string dataReceived = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);

        // Console.WriteLine(dataReceived[0]);

        // Den checkar ifall första symbolen/bokstaven av meddelandet innehåller tecknet '/'
        // Om den gör det så kommer den förstå att det är ett chat kommando och senare kör kommandet
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

    // Funktion som kör igång två threads så båda personer kan skriva samtidigt och hindrar att det blir turn-based
    public void MessageThread()
    {
        Thread SendT = new(() =>
        {
            // Send väntar på användarens input och senare rensar raden där man skrev in något så det gamla
            // meddelandet inte är kvar när man vill skriva något nytt
            while (true)
            {
                Send(Console.ReadLine());
                Console.SetCursorPosition(2, 14);
                Console.Write("                                                                        ");
                Console.SetCursorPosition(2, 14);
            }
        });

        // Kör receive och senare placerar cursor i skriv rutan
        Thread ReceiveT = new(() =>
        {
            while (true)
            {
                Receive();
                Console.SetCursorPosition(2, 14);
            }
        });

        // Startar threads
        SendT.Start();

        ReceiveT.Start();
    }

    // Hanterar chat historikens queue, om meddelanden är över 9 så tar den bort ett meddelande så den kan få plats
    // med en ny meddelande på skärmen
    public static void QueueHandler()
    {
        if (messagehistory.Count > 9)
        {
            messagehistory.Dequeue();
        }
    }
}
