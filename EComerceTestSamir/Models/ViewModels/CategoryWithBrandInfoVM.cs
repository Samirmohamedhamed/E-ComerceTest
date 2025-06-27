namespace EComerceTestSamir.Models.ViewModels
{
    public class CategoryWithBrandInfoVM
    {
        public List<Brand> Brands { get; set; } = [];
        public List<Category> Categories { get; set; } = [];
        public Product Product { get; set; } = null!;
    }
}
