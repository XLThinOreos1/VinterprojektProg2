thecode handler = new();
Console.Write("Enter name: ");
User u = new(Console.ReadLine());
handler.userexample = u;

Console.WriteLine("Host(0) or Client(1)?");

if (Console.ReadLine() == "0")
{
    handler.Host();
}
else
{
    handler.Connect();
}
handler.MessageThread();

bool ShouldWindowClose = false;

while (!ShouldWindowClose) { }