using Ninject.Modules;
using Ninject.Web.Common;
using System.Data.Entity;
using System.Web;
using System.IO;
using Ninject.Parameters;
using System.Data.Entity.Core.EntityClient;
using ProductAPI.Services;

namespace ProductAPI
{
    public class WebModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ProductDBContext>().ToSelf().InRequestScope();
            Bind<IProductOptionService>().To<ProductOptionService>();
            Bind<IProductService>().To<ProductService>();
        }
    }
}