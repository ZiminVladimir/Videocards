using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Videocards.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace Videocards.Controllers
{
    public class VideocardsController : Controller
    {
        private readonly VCContext _context;
        IWebHostEnvironment _environment;

        public VideocardsController(VCContext context, IWebHostEnvironment env)
        {
            _context = context;
            _environment = env;
        }


        //public async Task<IActionResult> Index()
        //{
        //    var vccontext = _context.VideoCards.Include(v => v.Vendor);
        //    return View(await vccontext.ToListAsync());
        //}

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vc = await _context.VideoCards
                .Include(v => v.Vendor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vc == null)
            {
                return NotFound();
            }

            return View(vc);
        }

        public IActionResult Create()
        {
            var vendors = _context.Vendors.ToList();
            SelectList vendrs = new SelectList(vendors, "Id", "Name");
            ViewBag.Vendors = vendrs;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,VendorName,Company,ImageUrl,OverView")] Videocard vc, IFormFile upload)
        {
            if (upload != null)
            {
                string fileName = Path.GetFileName(upload.FileName);
                //check for graphics
                if (CheckByGraphicsFormat(fileName))
                {
                    Save(upload, fileName);
                    vc.ImageUrl = fileName;
                }
            }
            if (ModelState.IsValid)
            {
                var list = _context.Vendors.Where(v => v.Id.ToString() == vc.VendorName);
                var vendr = list.FirstOrDefault();
                vc.Vendor = vendr;
                vc.VendorName = vendr.Name;
                _context.Add(vc);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View("Create");
        }




        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vc = await _context.VideoCards.FindAsync(id);
            if (vc == null)
            {
                return NotFound();
            }
            return View(vc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Company,VendorId,ImageUrl,Vendor,OverView")] Videocard vc, IFormFile upload)
        {
            if (id != vc.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (upload != null)
                {
                    string fileName = System.IO.Path.GetFileName(upload.FileName);
                    //check for graphics
                    if (CheckByGraphicsFormat(fileName))
                    {
                        Save(upload, fileName);
                        vc.ImageUrl = "/Images/" + fileName;
                    }
                }
                try
                {
                    _context.Update(vc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VCExists(vc.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            //ViewData["VendorName"] = new SelectList(_context.Vendors, "Id", "Name", vc.VendorName);
            return View(vc);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vc = await _context.VideoCards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vc == null)
            {
                return NotFound();
            }

            return View(vc);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.VideoCards.FindAsync(id);
            _context.VideoCards.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private void Save(IFormFile upload, string fileName)
        {
            Bitmap image = new Bitmap(upload.OpenReadStream());
            //int width = 150;
            //int height = 200;
            //Bitmap smallImage = Resize(image, width, height);
            string path = "/wwwroot/Images/" + fileName;
            var root = _environment.ContentRootPath;
            path = root + path;
            // сохраняем файл в папку  в каталоге wwwroot
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                image.Save(fileStream, ImageFormat.Jpeg);
            }
        }

        private bool VCExists(int id)
        {
            return _context.VideoCards.Any(e => e.Id == id);
        }

        private bool CheckByGraphicsFormat(string fileName)
        {
            var ext = fileName.Substring(fileName.Length - 3);
            return string.CompareOrdinal(ext, "png") == 0 || string.CompareOrdinal(ext, "jpg") == 0;
        }
    }
}
