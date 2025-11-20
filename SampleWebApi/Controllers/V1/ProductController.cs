using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SampleWebApi.Filters;
using SampleWebApi.Model;
using SampleWebApi.Repos;
using SampleWebApi.Request;
using SampleWebApi.Response;
using SampleWebApi.Response.V1;
using SampleWebApi.Response.V2;
using System.Text;
using ProductResponse = SampleWebApi.Response.V1.ProductResponse;

namespace SampleWebApi.Controllers.V1
{
    [ApiController] 
    [ApiVersion("1.0")]
    //[Route("api/products")]
    [Route("api/v{version:apiVersion}/products")]
    [TrackActionTimeFilter]
    [Tags("Products")]
    public class ProductController : ControllerBase
    {
        /*
         
         ما أصاب عبدا هم ولا حزن فقال : اللهم إني عبدك وابن عبدك وابن أمتك ناصيتي بيدك ماض في حكمك عدل في قضاؤك ، أسألك بكل اسم هو لك سميت به نفسك أو أنزلته في كتابك أو علمته أحدا من خلقك أو استأثرت به في علم الغيب عندك أن تجعل القرآن ربيع قلبي و نور صدري وجلاء حزني وذهاب همي ، إلا أذهب الله همه و حزنه وأبدله مكانه فرجا
         
         
         */

        private readonly ProductRepository _repository;

        public ProductController(ProductRepository repository)
        {
            _repository = repository;
        }


        [HttpOptions]
        public IActionResult OptionsProducts()
        {
            Response.Headers.Append("Allow", "GET, HEAD, POST, PUT, PATCH, DELETE, OPTIONS");
            return NoContent();
        }

        [HttpHead("{productId:guid}")]
        public IActionResult HeadProduct(Guid productId)
        {
            return _repository.ExistsById(productId) ? Ok() : NotFound();
        }

        [HttpGet]
        public IActionResult GetPaged(int page = 1, int pageSize = 10)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            int totalCount = _repository.GetProductsCount();

            var products = _repository.GetProductsPage(page, pageSize);

            var pagedResult = PagedResult<ProductResponse>.Create(
                ProductResponse.FromModels(products),
                totalCount,
                page,
                pageSize);

            return Ok(pagedResult);
        }


        [HttpGet("{productId:guid}")]
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



        [HttpPost("CreateProduct")]
        [Consumes("application/json")]
        [ProducesResponseType<ProductResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Creates a new Product")]
        [EndpointDescription("Creates a new Product and returns the created result.")]
        public IActionResult CreateProduct(CreateProductRequest request)
        {
            if (_repository.ExistsByName(request.Name))
                return Conflict($"A product with the name '{request.Name}' already exists.");

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Price = request.Price
            };

            _repository.AddProduct(product);

            return CreatedAtRoute(routeName: nameof(GetProductById),
                                  routeValues: new { productId = product.Id },
                                  value: ProductResponse.FromModel(product));
        }


        [HttpPost("CreateProductReview/{productId}/reviews")]
        public IActionResult CreateProductReview(Guid productId,CreateProductReviewRequest reviewRequest)
        {          
            var productReview = new ProductReview
            {
                Id = Guid.NewGuid(),
                Reviewer = reviewRequest.Reviewer,
               Stars = reviewRequest.Stars,
               ProductId = productId
            };

            _repository.AddProductReview(productReview);

            return CreatedAtRoute(routeName: nameof(GetProductById),
                                   routeValues: 
                                   new { productId, incincludeReviewslude = true },
                                   value: ProductReviewResponse.FromModel(productReview));
        }



        [HttpPut("{productId:guid}")]
        public IActionResult Put(Guid productId, UpdateProductRequest request)
        {
            var product = _repository.GetProductById(productId);

            if (product is null)
                return NotFound($"Product with Id '{productId}' not found");

            product.Name = request.Name;
            product.Price = request.Price ?? 0;

            var succeeded = _repository.UpdateProduct(product);

            if (!succeeded)
                return StatusCode(500, "Failed to update product");

            return NoContent();
        }


        [HttpPatch("{productId:guid}")]
        public IActionResult Patch(Guid productId, JsonPatchDocument<UpdateProductRequest>? patchDoc)
        {
            if (patchDoc is null)
                return BadRequest("Invalid patch document.");

            var product = _repository.GetProductById(productId);

            if (product is null)
                return NotFound($"Product with Id '{productId}' not found.");

            var updateModel = new UpdateProductRequest
            {
                Name = product.Name,
                Price = product.Price
            };

            patchDoc.ApplyTo(updateModel);

            product.Name = updateModel.Name;
            product.Price = updateModel.Price ?? 0;

            var succeeded = _repository.UpdateProduct(product);

            if (!succeeded)
                return StatusCode(500, "Failed to patch product");

            return NoContent();
        }


        [HttpDelete("{productId:guid}")]
        public IActionResult Delete(Guid productId)
        {
            if (!_repository.ExistsById(productId))
                return NotFound($"Product with Id '{productId}' not found");

            var succeeded = _repository.DeleteProduct(productId);

            if (!succeeded)
                return StatusCode(500, "Failed to update product");

            return NoContent();
        }


        [HttpPost("process")]
        public IActionResult ProcessAsync()
        {
            var jobId = Guid.NewGuid();

            return Accepted(
                $"/api/products/status/{jobId}",
                new { jobId, status = "Processing" }
            );
        }


        [HttpGet("status/{jobId}")]
        public IActionResult GetProcessingStatus(Guid jobId)
        {
            var isStillProcessing = false; // fake it

            return Ok(new { jobId, status = isStillProcessing ? "Processing" : "Completed" });
        }

        [HttpGet("csv")]
        public IActionResult GetProductsCSV()
        {
            var products = _repository.GetProductsPage(1, 100);

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Id,Name,Price");

            foreach (var p in products)
            {
                csvBuilder.AppendLine($"{p.Id},{p.Name},{p.Price}");
            }

            var fileBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

            return File(fileBytes, "text/csv", "product-catalog_1_100.csv");
        }


        [HttpGet("physical-csv-file")]
        public IActionResult GetPhysicalFile()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "products.csv");

            return PhysicalFile(filePath, "text/csv", "products-export.csv");
        }


        [HttpGet("products-legacy")]
        public IActionResult GetRedirect()
        {
            return Redirect("/api/products/temp-products");
        }


        [HttpGet("temp-products")]
        public IActionResult TempProducts()
        {
            return Ok(new { message = "You're in the temp endpoint. Chill." });
        }


        [HttpGet("legacy-products")]
        public IActionResult GetPermanentRedirect()
        {
            return RedirectPermanent("/api/products/product-catalog");
        }



        [HttpGet("product-catalog")]
        public IActionResult Catalog()
        {
            return Ok(new { message = "This is the permanent new location." });
        }







    }
}
