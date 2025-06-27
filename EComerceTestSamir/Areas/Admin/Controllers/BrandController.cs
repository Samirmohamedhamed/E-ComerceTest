using EComerceTestSamir.Data;
using EComerceTestSamir.Models;
using EComerceTestSamir.Repositories;
using EComerceTestSamir.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace EComerceTestSamir.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly IBrandRepository brandRepository;//= new BrandRepository();
        public BrandController(IBrandRepository _brandRepository)
        {
            brandRepository = _brandRepository;
        }
        public IActionResult Index()
        {
            var brands=brandRepository.Get();
            return View(brands.ToList());
        }
        public IActionResult Create()
        {
            return View(new Brand { });
        }
        [HttpPost]
        public async Task<IActionResult> Create(Brand brand)
        {
            if (ModelState.IsValid)
            {
               await brandRepository.CreateAsync(brand);
               await brandRepository.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }
        public IActionResult Edit(int id)
        {
            var brands = brandRepository.GetOne(e=>e.Id==id);
            if (brands is not null)
            {
                return View(brands);
            }
            return RedirectToAction(actionName: "NotFoundPage", controllerName: "Home");

        }
        [HttpPost]
        public async Task<IActionResult> EditAsync(Brand brand)
        {if (ModelState.IsValid)
            {
                brandRepository.Update(brand);
                await brandRepository.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
            

        }
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var brand = brandRepository.GetOne(e => e.Id == id);
            if (brand is not null)
            {
                brandRepository.Delete(brand);
               await brandRepository.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(actionName: "NotFoundPage", controllerName: "Home");
        }
    }
}
