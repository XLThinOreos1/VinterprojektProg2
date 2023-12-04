using System.Net;
using System.Net.Sockets;

public class thecode
{
    TcpListener listener;
    TcpClient client;
    NetworkStream nwStream;

    // Message har en public user 
    public User userexample;

    Queue<string> messagehistory = new();

    //starts hosting and then wait for a client
    public void Host()
    {
        IPAddress localAdd = IPAddress.Parse("127.0.0.1");
        //start Listening for client
        listener = new TcpListener(IPAddress.Any, 5000);
        Console.WriteLine("waiting for client");
        listener.Start();
        client = listener.AcceptTcpClient();
        nwStream = client.GetStream();

        GUI.Menu();
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
    }

    private void Send(string UserMessage)
    {
        if (UserMessage[0] == '/')
        {
            Commands.RunCommand(UserMessage, true);
        }
        // checka ifall första tecknet är /
        //Converts the string into bytes so that it can be sent
        byte[] bytesToSend = System.Text.Encoding.UTF8.GetBytes($"{userexample.username}: {UserMessage}");
        nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        GUI.Menu();
        QueueHandler();
        messagehistory.Enqueue($"{userexample.username}: {UserMessage}");
        Console.Write("\b");
        Console.SetCursorPosition(0, 3);
        MessageHistoryWriter();
    }

    private void MessageHistoryWriter()
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
        GUI.Menu();
        QueueHandler();
        messagehistory.Enqueue(dataReceived);
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

        Thread ReceiveT = new Thread(() =>
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

    public void QueueHandler()
    {
        if (messagehistory.Count > 9)
        {
            messagehistory.Dequeue();
        }
    }
}
