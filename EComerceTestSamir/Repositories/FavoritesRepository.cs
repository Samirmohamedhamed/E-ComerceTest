using EComerceTestSamir.Data;
using EComerceTestSamir.Models;
using EComerceTestSamir.Repositories.IRepositories;

namespace EComerceTestSamir.Repositories
{
    public class FavoritesRepository : Repository<Favorite>, IFavoritesRepository
    {
        private readonly ApplicationDbContext context;

        public FavoritesRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}
