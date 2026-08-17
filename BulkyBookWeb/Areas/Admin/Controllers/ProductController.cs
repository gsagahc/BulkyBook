using BulkyBook.Business.Services.IServices;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security;


namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Employee")]
    public class ProductController : Controller
    {

       private readonly IProductService _productService;
       private readonly ICategoryService _categoryService;
       private readonly IWebHostEnvironment _webHostEnvironment;
        
        public ProductController(IProductService productService, ICategoryService categoryService, IWebHostEnvironment webHostEnvironment) 
        {
            _productService  = productService;
            _categoryService = categoryService;
            _webHostEnvironment = webHostEnvironment;
        }
        [AllowAnonymous]
      
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View("Index", products );
        }
      
        public async Task<IActionResult> Upsert(int? id)
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            ProductVM productVM = new()
            {
                CategoryList = categories.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //create
                return View(productVM);
            }
            else
            {
                productVM.Product = await _productService.GetProductByIdAsync(id.Value);
                return View(productVM);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Upsert")]
    
        public async Task<IActionResult> UpsertPost(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {

                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine("images", "products");
                    string finalPath = Path.Combine(wwwRootPath, productPath);


                    if (!Directory.Exists(finalPath))
                        Directory.CreateDirectory(finalPath);

                    //save the new image
                    using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = Path.Combine(@"\", productPath, fileName).Replace("\\", "/");
                }

                if (productVM.Product.Id == null || productVM.Product.Id == 0)
                {
                    //create
                    await _productService.CreateProductAsync(productVM.Product);
                }
                else
                {
                    await _productService.UpdateProductAsync(productVM.Product);

                }


                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                var categories = await _categoryService.GetAllCategoriesAsync();

                productVM = new()
                {
                    CategoryList = categories.Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    })
                };
                return View(productVM);
            }

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
               
        
        #region API CALLS
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync(true);
            return Json(new { data = products });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return Json(new { success = false, message = "Invalid ID" });
            }

            var productToBeDeleted = await _productService.GetProductByIdAsync(id.Value);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }
            if (!String.IsNullOrEmpty(productToBeDeleted.ImageUrl))
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\','/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            await _productService.DeleteProductAsync(id.Value);
          //  TempData["success"] = "Product deleted successfully";
          //  return RedirectToAction("Index");

            return Json(new { success = true, message = "Product deleted successfully" });
           
        


        }
        #endregion
    }
}
