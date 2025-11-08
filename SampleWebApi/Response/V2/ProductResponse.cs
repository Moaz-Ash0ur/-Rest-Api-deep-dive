using SampleWebApi.Model;

namespace SampleWebApi.Response.V2
{
    public sealed class ProductResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public PriceResponse Price { get; set; } = null!;
        public List<ProductReviewResponse> Reviews { get; set; } = new List<ProductReviewResponse>();



        private ProductResponse()
        { }


        public static ProductResponse FromModel(Product product, IEnumerable<ProductReview>? reviews = null)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product), "Cannot create a response from a null product");

            var response = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Price = new PriceResponse
                {
                    Amount = product.Price,
                    Currency = "USD"
                }
            };

            if (reviews != null)
                response.Reviews = ProductReviewResponse.FromModels(reviews).ToList();


            return response;
        }
  
        public static IEnumerable<ProductResponse> FromModels(IEnumerable<Product> products)
        {
            ArgumentNullException.ThrowIfNull(products);

            return products.Select(p => FromModel(p));
        }
    }

}
