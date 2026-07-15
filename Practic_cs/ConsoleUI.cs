using System;
using System.IO;

namespace Practic_cs
{
    public class ConsoleUI
    {
        private readonly OrderManager _manager;
        private bool _firstMenuShown = false;

        public ConsoleUI(OrderManager manager)
        {
            _manager = manager;
        }

        public void Run()
        {
            bool isRunning = true;

            while (isRunning)
            {
                ShowMenu();

                string choice = "";
                bool validChoice = false;

                while (!validChoice)
                {
                    char key = Console.ReadKey(true).KeyChar;
                    choice = key.ToString();

                    if (choice == "0" || choice == "1" || choice == "2" || choice == "3" ||
                        choice == "4" || choice == "5" || choice == "6" || choice == "7")
                    {
                        Console.WriteLine(choice);
                        validChoice = true;
                    }
                }

                Console.WriteLine();

                switch (choice)
                {
                    case "1": AddItemUI(); break;
                    case "2": RemoveItemUI(); break;
                    case "3": AddTipUI(); break;
                    case "4": DisplayBillUI(); break;
                    case "5":
                        _manager.ClearAll();
                        Console.WriteLine("All items have been cleared.");
                        break;

                    case "6":
                        string savePath = "";
                        while (true)
                        {
                            Console.Write("Enter the file path to save items to: ");
                            savePath = Console.ReadLine();

                            if (!string.IsNullOrWhiteSpace(savePath))
                            {
                                break;
                            }
                            Console.WriteLine("Error: File path cannot be empty. Please try again.");
                        }
                        Console.WriteLine(_manager.SaveToFile(savePath));
                        break;

                    case "7":
                        string loadPath = "";
                        while (true)
                        {
                            Console.Write("Enter the file path to load items from: ");
                            loadPath = Console.ReadLine();

                            if (File.Exists(loadPath))
                            {
                                break;
                            }

                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Error: File '{loadPath}' does not exist. Please enter a valid path.");
                            Console.ResetColor();
                        }
                        Console.WriteLine(_manager.LoadFromFile(loadPath));
                        break;

                    case "0":
                        Console.WriteLine("Good-bye and thanks for using this program.");
                        isRunning = false;
                        break;
                }

                if (isRunning)
                {
                    Console.WriteLine();
                }
            }
        }

        private void ShowMenu()
        {
            if (!_firstMenuShown)
            {
                Console.WriteLine(" ___________________________");
                Console.WriteLine("|  _______________________  |");
                Console.WriteLine("| |                       | |");
                Console.WriteLine("| | Kitten's Cafe         | |");
                Console.WriteLine("| | --------------------- | |");
                Console.WriteLine("| | 1. Add Item           | |");
                Console.WriteLine("| | 2. Remove Item        | |");
                Console.WriteLine("| | 3. Add Tip            | |");
                Console.WriteLine("| | 4. Display Bill       | |");
                Console.WriteLine("| | 5. Clear All          | |");
                Console.WriteLine("| | 6. Save to file       | |");
                Console.WriteLine("| | 7. Load from file     | |");
                Console.WriteLine("| | 0. Exit               | |");
                Console.WriteLine("| |_______________________| |");
                Console.WriteLine("|___________________________|");
                Console.Write("Enter your choice: ");
                _firstMenuShown = true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("// Menu omitted to save screen space");
                Console.ResetColor();
                Console.Write("Enter your choice: ");
            }
        }

        private void AddItemUI()
        {
            Console.Write("Enter description: ");
            string desc = Console.ReadLine();
            Console.Write("Enter price: ");
            string price = Console.ReadLine();
            Console.WriteLine(_manager.AddItem(desc, price));
        }

        private void RemoveItemUI()
        {
            var itemsToRemove = _manager.GetItems();
            if (itemsToRemove.Length == 0)
            {
                Console.WriteLine("There are no items in the bill.");
                return;
            }

            Console.WriteLine("ItemNo Description       Price");
            for (int i = 0; i < itemsToRemove.Length; i++)
            {
                Console.WriteLine($"{i + 1}      {itemsToRemove[i].Description,-16} ${itemsToRemove[i].Price:F2}");
            }

            Console.Write("\nEnter the item number to remove or 0 to cancel: ");
            Console.WriteLine(_manager.RemoveItem(Console.ReadLine()));
        }

        private void AddTipUI()
        {
            if (_manager.GetItems().Length == 0)
            {
                Console.WriteLine("There are no items in the bill to add tip for.");
                return;
            }

            Console.WriteLine($"Net Total: ${_manager.GetNetTotal():F2}");
            Console.WriteLine("1 - Tip Percentage");
            Console.WriteLine("2 - Tip Amount");
            Console.WriteLine("3 - No Tip");

            string tipMethod = "";
            while (true)
            {
                Console.Write("Enter Tip Method (1-3): ");
                tipMethod = Console.ReadLine();

                if (tipMethod == "1" || tipMethod == "2" || tipMethod == "3")
                {
                    break;
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Invalid choice. Please enter 1, 2, or 3.");
                Console.ResetColor();
            }

            if (tipMethod == "1")
            {
                while (true)
                {
                    Console.Write("Enter tip percentage: ");
                    string result = _manager.SetTipPercentage(Console.ReadLine());

                    if (result.StartsWith("Error"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(result);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine(result);
                        break;
                    }
                }
            }
            else if (tipMethod == "2")
            {
                while (true)
                {
                    Console.Write("Enter Tip amount: ");
                    string result = _manager.SetTipAmount(Console.ReadLine());

                    if (result.StartsWith("Error"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(result);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine(result);
                        break;
                    }
                }
            }
            else if (tipMethod == "3")
            {
                _manager.ClearTip();
                Console.WriteLine("Tip cleared successfully.");
            }
        }

        private void DisplayBillUI()
        {
            var billItems = _manager.GetItems();
            if (billItems.Length == 0)
            {
                Console.WriteLine("There are no items in the bill to display.");
                return;
            }

            Console.WriteLine("Description             Price");
            Console.WriteLine("----------------  -----------");
            foreach (var item in billItems)
            {
                string itemPrice = "$" + item.Price.ToString("F2");
                Console.WriteLine($"{item.Description,-16}  {itemPrice,11}");
            }

            Console.WriteLine("----------------  -----------");
            Console.WriteLine($"{"Net Total",16}  {"$" + _manager.GetNetTotal().ToString("F2"),11}");
            Console.WriteLine($"{"Tip Amount",16}  {"$" + _manager.GetTip().ToString("F2"),11}");
            Console.WriteLine($"{"Total GST",16}  {"$" + _manager.GetGst().ToString("F2"),11}");
            Console.WriteLine($"{"Total Amount",16}  {"$" + _manager.GetTotal().ToString("F2"),11}");
        }
    }
}