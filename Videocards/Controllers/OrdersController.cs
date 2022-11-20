using Microsoft.AspNetCore.Mvc;
using Videocards.Models;

namespace Videocards.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
