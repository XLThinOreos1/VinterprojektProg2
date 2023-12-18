public class Commands
{
    // Skapar en lista för globala kommandon som kommer köras på båda skärmar, just nu är det bara /clear som rensar chat history
    public static List<string> GlobalCommands = new() { "/clear" };

    public static void RunCommand(string command)
    {
        // Rensar messagehistory queue ifall man använder /clear kommandet
        if (command == "/clear")
        {
            NetworkCode.messagehistory.Clear();
        }
        // Aktiverar "hacker mode" som gör bara texten grön
        else if (command == "/hacker")
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }
        // Sätter textfärgen till default grå
        else if (command == "/default")
        {
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}