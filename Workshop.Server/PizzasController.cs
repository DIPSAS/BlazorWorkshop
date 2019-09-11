using Microsoft.AspNetCore.Mvc;

namespace Workshop.Server
{
    [Route("drugs")]
    [ApiController]
    public class DrugController : Controller
    {
        private readonly DrugStoreContext _db;

        public DrugController(DrugStoreContext db)
        {
            _db = db;
        }
    }
}
