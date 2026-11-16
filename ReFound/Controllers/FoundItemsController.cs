using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
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
    public class FoundItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;

        public FoundItemsController(
            ApplicationDbContext context,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _env = env;
        }

        // GET: FoundItems
        public async Task<IActionResult> Index()
        {
            return View(await _context.FoundItems.ToListAsync());
        }

        // SmartSEARCH API - Hybrid Synonym/Keyword + Local Ollama AI Ranking
        [HttpGet("/FoundItems/SmartSearch")]
        public async Task<IActionResult> SmartSearch(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(await _context.FoundItems.Select(i => i.Id).ToListAsync());
            }

            var items = await _context.FoundItems
                .Select(i => new { i.Id, i.ItemName, i.Category, i.Description, i.LocationFound })
                .ToListAsync();

            // 1. Fast local keyword + campus synonym scoring
            var synonymGroups = new List<string[]>
            {
                new[] { "jacket", "hoodie", "sweater", "coat", "cardigan", "windbreaker", "clothing" },
                new[] { "tumbler", "bottle", "flask", "aquaflask", "hydroflask", "mug", "drink" },
                new[] { "earphones", "earbuds", "airpods", "headphones", "headset", "audio" },
                new[] { "charger", "adapter", "power", "brick", "cable", "cord" },
                new[] { "usb", "flash", "drive", "thumb", "sandisk", "storage" },
                new[] { "glasses", "eyeglasses", "spectacles", "frames", "case" },
                new[] { "bag", "backpack", "pouch", "wallet", "purse" },
                new[] { "phone", "iphone", "android", "mobile", "cellphone" },
                new[] { "umbrella", "rain", "raincoat" }
            };

            var queryWords = query.ToLowerInvariant()
                .Split(new[] { ' ', ',', '.', '-', '/', '(', ')' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 1)
                .ToHashSet();

            var expandedTerms = new HashSet<string>(queryWords);
            foreach (var word in queryWords)
            {
                foreach (var group in synonymGroups)
                {
                    if (group.Any(g => g.Contains(word) || word.Contains(g)))
                    {
                        foreach (var syn in group) expandedTerms.Add(syn);
                    }
                }
            }

            var scores = items.ToDictionary(i => i.Id, _ => 0);

            foreach (var item in items)
            {
                string title = item.ItemName.ToLowerInvariant();
                string desc = item.Description.ToLowerInvariant();
                string cat = item.Category.ToLowerInvariant();
                string loc = item.LocationFound.ToLowerInvariant();

                foreach (var term in expandedTerms)
                {
                    bool isOriginalWord = queryWords.Contains(term);
                    int weight = isOriginalWord ? 10 : 7;

                    if (title.Contains(term)) scores[item.Id] += weight * 3;
                    if (desc.Contains(term)) scores[item.Id] += weight * 2;
                    if (cat.Contains(term)) scores[item.Id] += weight;
                    if (loc.Contains(term)) scores[item.Id] += weight * 2;
                }
            }

            // 2. Also ask Ollama refound-model for semantic context boost
            try
            {
                var formattedItems = string.Join("\n", items.Select(i =>
                    $"ID {i.Id}: {i.ItemName} | Category: {i.Category} | Description: {i.Description} | Location: {i.LocationFound}"
                ));

                string endpoint = "http://localhost:11434/api/chat";
                var systemPrompt = @"You are a strict Lost & Found search matcher.
Given a USER QUERY and ITEMS, identify ONLY the item IDs that match the query.
First write a 1-sentence reason, then output the matching integer IDs. Return empty ids if nothing matches.";

                var requestPayload = new
                {
                    model = "refound-model",
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = $"USER QUERY: \"{query}\"\n\nITEMS:\n{formattedItems}" }
                    },
                    stream = false,
                    format = new
                    {
                        type = "object",
                        properties = new
                        {
                            reason = new { type = "string" },
                            ids = new { type = "array", items = new { type = "integer" } }
                        },
                        required = new[] { "reason", "ids" }
                    },
                    options = new { temperature = 0 }
                };

                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(15);
                var content = new StringContent(JsonSerializer.Serialize(requestPayload), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(jsonResponse);
                    var rawResult = doc.RootElement.GetProperty("message").GetProperty("content").GetString()?.Trim() ?? "{}";
                    using var resultDoc = JsonDocument.Parse(rawResult);

                    if (resultDoc.RootElement.TryGetProperty("ids", out var idsElement))
                    {
                        int rankBonus = 8;
                        foreach (var el in idsElement.EnumerateArray())
                        {
                            if (el.TryGetInt32(out int id) && scores.ContainsKey(id))
                            {
                                scores[id] += Math.Max(rankBonus, 2);
                                rankBonus -= 2;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ollama fallback to keyword/synonym search: {ex.Message}");
            }

            // Return all items with a positive match score, ordered highest score first
            var rankedIds = scores
                .Where(kv => kv.Value > 0)
                .OrderByDescending(kv => kv.Value)
                .Select(kv => kv.Key)
                .ToList();

            return Json(rankedIds);
        }

        // GET: FoundItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foundItem = await _context.FoundItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (foundItem == null)
            {
                return NotFound();
            }

            return View(foundItem);
        }

        // GET: FoundItems/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: FoundItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,ItemName,Description,Category,DateFound,LocationFound,Status")] FoundItem foundItem, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    foundItem.ImagePath = await SaveImageAsync(imageFile);
                }

                _context.Add(foundItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(foundItem);
        }

        // GET: FoundItems/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foundItem = await _context.FoundItems.FindAsync(id);
            if (foundItem == null)
            {
                return NotFound();
            }
            return View(foundItem);
        }

        // POST: FoundItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ItemName,Description,Category,DateFound,LocationFound,Status,ImagePath")] FoundItem foundItem, IFormFile? imageFile)
        {
            if (id != foundItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        foundItem.ImagePath = await SaveImageAsync(imageFile);
                    }

                    _context.Update(foundItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FoundItemExists(foundItem.Id))
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
            return View(foundItem);
        }

        // GET: FoundItems/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foundItem = await _context.FoundItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (foundItem == null)
            {
                return NotFound();
            }

            return View(foundItem);
        }

        // POST: FoundItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var foundItem = await _context.FoundItems.FindAsync(id);
            if (foundItem != null)
            {
                _context.FoundItems.Remove(foundItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FoundItemExists(int id)
        {
            return _context.FoundItems.Any(e => e.Id == id);
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