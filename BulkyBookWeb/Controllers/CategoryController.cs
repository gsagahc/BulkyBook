using BulkyBookWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller
    {
        public readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext contex) 
        {
            _context = contex;
        }
        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View("Index", categories);
        }
        public IActionResult Create()
        {
           return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePost(BulkyBookWeb.Models.Category category)
        {
            bool nameExist = await _context.Categories.AnyAsync(c => c.Name.ToLower() == category.Name.ToLower());
              

            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(category.Name) || nameExist)
                {
                    ModelState.AddModelError("", "The Display Order cannot exactly match the Name.");
                    return View(category);
                }
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public async Task<IActionResult> Edit(BulkyBookWeb.Models.Category category)
        {
            bool nameExist = await _context.Categories.AnyAsync(c => c.Name.ToLower() == category.Name.ToLower());


            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(category.Name) || nameExist)
                {
                    ModelState.AddModelError("", "The Display Order cannot exactly match the Name.");
                    return View(category);
                }
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
          
                _context.Categories.Remove(category);
                _context.SaveChanges();
                TempData["success"] = "Category deleted successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
