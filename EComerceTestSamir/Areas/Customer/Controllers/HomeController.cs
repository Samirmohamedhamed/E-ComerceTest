using EComerceTestSamir.Data;
using EComerceTestSamir.Models;
using EComerceTestSamir.Models.ViewModels;
using EComerceTestSamir.Repositories;
using EComerceTestSamir.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace EComerceTestSamir.Areas.Customer.Controllers;
[Area("Customer")]
public class HomeController : Controller
{

    public readonly IProductRepository productRepository;
    public readonly ICategoryRepository categoryRepository;

    public HomeController(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        this.productRepository = productRepository;
        this.categoryRepository = categoryRepository;
    }

    public IActionResult Index(int categoryId = 0, string? query = null, double minPrice = 0, double maxPrice = 0, int pageNumber = 1)
    {
        // 1. Load Categories using repository
        var categories = categoryRepository.Get(); // Assuming simple Get with no params
        ViewData["categories"] = categories.ToList();

        // 2. Build filter expression dynamically
        Expression<Func<Product, bool>>? filter = p => true;

        if (categoryId > 0)
        {
            filter = Combine(filter, p => p.CategoryId == categoryId);
            ViewBag.CategoryId = categoryId;
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            filter = Combine(filter, p => p.Name.Contains(query));
            ViewBag.Query = query;
        }

        if (minPrice > 0)
        {
            filter = Combine(filter, p => p.Price >= (decimal)minPrice);
            ViewBag.MinPrice = minPrice;
        }

        if (maxPrice > 0)
        {
            filter = Combine(filter, p => p.Price <= (decimal)maxPrice);
            ViewBag.MaxPrice = maxPrice;
        }

        int pageSize = 8;
        int skip = (pageNumber - 1) * pageSize;

        // 3. Get filtered and paginated products
        var products = productRepository.Get(
            expression: filter,
            Includes: [p => p.Category],
            skip: skip,
            take: pageSize
        );

        // 4. Count total for pagination
        var totalCount = productRepository.Get(expression: filter).Count();
        ViewBag.TotalCountOfProduct = Math.Ceiling(totalCount / (double)pageSize);

        return View(products);
    }

    // Helper to combine expressions
    private Expression<Func<Product, bool>> Combine(Expression<Func<Product, bool>> expr1, Expression<Func<Product, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(Product));

        var body = Expression.AndAlso(
            Expression.Invoke(expr1, parameter),
            Expression.Invoke(expr2, parameter)
        );

        return Expression.Lambda<Func<Product, bool>>(body, parameter);
    }


    public async Task<IActionResult> DetailsItem(int id)
    {
        // 1. ?????? ???????
        var product = productRepository.GetOne(p => p.Id == id);

        if (product is null)
            return RedirectToAction(nameof(NotFoundPage));

        // 2. ?????? ?????? ??????
        var relatedProduct = productRepository.Get(
            expression: p => p.Name.Contains(product.Name.Substring(0, 5)) && p.Id != product.Id,
            take: 4
        );

        // 3. ?????? ?? ??? ?????
        var relatedCategory = productRepository.Get(
            expression: p => p.CategoryId == product.CategoryId && p.Id != product.Id,
            Includes: [p => p.Category],
            skip: 1,
            take: 4
        );

        var topProduct = productRepository
            .Get(expression: p => p.Id != product.Id)
            .OrderByDescending(p => p.Traffic)
            .Take(4)
            .ToList();

        var viewModel = new ProductRelated
        {
            product = product,
            relatedProduct = relatedProduct.ToList(),
            relatedCategory = relatedCategory.ToList(),
            TopProduct = topProduct
        };

        product.Traffic++;
        productRepository.Update(product);
        await productRepository.CommitAsync();

        return View(viewModel);
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

