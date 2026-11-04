using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public LostItemReportsController(ApplicationDbContext context)
        {
            _context = context;
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: LostItemReports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Location,DateLost,ContactInfo,Status")] LostItemReport lostItemReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lostItemReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lostItemReport);
        }

        // GET: LostItemReports/Edit/5
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Location,DateLost,ContactInfo,Status")] LostItemReport lostItemReport)
        {
            if (id != lostItemReport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
    }
}
