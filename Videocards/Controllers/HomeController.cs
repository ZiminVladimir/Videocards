using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Videocards.Models;

namespace Videocards.Controllers
{
    
    public class HomeController : Controller
    {
        private VCContext db;

        public HomeController(VCContext context)
        {
            db = context;
            //InitDb();
        }

        private void InitDb()
        {
            var vendr1 = new Vendor() { Name = "Gigabyte" };
            var vendr2 = new Vendor() { Name = "Powercolor" };

            var vc1 = new Videocard()
            {
                Name = "RTX 3060",
                Company = "Nvidia",
                VendorName = "Gigabyte",
                Vendor = vendr1,
                Price = 400,
                ImageUrl = "gig3060.png",
                OverView = "Видеокарта базируется на ядре GPU GA106-300. Чип располагает 3584 ядрами CUDA, на 35% меньше чем у RTX 3060 Ti с 4864 CUDA ядрами. Чип Ampere имеет 64 блока растеризации ROP, 112 блока текстурирования TMU. Карта RTX 3060 получила память GDDR6 15 ГГц объемом 12ГБ с пропускной способностью 360 ГБ/с. Шина памяти имеет разрядность 192 бит. Теплопакет TDP RTX 3060 ожидается на уровне 170 Вт, при этом питается карта через 8-pin коннектор. Вычислительная производительность 12,7 TFLOPS (FP32)."
            };
            var vc2 = new Videocard()
            {
                Name = "RX 6900XT",
                Company = "AMD",
                VendorName = "Powercolor",
                Vendor = vendr2,
                Price = 900,
                ImageUrl = "powerc6900xt.png",
                OverView = "Видеокарта AMD RX 6900 XT построена на ядре GPU Navi 21 XT. Чип располагает 5120 потоковыми процессорами (ядрами) и имеет 80 вычислительных блока (CU). Чип RX 6900 XT выполнен по 7-нм техпроцессу (у NVIDIA RTX 3000 8 нм). Видеокарта оснащена 16 гигабайтами GDDR6 памяти, работающей на эффективной частоте 16000 МГц. Шина 256 бит обеспечивает пропускную способность памяти 512 ГБ/с. Пиковая вычислительная производительность FP32 23.04 TFLOPS. TDP видеокарты составляет 300 Вт, при этом питание карты осуществляется через 8 + 8 pin коннектор."
            };
            var vc3 = new Videocard()
            {
                Name = "RX 6600",
                Company = "AMD",
                VendorName = "Gigabyte",
                Vendor = vendr1,
                Price = 300,
                ImageUrl = "gig6600.jpg",
                OverView = "Видеокарта AMD RX 6600 построена на ядре GPU Navi 23 XL. Чип располагает 1792 потоковыми процессорами (ядрами) и имеет 28 вычислительных блока (CU), 64 блока растеризации (ROP), 118 текстурных блока (TMU). Чип RX 6600 выполнен по 7-нм техпроцессу. Видеокарта оснащена 8 гигабайтами GDDR6 памяти, работающей на эффективной частоте 14000 МГц. Шина 128 бит обеспечивает пропускную способность памяти 256 ГБ/с. Пиковая вычислительная производительность FP32 9.07 TFLOPS. Хешрейт эфира 27 MH/s. TDP видеокарты составляет 150 Вт, при этом питание карты осуществляется через 8-pin коннектор."
            };
            db.Vendors.Add(vendr1);
            db.Vendors.Add(vendr2);
            db.VideoCards.AddRange(new[] { vc1, vc2, vc3 });
            db.SaveChanges();
        }

        public IActionResult Search(string searchString)//имя параметра строго совпадает с передаваемым
        {
            var result = new List<Videocard>();
            if (!string.IsNullOrEmpty(searchString))
            {
                Regex rg = new Regex(@"\w+", RegexOptions.IgnoreCase);
                var matches = rg.Matches(searchString);
                foreach (var word in matches)
                {
                    var list = db.VideoCards.Where(b => b.VendorName.Contains(word.ToString()) || b.Name.Contains(word.ToString())).ToList();
                    result.AddRange(list);
                }
                result = (from item in result select item).Distinct().ToList();
            }
            if (result.Count == 0)
            {
                ViewBag.Msg = "К сожалению, по вашему запросу ничего не нашлось. Попробуйте поискать что-нибудь другое";
                return View("SearchResults");
            }
            return View("SearchResults", result);
        }
        private SelectList GetVendors()
        {
            var vendors = db.Vendors.ToList();
            vendors.Insert(0, new Vendor { Name = "Все", Id = 0 });
            SelectList list = new SelectList(vendors, "Id", "Name");
            return list;
        }

        public IActionResult Index(int vendorId = 0)
        {
            var vcs = vendorId > 0 ? db.VideoCards.Include(b => b.Vendor)
                                            .Where(b => b.Vendor.Id == vendorId).ToList() :
                                         db.VideoCards.Include(b => b.Vendor).ToList();

            ViewBag.Vendors = GetVendors();
            return View(vcs);
            //return View(db.VideoCards.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Buy(int? id)
        {
            if (id == null) return RedirectToAction("Index");
            ViewBag.VCId = id;
            return View();
        }
        [HttpPost]
        public string Buy(Order order)
        {
            db.Orders.Add(order);
            // сохраняем в бд все изменения
            db.SaveChanges();
            return order.User + ", спасибо за покупку!";
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
