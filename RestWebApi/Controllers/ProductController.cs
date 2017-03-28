using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using BusinessEntities;
using BusinessServices;
using RestWebApi.ErrorHelper;
using RestWebApi.Filters;


namespace RestWebApi.Controllers
{
    [AuthorizationRequired]
    [RoutePrefix("v1/Products/Product")]
    public class ProductController : ApiController
    {

        private readonly IProductServices _productServices;

        public ProductController(IProductServices productServices)
        {
            _productServices = productServices;
        }

        // GET: api/Product
        [EnableQuery(PageSize = 5, AllowedQueryOptions = AllowedQueryOptions.Filter | AllowedQueryOptions.OrderBy, AllowedOrderByProperties = "ProductId")]
        [Route("~/MyRoute/allproducts")]
        public HttpResponseMessage Get()
        {
            var products = _productServices.GetAllProducts();
            if (products != null)
            {
                var productEntities = products as List<ProductEntity> ?? products.ToList();
                if (productEntities.Any())
                    return Request.CreateResponse(HttpStatusCode.OK, productEntities.AsQueryable());
            }
            throw new ApiDataException(1000,"Products not found",HttpStatusCode.NotFound);
        }

        // GET: api/Product/5
        [Route("productid/{id?}")]
        [Route("particularproduct/{id?}")]
        [Route("myproduct/{id:range(1,3)}")]
        [Route(@"id/{e:regex(^[0-9]$)}")]
        public HttpResponseMessage Get(int? id)
        {
            if (id != null)
            {
                var product = _productServices.GetProductById(id.Value);
                if (product != null)
                    return Request.CreateResponse(HttpStatusCode.OK, product);
                throw new ApiDataException(1001,"No product found for this id.",HttpStatusCode.NotFound);
            }
            throw new ApiException() {ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..."};
            //return Request.CreateResponse(HttpStatusCode.NotFound, "No product found for this id");
        }

        // POST: api/Product
        public int Post([FromBody]ProductEntity productEntity)
        {
            return _productServices.CreateProduct(productEntity);
        }

        // PUT: api/Product/5
        public bool Put(int id, [FromBody]ProductEntity productEntity)
        {
            if (id > 0)
            {
                return _productServices.UpdateProduct(id, productEntity);
            }
            return false;
        }

        // DELETE: api/Product/5
        public bool Delete(int? id)
        {
            if (id != null && id > 0)
            {
                var success = _productServices.DeleteProduct(id.Value);
                if(success) return success;
                throw new ApiDataException(1002,"Product is already deleted or not exist in system.", HttpStatusCode.NoContent);
            }
            throw new ApiException() {ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..."};
            
        }
    }
}
