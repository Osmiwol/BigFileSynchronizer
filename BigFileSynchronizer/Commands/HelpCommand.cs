using System;

namespace BigFileSynchronizer.Commands
{
    public static class HelpCommand
    {
        public static void Execute()
        {
            Console.WriteLine("BigFileSynchronizer CLI — автоматическая загрузка крупных ассетов в облако при git push.");
            Console.WriteLine();
            Console.WriteLine("Доступные команды:");
            Console.WriteLine("  init     — создать config.json, .state/, и git pre-push hook");
            Console.WriteLine("  push     — заархивировать и загрузить новые или изменённые ассеты");
            Console.WriteLine("  scan     — просканировать проект на крупные ассеты и предложить их добавить в config");
            Console.WriteLine("  help     — показать это сообщение");
            Console.WriteLine();
            Console.WriteLine("Пример:");
            Console.WriteLine("  BigFileSynchronizer.exe push");
        }
    }
}
