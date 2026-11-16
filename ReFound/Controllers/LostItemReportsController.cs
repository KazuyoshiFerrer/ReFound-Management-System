using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReFound.Data;
using ReFound.Models;

namespace ReFound.Controllers
{
    public class LostItemReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public LostItemReportsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: LostItemReports
        public async Task<IActionResult> Index()
        {
            return View(await _context.LostItemReports.ToListAsync());
        }

        // GET: LostItemReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lostItemReport = await _context.LostItemReports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lostItemReport == null)
            {
                return NotFound();
            }

            return View(lostItemReport);
        }

        // GET: LostItemReports/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: LostItemReports/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Location,DateLost,ContactInfo,Status")] LostItemReport lostItemReport, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    lostItemReport.ImagePath = await SaveImageAsync(imageFile);
                }

                _context.Add(lostItemReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lostItemReport);
        }

        // GET: LostItemReports/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lostItemReport = await _context.LostItemReports.FindAsync(id);
            if (lostItemReport == null)
            {
                return NotFound();
            }
            return View(lostItemReport);
        }

        // POST: LostItemReports/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Location,DateLost,ContactInfo,Status,ImagePath")] LostItemReport lostItemReport, IFormFile? imageFile)
        {
            if (id != lostItemReport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        lostItemReport.ImagePath = await SaveImageAsync(imageFile);
                    }

                    _context.Update(lostItemReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LostItemReportExists(lostItemReport.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(lostItemReport);
        }

        // GET: LostItemReports/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lostItemReport = await _context.LostItemReports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lostItemReport == null)
            {
                return NotFound();
            }

            return View(lostItemReport);
        }

        // POST: LostItemReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lostItemReport = await _context.LostItemReports.FindAsync(id);
            if (lostItemReport != null)
            {
                _context.LostItemReports.Remove(lostItemReport);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LostItemReportExists(int id)
        {
            return _context.LostItemReports.Any(e => e.Id == id);
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/uploads/{uniqueFileName}";
        }
    }
}