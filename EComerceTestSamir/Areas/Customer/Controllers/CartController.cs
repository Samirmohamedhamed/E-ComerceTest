using EComerceTestSamir.Models;
using EComerceTestSamir.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EComerceTestSamir.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IProductRepository productRepository;
        private readonly ICartRepository cartRepository;

        public CartController(UserManager<ApplicationUser> userManager, IProductRepository productRepository, ICartRepository cartRepository)
        {
            this.userManager = userManager;
            this.productRepository = productRepository;
            this.cartRepository = cartRepository;
        }
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                var carts = cartRepository.Get(expression: e => e.ApplicationId == user.Id, Includes: [e => e.Product]);
                ViewBag.TotalSalary = carts.Sum(e=>e.Product.Price * e.Count);
                return View(carts.ToList());
            }
            return BadRequest();
        }

        public async Task<IActionResult> AddToCart(int productId, int count)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is not null)
            {
                var product = productRepository.GetOne(e => e.Id == productId);
                if (product is not null)
                {
                    var cartInDb = cartRepository.GetFirstOrDefault(e => e.ProductId == productId && e.ApplicationId == user.Id);
                    if (cartInDb is not null)
                    {
                        cartInDb.Count += count;
                    }
                    else
                    {
                        if (count <= product.Quantity && count > 0)
                        {
                            await cartRepository.CreateAsync(new()
                            {
                                ProductId = productId,
                                ApplicationId = user.Id,
                                Count = count,
                            });
                        }
                    }

                    await cartRepository.CommitAsync();
                    TempData["Notification"] = "Add Product To Cart";

                    return RedirectToAction("Index", "Home", new { area = "Customer" });
                }
                return BadRequest();
            }
            return BadRequest();

        }
        public async Task<IActionResult> IncrementValue(int productId)
        {
            var user = await userManager.GetUserAsync(User);
            var cartInDb = cartRepository.GetFirstOrDefault(e => e.ProductId == productId && e.ApplicationId == user.Id);
            if (cartInDb is not null)
            {
                cartInDb.Count++;
               await cartRepository.CommitAsync();
                return RedirectToAction("Index");
            }
            return BadRequest();


        } 
        public async Task<IActionResult> decrement(int productId)
        {
            var user = await userManager.GetUserAsync(User);
            var cartInDb = cartRepository.GetFirstOrDefault(e => e.ProductId == productId && e.ApplicationId == user.Id);
            if (cartInDb is not null)
            {
                cartInDb.Count--;
               await cartRepository.CommitAsync();
                return RedirectToAction("Index");

            }
            return BadRequest();


        }
        public async Task<IActionResult> DeleteItemFromCart(int productId)
        {
            var user = await userManager.GetUserAsync(User);
            var cartInDb = cartRepository.GetFirstOrDefault(e => e.ProductId == productId && e.ApplicationId == user.Id);
            if (cartInDb is not null)
            {
                cartRepository.Delete(cartInDb);
               await cartRepository.CommitAsync();
                return RedirectToAction("Index");

            }
            return BadRequest();


        }

    }

        }
    

