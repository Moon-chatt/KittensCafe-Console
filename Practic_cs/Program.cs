using System;

namespace Practic_cs
{
    class Program
    {
        static void Main()
        {
            OrderManager manager = new OrderManager();
            ConsoleUI ui = new ConsoleUI(manager);
            ui.Run();
        }
    }
}