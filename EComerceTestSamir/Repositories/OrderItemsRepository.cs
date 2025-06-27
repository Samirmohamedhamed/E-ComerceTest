using EComerceTestSamir.Data;
using EComerceTestSamir.Models;
using EComerceTestSamir.Repositories.IRepositories;

namespace EComerceTestSamir.Repositories
{
    public class OrderItemsRepository : Repository<OrderItems>, IOrderItemsRepository
    {
        private readonly ApplicationDbContext context;

        public OrderItemsRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}
