using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SampleWebApi.Model;
using SampleWebApi.Repos;
using SampleWebApi.Request;
using SampleWebApi.Response;
using SampleWebApi.Response.V2;
using System.Text;

namespace SampleWebApi.Controllers.V2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/products")]
    [Route("api/v{version:apiVersion}/products")]
    public class ProductController : ControllerBase
    {

        private readonly ProductRepository _repository;

        public ProductController(ProductRepository repository)
        {
            _repository = repository;
        }

       
        [HttpGet("{productId:guid}", Name = "GetProductById")]
        public ActionResult<ProductResponse> GetProductById(Guid productId, bool includeReviews = false)
        {
            var product = _repository.GetProductById(productId);

            if (product is null)
                return NotFound($"Product with Id '{productId}' not found");

            List<ProductReview>? reviews = null;

            if (includeReviews == true)
            {
                reviews = _repository.GetProductReviews(productId);
            }

            return ProductResponse.FromModel(product, reviews);
        }






    }
}
