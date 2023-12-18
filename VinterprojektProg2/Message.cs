using System.Net;
using System.Net.Sockets;

public class thecode
{
    TcpListener listener;
    TcpClient client;
    NetworkStream nwStream;

    // Message har en public user 
    public User userexample;
    string OtherUsername;

    public static Queue<string> messagehistory = new();

    //starts hosting and then wait for a client
    public void Host()
    {
        // this starts listening for data being received on port 5000
        // IPAddress.Any just means that its listening on all network interfaces, its basically just 0.0.0.0
        listener = new TcpListener(IPAddress.Any, 5000);
        Console.WriteLine("waiting for client");
        listener.Start();
        // listens for a client and saves it when it sees someone trying to connect to the computer on that port
        client = listener.AcceptTcpClient();
        // saves the client stream to use later. 
        // the client stream is just all the data that is being received from the client
        nwStream = client.GetStream();

        GUI.Menu();

        // creates a buffer the size of the amount of data that is being received
        // the buffer needs to be created because nwStream.Read() needs somewhere to store the data
        byte[] buffer = new byte[client.ReceiveBufferSize];
        // stores the data in buffer, 0 offset, amount of bytes that the data is. 
        // it reads the entire received packet
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
        // the received data is in bytes, convert it to utf8 characters. (bytes to convert, index to start at, number of bytes)
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
