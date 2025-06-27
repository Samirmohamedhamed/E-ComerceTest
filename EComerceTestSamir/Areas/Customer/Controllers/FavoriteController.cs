using EComerceTestSamir.Models;
using EComerceTestSamir.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EComerceTestSamir.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class FavoriteController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IFavoritesRepository favoritesRepository;

        public FavoriteController(IProductRepository productRepository,UserManager<ApplicationUser> userManager,IFavoritesRepository favoritesRepository)
        {
            this.productRepository = productRepository;
            this.userManager = userManager;
            this.favoritesRepository = favoritesRepository;
        }
        public async Task<IActionResult> AddToFavorites(int id)
        {
            var user = await userManager.GetUserAsync(User);
            if(user is not null)
            {
                var product = productRepository.GetOne(e => e.Id == id);
                if(product is not null)
                {
                     Favorite favotir= new Favorite()
                     {
                         ProductId = id,
                         ApplicationId = user.Id,
                         CreatedAt = DateTime.UtcNow,
                     };
                   await  favoritesRepository.CreateAsync(favotir);
                   await favoritesRepository.CommitAsync();
                    TempData["Notification"] = "Add Product To Favorites";
                    return RedirectToAction("Index", "Home", new {area="Customer"});
                }
                return BadRequest();
            }
            return BadRequest();
        } 
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            if(user is not null)
            {
                var favorite = favoritesRepository.Get(e => e.ApplicationId == user.Id, Includes: [e=>e.Product]).ToList();

                return View(favorite);
            }
            return BadRequest();
        }
        public async Task<IActionResult> DeleteItemFromFavorite(int productId)
        {
            var user = await userManager.GetUserAsync(User);
            if(user is not null)
            {
                var favorite = favoritesRepository.GetFirstOrDefault(e => e.ApplicationId == user.Id&&e.ProductId== productId);
                if(favorite is not null)
                {
                    favoritesRepository.Delete(favorite);
                    await favoritesRepository.CommitAsync();

                }

                return RedirectToAction("Index");
            }
            return BadRequest();
        }
    }
}
