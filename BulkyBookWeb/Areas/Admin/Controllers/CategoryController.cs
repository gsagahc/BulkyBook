using BulkyBook.Business.Services.IServices;
using BulkyBook.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {

       private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService) 
        {
            _categoryService  = categoryService;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync() ;
            return View("Index", categories);
        }
        public async Task<IActionResult> Create()
        {
           return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePost(BulkyBook.Models.Category category)
        {
            bool nameExist = !await _categoryService.IsCategoryNameUniqueAsync(category.Name);

            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(category.Name) || nameExist)
                {
                    ModelState.AddModelError("", "The Display Order cannot exactly match the Name.");
                    return View(category);
                }
                await _categoryService.CreateCategoryAsync(category);
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
            var category =  _categoryService.GetCategoryByIdAsync(id.Value).Result;
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public async Task<IActionResult> Edit(BulkyBook.Models.Category category)
        {
            bool nameNotExist = false;
            if (!String.IsNullOrEmpty(category.Name) && 
                  await  _categoryService.IsCategoryNameUniqueAsync(category.Name, category.Id))
            {
                 nameNotExist = await _categoryService.IsCategoryNameUniqueAsync(category.Name, category.Id);
            }


            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(category.Name) || !nameNotExist)
                {
                    ModelState.AddModelError("", "The Display Order cannot exactly match the Name.");
                    return View(category);
                }
                await _categoryService.UpdateCategoryAsync(category);
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
            var category =  _categoryService.GetCategoryByIdAsync(id.Value).Result;
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
           if (await _categoryService.IsCategoryUsedByProductAsync(id))
            {
                TempData["error"] = "Category cannot be deleted because it is used by a product.";
                return RedirectToAction("Index");
            }
            await _categoryService.DeleteCategoryAsync(id);
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
         
            
        }
    }
}
