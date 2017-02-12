using System;
using System.Net;
using System.Web.Http;
using ProductAPI.Models;
using System.Linq;
using AutoMapper;
using ProductAPI.Services;

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
        public Products GetAll()
        {
            return _productService.Get();
        }

        [Route]
        [HttpGet]
        public Products SearchByName(string name)
        {
            return _productService.GetByName(name);
        }

        [Route("{id}")]
        [HttpGet]
        public Models.Product GetProduct(Guid id)
        {
            var product = _productService.GetById(id);
            if (product == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return product;
        }

        [Route]
        [HttpPost]
        public void Create(Models.Product product)
        {
            _productService.Create(product);
        }

        [Route("{id}")]
        [HttpPut]
        public void Update(Guid id, Models.Product product)
        {
            _productService.Update(product);
        }

        [Route("{id}")]
        [HttpDelete]
        public void Delete(Guid id)
        {
            _productService.Delete(id);
        }

        [Route("{productId}/options")]
        [HttpGet]
        public ProductOptions GetOptions(Guid productId)
        {
            return _productOptionService.GetByProductId(productId);
        }

        [Route("{productId}/options/{id}")]
        [HttpGet]
        public Models.ProductOption GetOption(Guid productId, Guid id)
        {
            return _productOptionService.GetById(id);
        }

        [Route("{productId}/options")]
        [HttpPost]
        public void CreateOption(Guid productId, Models.ProductOption option)
        {
            _productOptionService.Create(option);
        }

        [Route("{productId}/options/{id}")]
        [HttpPut]
        public void UpdateOption(Guid id, Models.ProductOption option)
        {
            _productOptionService.Update(option);
        }

        [Route("{productId}/options/{id}")]
        [HttpDelete]
        public void DeleteOption(Guid id)
        {
            _productOptionService.DeleteById(id);
        }
    }
}
