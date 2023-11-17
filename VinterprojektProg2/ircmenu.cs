class GUI
{
    void AskUser()
    {
        Console.Clear();
        Console.Write("Enter name: ");
        User u = new(Console.ReadLine());
    }

    void Menu()
    {
        Console.WriteLine(@"               IRC Channel");
        Console.WriteLine("---------------------------------------------");
    }
}