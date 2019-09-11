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
    // [Authorize]
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
                // .Where(o => o.UserId == GetUserId())
                .Include(o => o.DeliveryLocation)
                .Include(o => o.Drugs).ThenInclude(p => p.Special)
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
                // .Where(o => o.UserId == GetUserId())
                .Include(o => o.DeliveryLocation)
                .Include(o => o.Drugs).ThenInclude(p => p.Special)
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
            order.DeliveryLocation = new LatLong(51.5001, -0.1239);
            // order.UserId = GetUserId();

            // Enforce existence of Drug.SpecialId
            // in the database - prevent the submitter from making up
            // new specials
            foreach (var drug in order.Drugs)
            {
                drug.SpecialId = drug.Special.Id;
                drug.Special = null;
            }
            
            _db.Orders.Attach(order);
            await _db.SaveChangesAsync();
            return order.OrderId;
        }

        private string GetUserId()
        {
            // This will be the user's twitter username
            return HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        }
    }
}
