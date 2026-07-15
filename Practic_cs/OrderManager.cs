using System;
using System.IO;
using System.Globalization;

namespace Practic_cs
{
    public class MenuItem
    {
        public string Description { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderManager
    {
        private MenuItem[] items = new MenuItem[5];
        private int itemCount = 0;
        private decimal tipAmount = 0;
        private const decimal GstRate = 0.05m;

        public string AddItem(string description, string priceInput)
        {
            if (itemCount >= 5) return "Error: Bill already has 5 items (maximum reached).";
            if (string.IsNullOrWhiteSpace(description)) return "Error: Description cannot be empty.";

            description = description.Trim();
            if (description.Length < 3 || description.Length > 20) return "Error: Description must be between 3 and 20 characters.";

            priceInput = priceInput.Replace(',', '.');
            if (!decimal.TryParse(priceInput, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                return "Error: Price must be a valid number.";

            if (price <= 0) return "Error: Price must be greater than zero.";

            items[itemCount] = new MenuItem { Description = description, Price = price };
            itemCount++;
            return "Add item was successful.";
        }

        public string RemoveItem(string indexInput)
        {
            if (!int.TryParse(indexInput, out int index)) return "Error: Please enter a valid item number.";
            if (index == 0) return "Cancelled.";
            if (index < 1 || index > itemCount) return "Error: Item number not found in the bill.";

            for (int i = index - 1; i < itemCount - 1; i++) items[i] = items[i + 1];
            items[itemCount - 1] = null;
            itemCount--;
            return "Remove item was successful.";
        }

        public MenuItem[] GetItems()
        {
            MenuItem[] currentItems = new MenuItem[itemCount];
            Array.Copy(items, currentItems, itemCount);
            return currentItems;
        }

        public string SetTipPercentage(string pctInput)
        {
            pctInput = pctInput.Replace(',', '.');
            if (!decimal.TryParse(pctInput, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal pct) || pct < 0)
                return "Error: Invalid percentage.";

            tipAmount = Math.Round(GetNetTotal() * (pct / 100m), 2);
            return "Tip applied.";
        }

        public string SetTipAmount(string amtInput)
        {
            amtInput = amtInput.Replace(',', '.');
            if (!decimal.TryParse(amtInput, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amt) || amt < 0)
                return "Error: Invalid amount.";

            tipAmount = Math.Round(amt, 2);
            return "Tip applied.";
        }

        public void ClearTip() => tipAmount = 0;

        public decimal GetNetTotal()
        {
            decimal total = 0;
            for (int i = 0; i < itemCount; i++) total += items[i].Price;
            return total;
        }

        public decimal GetGst() => Math.Round(GetNetTotal() * GstRate, 2);
        public decimal GetTip() => tipAmount;
        public decimal GetTotal() => GetNetTotal() + GetGst() + GetTip();

        public void ClearAll()
        {
            Array.Clear(items, 0, items.Length);
            itemCount = 0;
            tipAmount = 0;
        }

        public string SaveToFile(string filepath)
        {
            filepath = filepath.Trim();
            if (string.IsNullOrWhiteSpace(filepath)) return "Error: Filename cannot be empty.";
            try
            {
                using (StreamWriter sw = new StreamWriter(filepath))
                {
                    for (int i = 0; i < itemCount; i++)
                        sw.WriteLine($"{items[i].Description};{items[i].Price.ToString(CultureInfo.InvariantCulture)}");
                }
                return $"Write to file {filepath} was successful.";
            }
            catch (Exception ex) { return $"System Error (Save): {ex.Message}"; }
        }

        public string LoadFromFile(string filepath)
        {
            filepath = filepath.Trim();
            if (!File.Exists(filepath)) return $"Error: File {filepath} not found.";
            try
            {
                string[] lines = File.ReadAllLines(filepath);
                ClearAll();
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    string[] parts = line.Split(';');
                    if (parts.Length == 2) AddItem(parts[0], parts[1]);
                }
                return $"Read from {filepath} was successful.";
            }
            catch (Exception ex) { return $"System Error (Load): {ex.Message}"; }
        }
    }
}