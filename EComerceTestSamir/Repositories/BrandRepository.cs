using EComerceTestSamir.Data;
using EComerceTestSamir.Models;
using EComerceTestSamir.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EComerceTestSamir.Repositories
{
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        private readonly ApplicationDbContext context;

        public BrandRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}

