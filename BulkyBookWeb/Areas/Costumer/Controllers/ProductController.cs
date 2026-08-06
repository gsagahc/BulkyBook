using BulkyBook.Business.Services.IServices;
using BulkyBook.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyBookWeb.Areas.Costumer.Controllers
{
    [Area("Costumer")]
    public class ProductController : Controller
    {

       private readonly IProductService _productService;

        public ProductController(IProductService productService) 
        {
            _productService  = productService;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync() ;
            return View("Index", products);
        }
        public async Task<IActionResult> Create()
        {
           return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePost(BulkyBook.Models.Product product)
        {
            bool titleExist = await _productService.IsProductTitleUniqueAsync(product.Title);

            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(product.Title) || titleExist)
                {
                    ModelState.AddModelError("", "The Display Order cannot exactly match the Name.");
                    return View(product);
                }
                await _productService.CreateProductAsync(product);
                TempData["success"] = "Product created successfully";
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
            var product =  _productService.GetProductByIdAsync(id.Value).Result;
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public async Task<IActionResult> Edit(BulkyBook.Models.Product product)
        {
            bool titleNotExist = false;
            if (!String.IsNullOrEmpty(product.Title) && 
                  await  _productService.IsProductTitleUniqueAsync(product.Title, product.Id))
            {
                 titleNotExist = await _productService.IsProductTitleUniqueAsync(product.Title, product.Id);
            }


            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(product.Title) || !titleNotExist)
                {
                    ModelState.AddModelError("", "The Display Order cannot exactly match the Title.");
                    return View(product);
                }
                await _productService.UpdateProductAsync(product);
                TempData["success"] = "Product updated successfully";
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
            var product =  _productService.GetProductByIdAsync(id.Value).Result;
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
          
            await _productService.DeleteProductAsync(id);
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");
         
            
        }
    }
}
