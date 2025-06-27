using EComerceTestSamir.Data;
using EComerceTestSamir.Models;
using EComerceTestSamir.Repositories;
using EComerceTestSamir.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EComerceTestSamir.Areas.Admin.Controllers
{


    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository categoryRepository;// = new CategoryRepository();
        public CategoryController(ICategoryRepository _categoryRepository)
        {
            categoryRepository = _categoryRepository;
        }

        [Authorize(Roles = $"{SD.Admin},{SD.SuperAdmin},{SD.Customer}")]
        public IActionResult Index()
        {
            var categories = categoryRepository.Get();

            return View(categories.ToList());
        }
        [Authorize(Roles = $"{SD.Admin},{SD.SuperAdmin}")]

        public IActionResult Create()
        {
            return View(new Category { });
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.Admin},{SD.SuperAdmin}")]

        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
               await categoryRepository.CreateAsync(category);
               await categoryRepository.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        [Authorize(Roles = $"{SD.Admin},{SD.SuperAdmin}")]

        public IActionResult Edit(int id)
        {
            var categories = categoryRepository.GetOne(e=>e.Id ==id);
            if (categories is not null)
            {
                return View(categories);
            }
            return RedirectToAction(actionName: "NotFoundPage", controllerName: "Home");

        }
        [HttpPost]
        [Authorize(Roles = $"{SD.Admin},{SD.SuperAdmin}")]

        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                categoryRepository.Update(category);
                await categoryRepository.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(category);
            }


        }
        [Authorize(Roles = $"{SD.Admin},{SD.SuperAdmin}")]

        public async Task<IActionResult> Delete(int id)
        {
            var category = categoryRepository.GetOne(e=>e.Id==id);
            if (category is not null)
            {
                categoryRepository.Delete(category);
               await categoryRepository.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(actionName: "NotFoundPage", controllerName: "Home");
        }
    }
}
