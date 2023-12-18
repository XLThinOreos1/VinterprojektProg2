public class Commands
{
    public static List<string> GlobalCommands = new() { "/clear" };

    public static void RunCommand(string command)
    {
        if (command == "/clear")
        {
            NetworkCode.messagehistory.Clear();
        }
        else if (command == "/hacker")
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }
        else if (command == "/default")
        {
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }

}