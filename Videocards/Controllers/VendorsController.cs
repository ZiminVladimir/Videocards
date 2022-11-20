using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Videocards.Models;

namespace Videocards.Controllers
{
    public class VendorsController : Controller
    {
        private VCContext _context;
        public VendorsController(VCContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id, Name")] Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vendor);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View("Create");
        }

        public IActionResult Create(int id)
        {

            return View();
        }
    }
}
