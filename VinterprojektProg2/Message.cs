using System.Net;
using System.Net.Sockets;

public class thecode
{
    TcpListener listener;
    TcpClient client;
    NetworkStream nwStream;

    // Message har en public user 
    public User userexample;

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
        //Converts the string into bytes so that it can be sent
        byte[] bytesToSend = System.Text.Encoding.ASCII.GetBytes($"{userexample.username}: {UserMessage}");
        nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        Console.Write("\b");
        Console.WriteLine($"{userexample.username}: {UserMessage}");
    }

    private void Receive()
    {
        byte[] buffer = new byte[client.ReceiveBufferSize];

        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

        //convert the bytes received into a string
        string dataReceived = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);

        Console.WriteLine(dataReceived);
    }

    public void MessageThread()
    {
        Thread SendT = new Thread(() =>
        {
            while (true)
            {
                Send(Console.ReadLine());
            }
        });

        Thread ReceiveT = new Thread(() =>
        {
            while (true)
            {
                Receive();
            }
        });

        SendT.IsBackground = true;
        SendT.Start();

        ReceiveT.IsBackground = true;
        ReceiveT.Start();
    }
}

