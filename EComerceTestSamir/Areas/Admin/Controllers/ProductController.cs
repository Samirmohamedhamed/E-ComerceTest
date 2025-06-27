using EComerceTestSamir.Data;
using EComerceTestSamir.Models;
using EComerceTestSamir.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EComerceTestSamir.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index()
        {
            var products = _context.Products.Include(e=>e.Category).Include(e=>e.Brand)
                .Select(e=> new ProductWithCategoryWithBrandVM()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Price = e.Price,
                    Quantity = e.Quantity,
                    Rate = e.Rate,
                    CategoryName =e.Category.Name,
                    BrandName=e.Brand.Name,
                    Status = e.Status,
                   

                });
            return View(products.ToList());
        }
        public IActionResult Create()
        {
            var categories = _context.Categories;
            var brands = _context.Brands;
            var returnInfoCategoryAndBrand = new CategoryWithBrandInfoVM()
            {
                Categories = categories.ToList(),
                Brands = brands.ToList(),
                Product = new Product() { }
            };
            return View(returnInfoCategoryAndBrand);
        }
        [HttpPost]
        public IActionResult Create(Product product,IFormFile MainImg)
        {
            var categories = _context.Categories.Where(e=>e.Status==true);
            var brands = _context.Brands.Where(e => e.Status == true);
            var returnInfoCategoryAndBrand = new CategoryWithBrandInfoVM()
            {
                Categories = categories.ToList(),
                Brands = brands.ToList(),
                Product = product,
            };
            ModelState.Remove("Product.Brand");
            ModelState.Remove("Product.Category");
            if (ModelState.IsValid && MainImg != null && MainImg.Length>0)
            {
                var fileName = Guid.NewGuid().ToString()+Path.GetExtension(MainImg.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\images"
                    ,fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    MainImg.CopyTo(stream);
                }
                product.MainImg = fileName;
                _context.Add(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(returnInfoCategoryAndBrand);
        }
        public IActionResult Edit(int id)
        {
            var categories = _context.Categories;
            var brands = _context.Brands;
            var product = _context.Products.Find(id);
            if (product is not null)
            {
                var returnInfoCategoryAndBrand = new CategoryWithBrandInfoVM()
                {
                    Categories = categories.ToList(),
                    Brands = brands.ToList(),
                    Product = product
                };
                return View(returnInfoCategoryAndBrand);

            }
            return RedirectToAction(actionName: "NotFoundPage", controllerName: "Home");

        }
        [HttpPost]
        public IActionResult Edit(Product product,IFormFile MainImg)
        {
            
            ModelState.Remove("Product.Brand");
            ModelState.Remove("Product.Category");
            ModelState.Remove("MainImg"); 

            var productInDb = _context.Products.AsNoTracking()
                .FirstOrDefault(e=>e.Id == product.Id);
            if(ModelState.IsValid &&productInDb!=null)
            {
                if (MainImg != null && MainImg.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(MainImg.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"
                        , fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        MainImg.CopyTo(stream);
                    }
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"
                        , productInDb.MainImg);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                    product.MainImg = fileName;
                }
                else
                {
                    product.MainImg = productInDb.MainImg;
                }
                _context.Update(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            var categories = _context.Categories;
            var brands = _context.Brands;
            var returnInfoCategoryAndBrand = new CategoryWithBrandInfoVM()
            {
                Categories = categories.ToList(),
                Brands = brands.ToList(),
                Product = product,
            };
            return View(returnInfoCategoryAndBrand);
        }
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product is not null)
            {
                var oldFileName = product.MainImg;
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"
                    , oldFileName);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
                _context.Remove(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(actionName: "NotFoundPage", controllerName: "Home");
        }
        public IActionResult DeleteImage(int id)
        {
            var product = _context.Products.Find(id);
            if (product is not null)
            {
                var oldFileName = product.MainImg;
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"
                    , oldFileName);
                Console.WriteLine(oldPath);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
                product.MainImg = "dummyImage.png";
                _context.Products.Update(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Edit), new{id=id});
            }
            return RedirectToAction(actionName: "NotFoundPage", controllerName: "Home");

        }
        public IActionResult ReplaceImage(int id,IFormFile MainImg)
        {
            var product = _context.Products.Find(id);
            if (product is not null)
            {
                var oldFileName = product.MainImg;
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"
                    , oldFileName);
                Console.WriteLine(oldPath);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
                if ( MainImg != null && MainImg.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(MainImg.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"
                        , fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        MainImg.CopyTo(stream);
                    }
                    product.MainImg = fileName;
                    _context.Products.Update(product);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Edit), new { id = id });
                }
                return RedirectToAction(nameof(Edit), new { id = id });


            }
            return RedirectToAction(actionName: "NotFoundPage", controllerName: "Home");

        }
        public IActionResult Details(int id)
        {
            var ItemProduct = _context.Products.Find(id);
            return View(ItemProduct);
        }

    }
}
