﻿using EComerceTestSamir.Data;
using EComerceTestSamir.Models;
using EComerceTestSamir.Repositories;
using EComerceTestSamir.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;

namespace EComerceTestSamir.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CheckoutController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IProductRepository productRepository;
        private readonly ICartRepository cartRepository;
        private readonly IOrderItemsRepository orderItemsRepository;
        private readonly IOrderRepository orderRepository;
        private readonly ApplicationDbContext applicationDbContext;

        public CheckoutController(UserManager<ApplicationUser> userManager, IProductRepository productRepository, ICartRepository cartRepository
            ,IOrderItemsRepository orderItemsRepository,IOrderRepository orderRepository, ApplicationDbContext applicationDbContext)
        {
            this.userManager = userManager;
            this.productRepository = productRepository;
            this.cartRepository = cartRepository;
            this.orderItemsRepository = orderItemsRepository;
            this.orderRepository = orderRepository;
            this.applicationDbContext = applicationDbContext;
        }
 
        public async Task<IActionResult> Pay()
        {
            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {

              
                var cartInDb = cartRepository.Get(e => e.ApplicationId == user.Id, Includes: [e => e.Product]);
                if (cartInDb.Any())
                {
                    Order order = new()
                    {
                        ApplicationId = user.Id,
                        OrderDate = DateTime.UtcNow,
                        OrderStatus = OrderStatus.pending,
                        TotalPrice = cartInDb.Sum(e => e.Product.Price * e.Count),
                        TransactionStatus = TransactionStatus.pending
                    };
                    await orderRepository.CreateAsync(order);
                    await orderRepository.CommitAsync();
                    Console.WriteLine($"Order Id: {order.Id}");

                    var options = new SessionCreateOptions
                    {
                        PaymentMethodTypes = new List<string> { "card" },// الشكل اللي علي الشمال خليه card 
                        LineItems = new List<SessionLineItemOptions>(),
                        Mode = "payment",
                        SuccessUrl = Url.Action("Success", "Checkout", new { area = "Customer", id = order.Id }, Request.Scheme),
                        CancelUrl = Url.Action("Cancel", "Checkout", new { area = "Customer",id=order.Id }, Request.Scheme),
                    };

                    foreach (var item in cartInDb)
                    {
                        options.LineItems.Add(new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "egp",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = item.Product.Name,
                                    Description = item.Product.Description,
                                },
                                UnitAmount = (long)(item.Product.Price * 100), 
                            },
                            Quantity = item.Count,
                        });
                    }

                    var service = new SessionService();
                    var session = service.Create(options);
                    order.SesstionId = session.Id;
                    await orderRepository.CommitAsync();
                    TempData["ValidNotfication"] = Guid.NewGuid().ToString();
                    return Redirect(session.Url);

                }
                return BadRequest();
            }
            return BadRequest();

        }
        public async Task<IActionResult> Success(int id)
        {
            var order1 =  orderRepository.GetOne(e => e.Id == id);

            if (order1 == null)
            {
                return NotFound("Order not found");
            }
            if (TempData["ValidNotfication"] is not null)
            {
                var order = orderRepository.GetOne(e => e.Id == id);
                var cartInDb = cartRepository.Get(e => e.ApplicationId == order.ApplicationId, Includes: [e => e.Product]);
                var transaction = await applicationDbContext.Database.BeginTransactionAsync();
                
                try
                {
                    //Decrement Quantity Of Product
                    foreach (var item in cartInDb)
                    {
                        item.Product.Quantity -= item.Count;
                    }
                  await   productRepository.CommitAsync();
                    //Transfer cart => OrderItems
                    foreach (var item in cartInDb)
                    {
                      await  orderItemsRepository.CreateAsync(new()
                        {
                            Count = item.Count,
                            ProductId = item.ProductId,
                            OrderId = id,
                            ItemPrice = item.Product.Price,

                        });
                    }
                  await  orderItemsRepository.CommitAsync();
                    //Remove Cart
                    foreach (var item in cartInDb)
                    {
                        cartRepository.Delete(item);
                    }
                    await cartRepository.CommitAsync();

                    //Update Order Prop
                    order.OrderStatus = OrderStatus.Inprocessing;
                    order.TransactionStatus = TransactionStatus.Completed;
                    var service = new SessionService();
                    var session = service.Get(order.SesstionId);
                   await orderRepository.CommitAsync();
                    transaction.Commit();
                    return View();
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                }


            }
            return BadRequest();
        }
        public IActionResult Cancel()
        {
            return View();
        }


    }
}
