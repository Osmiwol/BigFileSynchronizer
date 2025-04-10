using BigFileSynchronizer.Commands;
class Programm
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("BigFileSynchronizer by osmiwol: no command provided.");
            Console.WriteLine("Try: init, push, scan, help");
            return;
        }

        switch (args[0].ToLower())
        {
            case "init":
                InitCommand.Execute();
                break;
            case "push":
                PushCommand.Execute();
                break;
            case "scan":
                ScanCommand.Execute();
                break;
            case "help":
                HelpCommand.Execute();
                break;
            default:
                Console.WriteLine($"Unknown command: {args[0]}");
                HelpCommand.Execute();
                break;
        }
    }
}
