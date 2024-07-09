using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MoviePro.Data;
using MoviePro.Models.Database;
using MoviePro.Models.Settings;

namespace MoviePro.Controllers
{
    [Authorize]
    public class CollectionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AppSettings _appSettings;

        public CollectionsController(ApplicationDbContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        // GET: Collections
        public async Task<IActionResult> Index()
        {
            var defaultCollectionName = _appSettings.MovieProSettings.DefaultCollection.Name ?? Environment.GetEnvironmentVariable("Name");

            var collections = await _context.Collection.Where(c => c.Name != defaultCollectionName)
                .OrderBy(x => x.Name)
                .ToListAsync();

            //var collections = await _context.Collection
                //.OrderBy(x => x.Name)
                //.ToListAsync();

            return View(collections);
        }

        public IActionResult Create()
        {
            return View();
        }


        // POST: Collections/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Collection collection)
        {
            _context.Add(collection);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Collections");
        }

        // GET: Collections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collection.FindAsync(id);
            if (collection == null)
            {
                return NotFound();
            }
            return View(collection);
        }

        // POST: Collections/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Collection collection)
        {
            if (id != collection.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var defaultCollectionName = _appSettings.MovieProSettings.DefaultCollection.Name ?? Environment.GetEnvironmentVariable("Name");

                    if (collection.Name == defaultCollectionName)
                    {
                        return RedirectToAction("Index", "Collections");
                    }

                    _context.Update(collection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollectionExists(collection.Id))
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
            return View(collection);
        }

        // GET: Collections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collection
                .FirstOrDefaultAsync(m => m.Id == id);
            if (collection == null)
            {
                return NotFound();
            }

            var defaultCollectionName = _appSettings.MovieProSettings.DefaultCollection.Name ?? Environment.GetEnvironmentVariable("Name");

            if (collection.Name == defaultCollectionName)
            {
                return RedirectToAction("Index", "Collections");
            }

            return View(collection);
        }

        // POST: Collections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var collection = await _context.Collection.FindAsync(id);
            _context.Collection.Remove(collection);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Collections");
        }

        private bool CollectionExists(int id)
        {
            return _context.Collection.Any(e => e.Id == id);
        }
    }
}
