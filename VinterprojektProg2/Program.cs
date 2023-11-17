﻿using System.Net;
using System.Net.Sockets;

TcpListener listener;
TcpClient client;
NetworkStream nwStream;

Console.WriteLine("Host(0) or Client(1)?");

if (Console.ReadLine() == "0")
{
    Host();
}
else
{
    Connect();
}
//starts hosting and then wait for a client
void Host()
{
    IPAddress localAdd = IPAddress.Parse("127.0.0.1");
    //start Listening for client
    listener = new TcpListener(IPAddress.Any, 5000);
    Console.WriteLine("waiting for client");
    listener.Start();
    client = listener.AcceptTcpClient();
    nwStream = client.GetStream();
}

//connects to given ip
void Connect()
{
    Console.WriteLine("Enter server IP");
    string SERVER_IP = Console.ReadLine();
    client = new TcpClient(SERVER_IP, 5000);
    nwStream = client.GetStream();
}

while (true)
{
    Send();
    Receive();
}

void Receive()
{
    byte[] buffer = new byte[client.ReceiveBufferSize];

    int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

    //convert the bytes received into a string
    string dataReceived = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);

    Console.WriteLine(dataReceived);
}

void Send()
{
    //Converts the string into bytes so that it can be sent
    byte[] bytesToSend = System.Text.ASCIIEncoding.ASCII.GetBytes("hello");
    nwStream.Write(bytesToSend, 0, bytesToSend.Length);
}


Console.ReadLine();