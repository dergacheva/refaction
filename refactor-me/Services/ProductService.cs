using AutoMapper;
using ProductAPI.Models;
using ProductAPI.Validation;
using System;
using System.Linq;

namespace ProductAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductDBContext _dbContext;
        private readonly IProductOptionService _productOptionService;

        public ProductService(ProductDBContext dbContext, IProductOptionService productOptionService)
        {
            _dbContext = dbContext;
            _productOptionService = productOptionService;
        }

        public Products Get()
        {
            return Mapper.Map<Products>(_dbContext.Products.ToList());
        }

        public Products GetByName(string name)
        {
            var products = _dbContext.Products.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
            return Mapper.Map<Products>(products);
        }

        public Models.Product GetById(Guid id)
        {
            var product = _dbContext.Products.Find(id);
            return Mapper.Map<Models.Product>(product);
        }

        public void Create(Models.Product product)
        {
            var original = _dbContext.Products.Find(product.Id);
            if (original != null)
            {
                throw new InvalidAPIRequestException("Id is not unique");
            }
            var domainProduct = Mapper.Map<Product>(product);
            _dbContext.Products.Add(domainProduct);
            _dbContext.SaveChanges();
        }

        public void Update(Models.Product product)
        {
            var updated = Mapper.Map<Product>(product);
            var original = _dbContext.Products.Find(updated.Id);

            if (original == null)
            {
                throw new InvalidAPIRequestException("Product does not exist");
            }

            _dbContext.Entry(original).CurrentValues.SetValues(updated);
            _dbContext.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var original = _dbContext.Products.Find(id);

            if (original == null)
            {
                throw new InvalidAPIRequestException("Product does not exist");
            }
            _productOptionService.DeleteByProductId(id);
            _dbContext.Products.Remove(original);
            _dbContext.SaveChanges();
        }
    }
}