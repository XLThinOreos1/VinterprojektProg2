using System.Diagnostics;

public class Commands
{
    public static List<string> GlobalCommands = new() { "/clear" };

    public static void RunCommand(string command)
    {
        if (command == "/clear")
        {
            thecode.messagehistory.Clear();
        }
        else if (command == "/hacker")
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }
        else if (command == "/help")
        {
            Console.SetCursorPosition(0, 16);
            Console.WriteLine("/clear, /hacker, /help");
        }
    }

}