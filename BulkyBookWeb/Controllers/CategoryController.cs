using BulkyBookWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
        [ActionName ("Create")]
        public IActionResult CreatePost(BulkyBookWeb.Models.Category category)
        {
            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(category.Name) && _context.Categories.Any(c => c.Name == category.Name))
                {
                    ModelState.AddModelError("", "The Display Order cannot exactly match the Name.");
                    return View(category);
                }
                _context.Categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
