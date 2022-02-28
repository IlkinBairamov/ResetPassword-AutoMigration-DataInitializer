using FrontToBack.DataAccessLayer;
using FrontToBack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _dbContext;

        public CategoryController(AppDbContext dbContext)   
        {
            _dbContext = dbContext;
        }
    
        public async Task<IActionResult> Index()
        {
            var categories = await _dbContext.Categories.ToListAsync();
            return View(categories);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var category = await _dbContext.Categories.FindAsync(id);
            if (category==null)
                return NotFound();

            return View(category);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var isExsistCategory = await _dbContext.Categories.AnyAsync(x => x.Name.ToLower() == category.Name.ToLower());
            if (isExsistCategory)
            {
                ModelState.AddModelError("Name", "This Category Already Exsist");
                return View();
            }

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Category category)
        {
            if (id == null)
                return NotFound();

            if (id != category.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View();

            var existCategory = await _dbContext.Categories.FindAsync(id);
            if (existCategory == null)
                return NotFound();

            var isExist = await _dbContext.Categories
                .AnyAsync(x => x.Name.ToLower() == category.Name.ToLower() &&
                               x.Id != id);
            if (isExist)
            {
                ModelState.AddModelError("Name", "Bu adda kateqoriya var.");
                return View();
            }

            existCategory.Name = category.Name;
            existCategory.Description = category.Description;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteCategory(int? id)
        {
            if (id == null)
                return BadRequest();

            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
