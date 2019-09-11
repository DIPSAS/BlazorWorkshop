using System;
using System.Collections.Generic;
using System.Linq;

namespace Workshop
{
    public class Order
    {
        public int OrderId { get; set; }

        public string UserId { get; set; }

        public DateTime CreatedTime { get; set; }

        public Address DeliveryAddress { get; set; } = new Address();

        public LatLong DeliveryLocation { get; set; }

        public List<Drug> Drugs { get; set; } = new List<Drug>();

    }
}
