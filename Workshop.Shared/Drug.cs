using System.Collections.Generic;
using System.Linq;

namespace Workshop
{
    /// <summary>
    /// Represents a customized drug as part of an order
    /// </summary>
    public class Drug
    {
        public const int DefaultSize = 1;
        public const int MinimumSize = 1;
        public const int MaximumSize = 10;

        public int Id { get; set; }

        public int Size { get; set; }
        public int OrderId { get; set; }

        public DrugSpecial Special { get; set; }

        public int SpecialId { get; set; }

        public decimal GetTotalPrice()
        {
            return Special.BasePrice * Size; 
        }

        public string GetFormattedTotalPrice()
        {
            return GetTotalPrice().ToString("0.00");
        }

    }
}
