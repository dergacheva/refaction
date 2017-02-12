using System;
using System.Net;
using System.Web.Http;
using ProductAPI.Models;
using System.Linq;
using AutoMapper;

namespace ProductAPI.Controllers
{
    [RoutePrefix("products")]
    public class ProductsController : ApiController
    {
        private readonly ProductDBContext _dbContext;

        public ProductsController(ProductDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Route]
        [HttpGet]
        public Products GetAll()
        {
            return Mapper.Map<Products>(_dbContext.Products.ToList());
        }

        [Route]
        [HttpGet]
        public Products SearchByName(string name)
        {
            var products = _dbContext.Products.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
            return Mapper.Map<Products>(products);
        }

        [Route("{id}")]
        [HttpGet]
        public Models.Product GetProduct(Guid id)
        {
            var product = _dbContext.Products.Find(id);
            if (product == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return Mapper.Map<Models.Product>(product);
        }

        [Route]
        [HttpPost]
        public void Create(Models.Product product)
        {
            var domainProduct = Mapper.Map<Product>(product);
            _dbContext.Products.Add(domainProduct);
            _dbContext.SaveChanges();
        }

        [Route("{id}")]
        [HttpPut]
        public void Update(Guid id, Models.Product product)
        {
            var updated = Mapper.Map<Product>(product);
            var original = _dbContext.Products.Find(id);

            if (original != null)
            {
                _dbContext.Entry(original).CurrentValues.SetValues(updated);
                _dbContext.SaveChanges();
            }
        }

        [Route("{id}")]
        [HttpDelete]
        public void Delete(Guid id)
        {
            var original = _dbContext.Products.Find(id);

            if (original != null)
            {
                _dbContext.ProductOptions.RemoveRange(_dbContext.ProductOptions.Where(x => x.ProductId == id));
                _dbContext.Products.Remove(original);
                _dbContext.SaveChanges();
            }
        }

        [Route("{productId}/options")]
        [HttpGet]
        public ProductOptions GetOptions(Guid productId)
        {
            var options = _dbContext.ProductOptions.Where(x => x.ProductId == productId).ToList();
            return Mapper.Map<ProductOptions>(options);
        }

        [Route("{productId}/options/{id}")]
        [HttpGet]
        public Models.ProductOption GetOption(Guid productId, Guid id)
        {
            var option = _dbContext.ProductOptions.Find(id);
            if (option == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return Mapper.Map<Models.ProductOption>(option);
        }

        [Route("{productId}/options")]
        [HttpPost]
        public void CreateOption(Guid productId, Models.ProductOption option)
        {
            var domainOption = Mapper.Map<ProductOption>(option);
            _dbContext.ProductOptions.Add(domainOption);
            _dbContext.SaveChanges();
        }

        [Route("{productId}/options/{id}")]
        [HttpPut]
        public void UpdateOption(Guid id, Models.ProductOption option)
        {
            var updated = Mapper.Map<ProductOption>(option);
            var original = _dbContext.ProductOptions.Find(id);

            if (original != null)
            {
                _dbContext.Entry(original).CurrentValues.SetValues(updated);
                _dbContext.SaveChanges();
            }
        }

        [Route("{productId}/options/{id}")]
        [HttpDelete]
        public void DeleteOption(Guid id)
        {
            var original = _dbContext.ProductOptions.Find(id);
            if (original != null)
            {
                _dbContext.ProductOptions.Remove(original);
                _dbContext.SaveChanges();
            }
        }
    }
}
