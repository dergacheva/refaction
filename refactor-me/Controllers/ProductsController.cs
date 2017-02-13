using System;
using System.Net;
using System.Web.Http;
using ProductAPI.Services;

using ContractProducts = ProductAPI.Models.Products;
using ContractProduct = ProductAPI.Models.Product;

using ContractProductOptions = ProductAPI.Models.ProductOptions;
using ContractProductOption = ProductAPI.Models.ProductOption;
using ProductAPI.Validation;

namespace ProductAPI.Controllers
{
    [RoutePrefix("products")]
    public class ProductsController : ApiController
    {
        private readonly IProductService _productService;
        private readonly IProductOptionService _productOptionService;

        public ProductsController(IProductService productService, IProductOptionService productOptionService)
        {
            _productOptionService = productOptionService;
            _productService = productService;
        }

        [Route]
        [HttpGet]
        public ContractProducts GetAll()
        {
            return _productService.Get();
        }

        [Route]
        [HttpGet]
        public ContractProducts SearchByName(string name)
        {
            return _productService.GetByName(name);
        }

        [Route("{id}")]
        [HttpGet]
        public ContractProduct GetProduct(Guid id)
        {
            var product = _productService.GetById(id);
            if (product == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return product;
        }

        [Route]
        [HttpPost]
        public void Create(ContractProduct product)
        {
            try
            {
                _productService.Create(product);
            }
            catch (InvalidAPIRequestException)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

        [Route("{id}")]
        [HttpPut]
        public void Update(Guid id, ContractProduct product)
        {
            if(product.Id != id)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            try
            {
                _productService.Update(product);
            }
            catch (InvalidAPIRequestException)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

        [Route("{id}")]
        [HttpDelete]
        public void Delete(Guid id)
        {
            try
            {
                _productService.Delete(id);
            }
            catch (InvalidAPIRequestException)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

        [Route("{productId}/options")]
        [HttpGet]
        public ContractProductOptions GetOptions(Guid productId)
        {
            return _productOptionService.GetByProductId(productId);
        }

        [Route("{productId}/options/{id}")]
        [HttpGet]
        public ContractProductOption GetOption(Guid productId, Guid id)
        {
            var option = _productOptionService.GetByProductIdAndId(productId, id);
            if (option == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return option;
        }

        [Route("{productId}/options")]
        [HttpPost]
        public void CreateOption(Guid productId, ContractProductOption option)
        {
            if (option.ProductId != productId)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            try
            {
                _productOptionService.Create(option);
            }
            catch (InvalidAPIRequestException)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

        [Route("{productId}/options/{id}")]
        [HttpPut]
        public void UpdateOption(Guid productId, Guid id, ContractProductOption option)
        {
            if (option.ProductId != productId)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            try
            {
                _productOptionService.Update(option);
            }
            catch (InvalidAPIRequestException)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

        [Route("{productId}/options/{id}")]
        [HttpDelete]
        public void DeleteOption(Guid id)
        {
            try
            {
                _productOptionService.DeleteById(id);
            }
            catch (InvalidAPIRequestException)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }
    }
}
