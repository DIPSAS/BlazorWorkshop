using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Workshop.Server
{
    [Route("orders")]
    [ApiController]
    public class OrdersController : Controller
    {
        private readonly DrugStoreContext _db;

        public OrdersController(DrugStoreContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderWithStatus>>> GetOrders()
        {
            var orders = await _db.Orders
                .Include(o => o.DeliveryLocation)
                .Include(o => o.Drugs).ThenInclude(p => p.Deal)
                .Include(o => o.Drugs)
                .OrderByDescending(o => o.CreatedTime)
                .ToListAsync();

            return orders.Select(o => OrderWithStatus.FromOrder(o)).ToList();
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderWithStatus>> GetOrderWithStatus(int orderId)
        {
            var order = await _db.Orders
                .Where(o => o.OrderId == orderId)
                .Include(o => o.DeliveryLocation)
                .Include(o => o.Drugs).ThenInclude(p => p.Deal)
                .Include(o => o.Drugs)
                .SingleOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return OrderWithStatus.FromOrder(order);
        }

        [HttpPost]
        public async Task<ActionResult<int>> PlaceOrder(Order order)
        {
            order.CreatedTime = DateTime.Now;
            order.DeliveryLocation = new LatLong(69.6670603, 18.9702901);

            // Enforce existence of Drug.DealId
            // in the database - prevent the submitter from making up
            // new deals
            foreach (var drug in order.Drugs)
            {
                drug.DealId = drug.Deal.Id;
                drug.Deal = null;
            }

            _db.Orders.Attach(order);
            await _db.SaveChangesAsync();
            return order.OrderId;
        }
    }
}
