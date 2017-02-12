using AutoMapper;
using ProductAPI.Models;
using System;
using System.Linq;

namespace ProductAPI.Services
{
    public class ProductOptionService : IProductOptionService
    {
        private readonly ProductDBContext _dbContext;

        public ProductOptionService(ProductDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ProductOptions GetByProductId(Guid productId)
        {
            var options = _dbContext.ProductOptions.Where(x => x.ProductId == productId).ToList();
            return Mapper.Map<ProductOptions>(options);
        }

        public Models.ProductOption GetById(Guid id)
        {
            var option = _dbContext.ProductOptions.Find(id);
            return Mapper.Map<Models.ProductOption>(option);
        }

        public void Create(Models.ProductOption option)
        {
            var domainOption = Mapper.Map<ProductOption>(option);
            _dbContext.ProductOptions.Add(domainOption);
            _dbContext.SaveChanges();
        }

        public void Update(Models.ProductOption option)
        {
            var updated = Mapper.Map<ProductOption>(option);
            var original = _dbContext.ProductOptions.Find(updated.Id);

            if (original == null) return;

            _dbContext.Entry(original).CurrentValues.SetValues(updated);
            _dbContext.SaveChanges();
        }

        public void DeleteById(Guid id)
        {
            var original = _dbContext.ProductOptions.Find(id);
            if (original == null)
                return;
            _dbContext.ProductOptions.Remove(original);
            _dbContext.SaveChanges();
        }

        public void DeleteByProductId(Guid id)
        {
            _dbContext.ProductOptions.RemoveRange(_dbContext.ProductOptions.Where(x => x.ProductId == id));
            _dbContext.SaveChanges();
        }
    }
}