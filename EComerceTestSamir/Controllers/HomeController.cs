using EComerceTestSamir.Data;
using EComerceTestSamir.Models;
using EComerceTestSamir.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EComerceTestSamir.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public readonly ApplicationDbContext Context = new();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(int categoryId,string? query, double minPrice,double maxPrice,int pageNumber = 1)
        {
            IQueryable<Product> products = Context.Products.Include(e => e.Category);
            var categories = Context.Categories;
            ViewData["categories"] = categories.ToList();
            if (categoryId>0&&categoryId<categories.Count())
            {
                products = products.Where(e=>e.CategoryId==categoryId);
                ViewBag.CategoryId = categoryId;
            }
            if (query is not null)
            {
                products = products.Where(e=>e.Name.Contains(query));
                ViewBag.Query = query;
            }
            if (minPrice > 0)
            {
                products = products.Where(e => e.Price >= (decimal)minPrice);
                ViewBag.MinPrice = minPrice;
            }
            if (maxPrice > 0)
            {
                products = products.Where(e => e.Price <= (decimal)maxPrice);
                ViewBag.MaxPrice = maxPrice;
            }
            products = products.Skip((pageNumber - 1) * 8).Take(8);//        products = products.Skip((page - 1) * 8).Take(8);

            ViewBag.TotalCountOfProduct = Math.Ceiling(products.Count() / 8.0);
            return View(products.ToList());
        }
        public IActionResult DetailsItem(int id)
        {
            var product = Context.Products.FirstOrDefault(e=>e.Id==id);
            if(product is not null)
            {
                var relatedProduct = Context.Products.Where(e => e.Name.Contains(product.Name.Substring(0,5))&&e.Id!=product.Id).Skip(0).Take(4).ToList();
                var SameProductOFCategory = Context.Products.Include(e => e.Category).Where(e => e.CategoryId == product.CategoryId && e.Id != product.Id).Skip(1).Take(4).ToList();
                var ReturnProduct = new ProductRelated()
                {
                    product = product,
                    relatedProduct = relatedProduct,
                    relatedCategory = SameProductOFCategory
                };
               
                return View(ReturnProduct);

            }
            return RedirectToAction(nameof(NotFoundPage));
        }
        public IActionResult NotFoundPage()
        {
            return View();
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
}
