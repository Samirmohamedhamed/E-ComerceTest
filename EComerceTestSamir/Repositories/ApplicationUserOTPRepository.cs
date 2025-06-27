using EComerceTestSamir.Data;
using EComerceTestSamir.Models;
using EComerceTestSamir.Repositories.IRepositories;

namespace EComerceTestSamir.Repositories
{
    public class ApplicationUserOTPRepository : Repository<ApplicationUserOTP>, IApplicationUserOTPRepository
    {
        public ApplicationUserOTPRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
