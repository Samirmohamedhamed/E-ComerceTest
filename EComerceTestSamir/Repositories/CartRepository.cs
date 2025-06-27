using EComerceTestSamir.Data;
using EComerceTestSamir.Models;
using EComerceTestSamir.Repositories.IRepositories;

namespace EComerceTestSamir.Repositories
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        private readonly ApplicationDbContext context;

        public CartRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}
