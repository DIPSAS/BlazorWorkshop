using System.Collections.Generic;
using System.Linq;

namespace Workshop
{
    /// <summary>
    /// Represents a customized drug as part of an order
    /// </summary>
    public class Drug
    {
        public const int DefaultQuantity = 1;
        public const int MinimumQuantity = 1;
        public const int MaximumQuantity = 10;

        public int Id { get; set; }

        public int Quantity { get; set; }
        public int OrderId { get; set; }

        public DrugDeal Deal { get; set; }

        public int DealId { get; set; }

        public decimal GetTotalPrice()
        {
            return Deal.BasePrice * Quantity; 
        }

        public string GetFormattedTotalPrice()
        {
            return GetTotalPrice().ToString("0.00");
        }

    }
}
