using System.Collections.Generic;
using System.Linq;

namespace Workshop
{
    /// <summary>
    /// Represents a customized drug as part of an order
    /// </summary>
    public class Drug
    {
        public const int DefaultSize = 12;
        public const int MinimumSize = 9;
        public const int MaximumSize = 17;

        public int Id { get; set; }

        public int OrderId { get; set; }

        public DrugSpecial Special { get; set; }

        public int SpecialId { get; set; }

    }
}
