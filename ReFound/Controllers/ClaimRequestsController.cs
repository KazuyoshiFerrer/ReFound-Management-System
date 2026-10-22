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
    public class ClaimRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClaimRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ClaimRequests
        public async Task<IActionResult> Index()
        {
            return View(await _context.ClaimRequests.ToListAsync());
        }

        // GET: ClaimRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claimRequest = await _context.ClaimRequests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (claimRequest == null)
            {
                return NotFound();
            }

            return View(claimRequest);
        }

        // GET: ClaimRequests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClaimRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FoundItemId,ClaimantUserId,ClaimDate,ProofOfOwnership,Status")] ClaimRequest claimRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(claimRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(claimRequest);
        }

        // GET: ClaimRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claimRequest = await _context.ClaimRequests.FindAsync(id);
            if (claimRequest == null)
            {
                return NotFound();
            }
            return View(claimRequest);
        }

        // POST: ClaimRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FoundItemId,ClaimantUserId,ClaimDate,ProofOfOwnership,Status")] ClaimRequest claimRequest)
        {
            if (id != claimRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(claimRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClaimRequestExists(claimRequest.Id))
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
            return View(claimRequest);
        }

        // GET: ClaimRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claimRequest = await _context.ClaimRequests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (claimRequest == null)
            {
                return NotFound();
            }

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
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClaimRequestExists(int id)
        {
            return _context.ClaimRequests.Any(e => e.Id == id);
        }
    }
}
