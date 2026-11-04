using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReFound.Data;
using ReFound.Models;

namespace ReFound.Controllers
{
    public class ClaimRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClaimRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Populate Found Item dropdown
        private async Task PopulateFoundItemsAsync(int? selectedId = null)
        {
            var foundItems = await _context.FoundItems
                .OrderBy(f => f.ItemName)
                .ToListAsync();

            ViewBag.FoundItems = foundItems.Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = $"ID: {f.Id} - {f.ItemName}",
                Selected = f.Id == selectedId
            }).ToList();
        }

        // GET: ClaimRequests
        public async Task<IActionResult> Index()
        {
            var claimRequests = await _context.ClaimRequests
                .Include(c => c.FoundItem)
                .ToListAsync();

            return View(claimRequests);
        }

        // GET: ClaimRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var claimRequest = await _context.ClaimRequests
                .Include(c => c.FoundItem)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (claimRequest == null)
                return NotFound();

            return View(claimRequest);
        }

        // GET: ClaimRequests/Create
        public async Task<IActionResult> Create()
        {
            await PopulateFoundItemsAsync();
            return View();
        }

        // POST: ClaimRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClaimRequest claimRequest)
        {
            var foundItemExists = await _context.FoundItems
                .AnyAsync(f => f.Id == claimRequest.FoundItemId);

            if (!foundItemExists)
            {
                ModelState.AddModelError(
                    "FoundItemId",
                    "Please select a valid found item."
                );
            }

            if (ModelState.IsValid)
            {
                claimRequest.Status = "Pending";
                claimRequest.RequestDate = DateTime.Today;
                claimRequest.FoundItem = null;

                _context.ClaimRequests.Add(claimRequest);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            await PopulateFoundItemsAsync(claimRequest.FoundItemId);
            return View(claimRequest);
        }

        // GET: ClaimRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var claimRequest = await _context.ClaimRequests.FindAsync(id);

            if (claimRequest == null)
                return NotFound();

            return View(claimRequest);
        }

        // POST: ClaimRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClaimRequest claimRequest)
        {
            if (id != claimRequest.Id)
                return NotFound();

            var foundItemExists = await _context.FoundItems
                .AnyAsync(f => f.Id == claimRequest.FoundItemId);

            if (!foundItemExists)
            {
                ModelState.AddModelError(
                    "FoundItemId",
                    "Please select a valid found item."
                );
            }

            if (ModelState.IsValid)
            {
                try
                {
                    claimRequest.FoundItem = null;
                    _context.Update(claimRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ClaimRequests.Any(e => e.Id == id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(claimRequest);
        }

        // GET: ClaimRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var claimRequest = await _context.ClaimRequests
                .Include(c => c.FoundItem)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (claimRequest == null)
                return NotFound();

            return View(claimRequest);
        }

        // POST: ClaimRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var claimRequest = await _context.ClaimRequests.FindAsync(id);

            if (claimRequest != null)
            {
                _context.ClaimRequests.Remove(claimRequest);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}