using ProductAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductAPI.Services
{
    public interface IProductOptionService
    {
        ProductOptions GetByProductId(Guid productId);

        Models.ProductOption GetByProductIdAndId(Guid productId, Guid id);

        void Create(Models.ProductOption option);

        void Update(Models.ProductOption option);

        void DeleteById(Guid id);

        void DeleteByProductId(Guid id);
    }
}
