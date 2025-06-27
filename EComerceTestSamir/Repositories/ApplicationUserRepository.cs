using EComerceTestSamir.Data;
using EComerceTestSamir.Models;
using EComerceTestSamir.Repositories.IRepositories;

namespace EComerceTestSamir.Repositories
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext context;

        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}
