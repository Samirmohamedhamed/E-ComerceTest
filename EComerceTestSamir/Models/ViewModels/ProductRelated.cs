namespace EComerceTestSamir.Models.ViewModels
{
    public class ProductRelated
    {
        public Product product { get; set; } = null!;
        public List<Product> relatedProduct { get; set; } = null!;
        public List<Product>relatedCategory { get; set; } = null!;
        public List<Product> TopProduct { get; set; } = null!;
    }
}
