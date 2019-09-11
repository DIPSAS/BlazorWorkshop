using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Workshop.Server
{
    [Route("specials")]
    [ApiController]
    public class SpecialsController : Controller
    {
        private readonly DrugStoreContext _db;

        public SpecialsController(DrugStoreContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<DrugSpecial>>> GetSpecials()
        {
            return (await _db.Specials.ToListAsync()).OrderByDescending(sp => sp.BasePrice).ToList();
        }
    }
}
