using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReFound.Data;
using ReFound.Models;

namespace ReFound.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalFound = await _context.FoundItems.CountAsync();
        ViewBag.UnclaimedCount = await _context.FoundItems.CountAsync(f => f.Status == "Unclaimed");
        ViewBag.LostReportsCount = await _context.LostItemReports.CountAsync();
        ViewBag.ClaimRequestsCount = await _context.ClaimRequests.CountAsync();

        var recentFoundItems = await _context.FoundItems
            .Where(f => f.Status == "Unclaimed")
            .OrderByDescending(f => f.DateFound)
            .Take(5)
            .ToListAsync();

        return View(recentFoundItems);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}