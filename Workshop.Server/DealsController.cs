using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Workshop.Server
{
    [Route("deals")]
    [ApiController]
    public class DealsController : Controller
    {
        private readonly DrugStoreContext _db;

        public DealsController(DrugStoreContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<DrugDeal>>> GetDeals()
        {
            return (await _db.Deals.ToListAsync()).OrderByDescending(sp => sp.BasePrice).ToList();
        }
    }
}
