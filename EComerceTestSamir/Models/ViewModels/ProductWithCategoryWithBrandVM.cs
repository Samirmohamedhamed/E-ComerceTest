namespace EComerceTestSamir.Models.ViewModels
{
    public class ProductWithCategoryWithBrandVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public double Rate { get; set; }
        public bool Status { get; set; }
        public String CategoryName{ get; set; }= string.Empty;
        public String BrandName { get; set; }= string.Empty;
    }
}
