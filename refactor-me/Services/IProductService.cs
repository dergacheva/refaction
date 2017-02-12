using ProductAPI.Models;
using System;

namespace ProductAPI.Services
{
    public interface IProductService
    {
        Products Get();

        Products GetByName(string name);

        Models.Product GetById(Guid id);

        void Create(Models.Product product);

        void Update(Models.Product product);

        void Delete(Guid id);
    }
}
