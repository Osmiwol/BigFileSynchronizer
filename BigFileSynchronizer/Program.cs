using BigFileSynchronizer.Commands;
class Programm
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("BigFileSynchronizer is a console tool. Please run it with command-line arguments.");
            Console.WriteLine();
            HelpCommand.Execute();
            Console.WriteLine("press any key to exit...");
            Console.ReadKey();
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
            case "pull":
                PullCommand.Execute();
                break;
            case "reset":
                ResetCommand.Execute();
                break;
            case "auth":
                AuthCommand.Execute(args);
                break;

            default:
                Console.WriteLine($"Unknown command: {args[0]}");
                HelpCommand.Execute();
                break;
        }
    }
}
